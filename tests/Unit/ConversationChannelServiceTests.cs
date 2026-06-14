using System.Text;
using ITSupportNative.Catalog.Application;
using ITSupportNative.Catalog.Fixtures;
using ITSupportNative.Contracts.ControlPlane;
using ITSupportNative.Contracts.Conversation;
using ITSupportNative.Conversation.Application;
using ITSupportNative.Conversation.Channels;
using ITSupportNative.Conversation.Domain;

namespace ITSupportNative.UnitTests;

public sealed class ConversationChannelServiceTests
{
    [Fact]
    public async Task QueryReturnsDecisionWithoutControlPlaneMutation()
    {
        var controlPlane = new RecordingControlPlane();
        ConversationChannelService service = CreateService(controlPlane);
        ConversationSession session = ConversationSession.Start("query-session");

        ConversationChannelTurn turn = await service.HandleAsync(
            session,
            Input(
                session,
                "query-1",
                ConversationChannelActions.CatalogQuery,
                productReference: "secure-transfer"),
            CancellationToken.None);

        Assert.Equal("query_answered", turn.Output.ResultCode);
        Assert.Equal("inform", turn.Output.Decision?.Kind);
        Assert.Null(turn.Output.Request);
        Assert.Equal(0, controlPlane.UniqueMutationCount);
        Assert.Equal(0, controlPlane.ReadCount);
    }

    [Fact]
    public async Task ConfirmedRequestIsDurableAndIdempotent()
    {
        var controlPlane = new RecordingControlPlane();
        ConversationChannelService service = CreateService(controlPlane);
        ConversationSession session =
            ConversationSession.Start("confirmed-session");

        ConversationChannelTurn proposal = await service.HandleAsync(
            session,
            Input(
                session,
                "request-1",
                ConversationChannelActions.SoftwareRequest,
                productReference: "secure-transfer"),
            CancellationToken.None);
        ConversationChannelTurn confirmation = await service.HandleAsync(
            proposal.Session,
            Input(
                proposal.Session,
                "continue-1",
                ConversationChannelActions.ProposalContinue),
            CancellationToken.None);
        ConversationChannelInput confirmInput = Input(
            confirmation.Session,
            "confirm-1",
            ConversationChannelActions.RequestConfirm,
            idempotencyKey:
                ConversationChannelIdempotency.Create(confirmation.Session));

        ConversationChannelTurn created = await service.HandleAsync(
            confirmation.Session,
            confirmInput,
            CancellationToken.None);
        ConversationChannelTurn retried = await service.HandleAsync(
            created.Session,
            confirmInput,
            CancellationToken.None);

        Assert.Equal("request_created", created.Output.ResultCode);
        Assert.False(created.Output.Request?.Replayed);
        Assert.Equal("duplicate_command", retried.Output.ResultCode);
        Assert.True(retried.Output.Request?.Replayed);
        Assert.Equal(1, controlPlane.UniqueMutationCount);
        Assert.Equal(2, controlPlane.CreateCallCount);
    }

    [Fact]
    public async Task HumanReviewRequiresConfirmationButDoesNotCreateInstallation()
    {
        var controlPlane = new RecordingControlPlane();
        ConversationChannelService service = CreateService(controlPlane);
        ConversationSession session = ConversationSession.Start("review-session");

        ConversationChannelTurn proposal = await service.HandleAsync(
            session,
            Input(
                session,
                "request-1",
                ConversationChannelActions.SoftwareRequest,
                productReference: "insight-studio"),
            CancellationToken.None);
        ConversationChannelTurn confirmation = await service.HandleAsync(
            proposal.Session,
            Input(
                proposal.Session,
                "continue-1",
                ConversationChannelActions.ProposalContinue),
            CancellationToken.None);
        ConversationChannelTurn confirmed = await service.HandleAsync(
            confirmation.Session,
            Input(
                confirmation.Session,
                "confirm-1",
                ConversationChannelActions.RequestConfirm,
                idempotencyKey:
                    ConversationChannelIdempotency.Create(
                        confirmation.Session)),
            CancellationToken.None);

        Assert.Equal("escalate", proposal.Output.Decision?.Kind);
        Assert.Equal("capability_unavailable", confirmed.Output.ResultCode);
        Assert.Equal("human_review", confirmed.Output.Request?.Kind);
        Assert.Equal(0, controlPlane.UniqueMutationCount);
    }

