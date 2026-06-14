using ITSupportNative.Catalog.Application;
using ITSupportNative.Catalog.Domain;
using ITSupportNative.Contracts.ControlPlane;
using ITSupportNative.Contracts.Conversation;
using ITSupportNative.Conversation.Domain;

namespace ITSupportNative.Conversation.Application;

public sealed class ConversationChannelService(
    ConversationService conversationService,
    IConversationControlPlane controlPlane)
{
    public async Task<ConversationChannelTurn> HandleAsync(
        ConversationSession session,
        ConversationChannelInput input,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(input);

        if (!ConversationChannelContract.IsValid(input)
            || !string.Equals(session.Id, input.SessionId, StringComparison.Ordinal))
        {
            return new(
                session,
                CreateOutput(
                    session,
                    input,
                    "invalid_transition",
                    error: new(
                        "invalid_channel_context",
                        "The channel context is invalid.")));
        }

        return input.Action switch
        {
            ConversationChannelActions.RequestStatus =>
                await GetRequestStatusAsync(session, input, cancellationToken),
            ConversationChannelActions.CaseStatus =>
                await GetCaseStatusAsync(session, input, cancellationToken),
            _ => await HandleConversationAsync(session, input, cancellationToken),
        };
    }

    private async Task<ConversationChannelTurn> HandleConversationAsync(
        ConversationSession session,
        ConversationChannelInput input,
        CancellationToken cancellationToken)
    {
        ConversationIntent intent = input.Action switch
        {
            ConversationChannelActions.CatalogQuery =>
                ConversationIntent.QueryCatalog,
            ConversationChannelActions.SoftwareRequest =>
                ConversationIntent.RequestSoftware,
            ConversationChannelActions.ProposalContinue =>
                ConversationIntent.ContinueProposal,
            ConversationChannelActions.RequestConfirm =>
                ConversationIntent.Confirm,
            ConversationChannelActions.ConversationCancel =>
                ConversationIntent.Cancel,
            _ => throw new InvalidOperationException(
                "The channel action was not normalized."),
        };
        ConversationTurn turn = conversationService.Handle(
            session,
            new ConversationCommand(
                input.MessageId,
                intent,
                input.ProductReference));

        if (input.Action != ConversationChannelActions.RequestConfirm
            || turn.Session.Request is null
            || turn.Code is not (
                ConversationResultCode.RequestCreated
                or ConversationResultCode.DuplicateCommand))
        {
            return new(
                turn.Session,
                CreateOutput(
                    turn.Session,
                    input,
                    ToResultCode(turn.Code),
                    ToDecision(turn.CatalogDecision),
                    ToRequest(turn.Session.Request)));
        }

        SyntheticRequest request = turn.Session.Request;
        if (!string.Equals(
            input.IdempotencyKey,
            request.IdempotencyKey,
            StringComparison.Ordinal))
        {
            return new(
                turn.Session,
                CreateOutput(
                    turn.Session,
                    input,
                    "invalid_transition",
                    request: ToRequest(request),
                    error: new(
                        "idempotency_key_mismatch",
                        "The confirmation idempotency key is invalid.")));
        }

        if (request.Kind == ConversationRequestKind.HumanReview)
        {
            return new(
                turn.Session,
                CreateOutput(
                    turn.Session,
                    input,
                    "capability_unavailable",
                    request: ToRequest(request),
                    error: new(
                        "human_review_api_unavailable",
                        "Durable human review is not available in this increment.")));
        }

        try
        {
            CreateSoftwareInstallationData? created =
                await controlPlane.CreateSoftwareInstallationAsync(
                    input.CorrelationId,
                    input.IdempotencyKey!,
                    input.DeviceId,
                    request.ProductReference,
                    request.ProductVersion,
                    cancellationToken);

            return new(
                turn.Session,
                CreateOutput(
                    turn.Session,
                    input,
                    ToResultCode(turn.Code),
                    request: ToRequest(request, created)));
        }
        catch (HttpRequestException)
        {
            return ControlPlaneUnavailable(turn.Session, input, request);
        }
        catch (InvalidDataException)
        {
            return ControlPlaneUnavailable(turn.Session, input, request);
        }
    }

    private async Task<ConversationChannelTurn> GetRequestStatusAsync(
        ConversationSession session,
        ConversationChannelInput input,
        CancellationToken cancellationToken)
    {
        try
        {
            ControlPlaneSupportRequest? request =
                await controlPlane.GetSupportRequestAsync(
                    input.CorrelationId,
                    input.RequestId!,
                    cancellationToken);

            return request is null
                ? ControlPlaneUnavailable(session, input)
                : new(
                    session,
                    CreateOutput(
                        session,
                        input,
                        "request_status_returned",
                        status: new(
                            request.Id,
                            request.Status,
                            request.Job.Status,
                            CaseStatus: null,
                            CaseResult: null,
                            ExternalTicketReference: null)));
        }
        catch (HttpRequestException)
        {
            return ControlPlaneUnavailable(session, input);
        }
        catch (InvalidDataException)
        {
            return ControlPlaneUnavailable(session, input);
        }
    }

    private async Task<ConversationChannelTurn> GetCaseStatusAsync(
        ConversationSession session,
        ConversationChannelInput input,
        CancellationToken cancellationToken)
    {
        try
        {
            ControlPlaneBotCase? botCase =
                await controlPlane.GetBotCaseAsync(
                    input.CorrelationId,
                    input.RequestId!,
                    cancellationToken);

            return botCase is null
                ? ControlPlaneUnavailable(session, input)
                : new(
                    session,
                    CreateOutput(
                        session,
                        input,
                        "case_status_returned",
                        status: new(
                            botCase.RequestId,
                            RequestStatus: null,
                            JobStatus: null,
                            botCase.Status,
                            botCase.Result,
                            botCase.ExternalTicket?.Reference)));
        }
        catch (HttpRequestException)
        {
            return ControlPlaneUnavailable(session, input);
        }
        catch (InvalidDataException)
        {
            return ControlPlaneUnavailable(session, input);
        }
    }

    private static ConversationChannelTurn ControlPlaneUnavailable(
        ConversationSession session,
        ConversationChannelInput input,
        SyntheticRequest? request = null)
    {
        return new(
            session,
            CreateOutput(
                session,
                input,
                "control_plane_unavailable",
                request: ToRequest(request),
                error: new(
                    "control_plane_unavailable",
                    "The shared control plane is unavailable.")));
    }

    private static ConversationChannelOutput CreateOutput(
        ConversationSession session,
        ConversationChannelInput input,
        string resultCode,
        ConversationChannelDecision? decision = null,
        ConversationChannelRequestView? request = null,
        ConversationChannelStatusView? status = null,
        ConversationChannelError? error = null)
    {
        return new(
            ConversationChannelProtocol.Version,
            input.MessageId,
            input.CorrelationId,
            input.SessionId,
            ToState(session.State),
            resultCode,
            decision,
            request,
            status,
            error);
    }

    private static ConversationChannelDecision? ToDecision(
        CatalogDecision? decision)
    {
        return decision is null
            ? null
            : new(
                ToDecisionKind(decision.Kind),
                ToProductStatus(decision.EffectiveStatus),
                decision.Product?.Id,
                decision.Product?.Name,
                decision.Product?.Version.DisplayName,
                decision.Alternatives
                    .Select(
                        product => new ConversationChannelAlternative(
                            product.Id,
                            product.Name,
                            product.Version.DisplayName))
                    .ToArray());
    }

    private static ConversationChannelRequestView? ToRequest(
        SyntheticRequest? request,
        CreateSoftwareInstallationData? created = null)
    {
        return request is null
            ? null
            : new(
                request.Kind == ConversationRequestKind.SoftwareAcquisition
                    ? "software_acquisition"
                    : "human_review",
                request.ProductReference,
                request.ProductVersion,
                request.IdempotencyKey,
                request.Reference,
                created?.Request.Id,
                created?.Request.Reference,
                created?.Replayed);
    }

    private static string ToState(ConversationState state)
    {
        return state switch
        {
            ConversationState.Query => "query",
            ConversationState.Proposal => "proposal",
            ConversationState.ConfirmationRequired => "confirmation_required",
            ConversationState.RequestCreated => "request_created",
            ConversationState.Cancelled => "cancelled",
            _ => throw new InvalidOperationException(
                "The conversation state is not supported."),
        };
    }

    private static string ToResultCode(ConversationResultCode result)
    {
        return result switch
        {
            ConversationResultCode.QueryAnswered => "query_answered",
            ConversationResultCode.ProposalReady => "proposal_ready",
            ConversationResultCode.ConfirmationRequired =>
                "confirmation_required",
            ConversationResultCode.RequestCreated => "request_created",
            ConversationResultCode.Cancelled => "cancelled",
            ConversationResultCode.Rejected => "rejected",
            ConversationResultCode.InvalidTransition => "invalid_transition",
            ConversationResultCode.DuplicateCommand => "duplicate_command",
            _ => throw new InvalidOperationException(
                "The conversation result is not supported."),
        };
    }

    private static string ToDecisionKind(CatalogDecisionKind kind)
    {
        return kind switch
        {
            CatalogDecisionKind.Inform => "inform",
            CatalogDecisionKind.Propose => "propose",
            CatalogDecisionKind.Escalate => "escalate",
            CatalogDecisionKind.Reject => "reject",
            _ => throw new InvalidOperationException(
                "The catalog decision is not supported."),
        };
    }

    private static string ToProductStatus(SoftwareProductStatus status)
    {
        return status switch
        {
            SoftwareProductStatus.Approved => "approved",
            SoftwareProductStatus.Unlisted => "unlisted",
            SoftwareProductStatus.EndOfLife => "end_of_life",
            SoftwareProductStatus.Prohibited => "prohibited",
            _ => throw new InvalidOperationException(
                "The catalog status is not supported."),
        };
    }
}
