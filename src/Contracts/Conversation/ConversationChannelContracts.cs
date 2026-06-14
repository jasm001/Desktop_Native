namespace ITSupportNative.Contracts.Conversation;

public static class ConversationChannelProtocol
{
    public const int Version = 1;
}

public static class ConversationChannelActions
{
    public const string CatalogQuery = "catalog.query";
    public const string SoftwareRequest = "software.request";
    public const string ProposalContinue = "proposal.continue";
    public const string RequestConfirm = "request.confirm";
    public const string ConversationCancel = "conversation.cancel";
    public const string RequestStatus = "request.status";
    public const string CaseStatus = "case.status";

    public static IReadOnlySet<string> Allowed { get; } = new HashSet<string>(
        [
            CatalogQuery,
            SoftwareRequest,
            ProposalContinue,
            RequestConfirm,
            ConversationCancel,
            RequestStatus,
            CaseStatus,
        ],
        StringComparer.Ordinal);
}

public sealed record ConversationChannelInput(
    int Version,
    string MessageId,
    string CorrelationId,
    string SessionId,
    string ActorSubject,
    string DeviceId,
    string Action,
    string? ProductReference,
    string? RequestId,
    string? IdempotencyKey);

public sealed record ConversationChannelOutput(
    int Version,
    string MessageId,
    string CorrelationId,
    string SessionId,
    string State,
    string ResultCode,
    ConversationChannelDecision? Decision,
    ConversationChannelRequestView? Request,
    ConversationChannelStatusView? Status,
    ConversationChannelError? Error);

public sealed record ConversationChannelDecision(
    string Kind,
    string EffectiveStatus,
    string? ProductId,
    string? ProductName,
    string? ProductVersion,
    IReadOnlyList<ConversationChannelAlternative> Alternatives);

public sealed record ConversationChannelAlternative(
    string ProductId,
    string ProductName,
    string ProductVersion);

public sealed record ConversationChannelRequestView(
    string Kind,
    string ProductId,
    string ProductVersion,
    string IdempotencyKey,
    string SyntheticReference,
    string? PersistedRequestId,
    string? PersistedReference,
    bool? Replayed);

public sealed record ConversationChannelStatusView(
    string RequestId,
    string? RequestStatus,
    string? JobStatus,
    string? CaseStatus,
    string? CaseResult,
    string? ExternalTicketReference);

public sealed record ConversationChannelError(
    string Code,
    string Message);