    [Fact]
    public async Task DuplicateCommandWithAnotherIdempotencyKeyCannotMutate()
    {
        var controlPlane = new RecordingControlPlane();
        ConversationChannelService service = CreateService(controlPlane);
        ConversationSession session =
            ConversationSession.Start("mismatch-session");

        ConversationChannelTurn proposal = await service.HandleAsync(
            session,
            Input(
                session,
                "request-1",
                ConversationChannelActions.SoftwareRequest,
                productReference: "secure-transfer"),
            CancellationToken.None);
        ConversationChannelTurn confirmation = await service.HandleAsync(
            proposal.Session,
            Input(
                proposal.Session,
                "continue-1",
                ConversationChannelActions.ProposalContinue),
            CancellationToken.None);
        ConversationChannelInput confirmedInput = Input(
            confirmation.Session,
            "confirm-1",
            ConversationChannelActions.RequestConfirm,
            idempotencyKey:
                ConversationChannelIdempotency.Create(confirmation.Session));
        ConversationChannelTurn created = await service.HandleAsync(
            confirmation.Session,
            confirmedInput,
            CancellationToken.None);

        ConversationChannelTurn mismatch = await service.HandleAsync(
            created.Session,
            confirmedInput with { IdempotencyKey = "another-key" },
            CancellationToken.None);

        Assert.Equal("invalid_transition", mismatch.Output.ResultCode);
        Assert.Equal(
            "idempotency_key_mismatch",
            mismatch.Output.Error?.Code);
        Assert.Equal(1, controlPlane.CreateCallCount);
        Assert.Equal(1, controlPlane.UniqueMutationCount);
    }

    [Fact]
    public async Task StatusQueriesUseReadOnlyControlPlaneCapabilities()
    {
        var controlPlane = new RecordingControlPlane();
        ConversationChannelService service = CreateService(controlPlane);
        ConversationSession session = ConversationSession.Start("status-session");

        ConversationChannelTurn requestStatus = await service.HandleAsync(
            session,
            Input(
                session,
                "status-1",
                ConversationChannelActions.RequestStatus,
                requestId: RecordingControlPlane.RequestId),
            CancellationToken.None);
        ConversationChannelTurn caseStatus = await service.HandleAsync(
            session,
            Input(
                session,
                "case-1",
                ConversationChannelActions.CaseStatus,
                requestId: RecordingControlPlane.RequestId),
            CancellationToken.None);

        Assert.Equal(
            "request_status_returned",
            requestStatus.Output.ResultCode);
        Assert.Equal("completed", requestStatus.Output.Status?.RequestStatus);
        Assert.Equal("case_status_returned", caseStatus.Output.ResultCode);
        Assert.Equal("escalated", caseStatus.Output.Status?.CaseStatus);
        Assert.Equal(
            "FAKE-0000000000000001",
            caseStatus.Output.Status?.ExternalTicketReference);
        Assert.Equal(0, controlPlane.UniqueMutationCount);
        Assert.Equal(2, controlPlane.ReadCount);
    }

    [Fact]
    public void RecordedChannelRejectsExecutableFieldsWithSanitizedError()
    {
        const string payload = """
            {
              "version": 1,
              "messageId": "message-1",
              "correlationId": "correlation-1",
              "sessionId": "session-1",
              "actorSubject": "development-user-001",
              "deviceId": "local-device-001",
              "action": "request.confirm",
              "productReference": null,
              "requestId": null,
              "idempotencyKey": "confirmation-key-1",
              "script": "untrusted"
            }
            """;
        var channel = new RecordedTeamsConversationChannel();

        InvalidDataException exception = Assert.Throws<InvalidDataException>(
            () => channel.Read(Encoding.UTF8.GetBytes(payload)));

        Assert.Equal(
            "The conversation channel payload is invalid.",
            exception.Message);
        Assert.Empty(channel.Inputs);
    }

    private static ConversationChannelService CreateService(
        IConversationControlPlane controlPlane)
    {
        return new(
            new ConversationService(
                new CatalogDecisionService(SyntheticCatalog.Products)),
            controlPlane);
    }

    private static ConversationChannelInput Input(
        ConversationSession session,
        string messageId,
        string action,
        string? productReference = null,
        string? requestId = null,
        string? idempotencyKey = null)
    {
        return new(
            ConversationChannelProtocol.Version,
            messageId,
            $"correlation-{messageId}",
            session.Id,
            "development-user-001",
            "local-device-001",
            action,
            productReference,
            requestId,
            idempotencyKey);
    }

    private sealed class RecordingControlPlane : IConversationControlPlane
    {
        public const string RequestId =
            "11111111-1111-4111-8111-111111111111";
        private readonly Dictionary<string, CreateSoftwareInstallationData>
            _requests = new(StringComparer.Ordinal);

        public int CreateCallCount { get; private set; }

        public int UniqueMutationCount => _requests.Count;

        public int ReadCount { get; private set; }

        public Task<CreateSoftwareInstallationData?>
            CreateSoftwareInstallationAsync(
                string correlationId,
                string idempotencyKey,
                string deviceId,
                string productId,
                string productVersion,
                CancellationToken cancellationToken)
        {
            CreateCallCount++;
            if (_requests.TryGetValue(idempotencyKey, out var existing))
            {
                return Task.FromResult<CreateSoftwareInstallationData?>(
                    existing with { Replayed = true });
            }

            var request = new ControlPlaneSupportRequest(
                RequestId,
                "REQ-0000000000000001",
                correlationId,
                "confirmed",
                deviceId,
                productId,
                productVersion,
                "software.install.simulated.v1",
                new DateTimeOffset(
                    2026,
                    6,
                    14,
                    12,
                    0,
                    0,
                    TimeSpan.Zero),
                new(
                    "22222222-2222-4222-8222-222222222222",
                    "queued",
                    []));
            var created = new CreateSoftwareInstallationData(
                request,
                Replayed: false);
            _requests.Add(idempotencyKey, created);
            return Task.FromResult<CreateSoftwareInstallationData?>(created);
        }

        public Task<ControlPlaneSupportRequest?> GetSupportRequestAsync(
            string correlationId,
            string requestId,
            CancellationToken cancellationToken)
        {
            ReadCount++;
            return Task.FromResult<ControlPlaneSupportRequest?>(
                new(
                    requestId,
                    "REQ-0000000000000001",
                    correlationId,
                    "completed",
                    "local-device-001",
                    "secure-transfer",
                    "6.5",
                    "software.install.simulated.v1",
                    new DateTimeOffset(
                        2026,
                        6,
                        14,
                        12,
                        0,
                        0,
                        TimeSpan.Zero),
                    new(
                        "22222222-2222-4222-8222-222222222222",
                        "completed",
                        [])));
        }

        public Task<ControlPlaneBotCase?> GetBotCaseAsync(
            string correlationId,
            string requestId,
            CancellationToken cancellationToken)
        {
            ReadCount++;
            var createdAt = new DateTimeOffset(
                2026,
                6,
                14,
                12,
                0,
                0,
                TimeSpan.Zero);
            return Task.FromResult<ControlPlaneBotCase?>(
                new(
                    "33333333-3333-4333-8333-333333333333",
                    requestId,
                    correlationId,
                    "software_installation",
                    "escalated",
                    "failed",
                    WaitingForUserSince: null,
                    EscalatedAt: createdAt,
                    createdAt,
                    createdAt,
                    new(
                        "44444444-4444-4444-8444-444444444444",
                        "fake",
                        "FAKE-0000000000000001",
                        "software_installation",
                        "open",
                        correlationId,
                        "execution_failed",
                        "Synthetic escalation.",
                        createdAt)));
        }
    }
}
