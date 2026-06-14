namespace ITSupportNative.Contracts.Conversation;

public static class ConversationChannelContract
{
    private const int MaxIdentifierLength = 100;
    private const int MaxActorLength = 128;
    private static readonly HashSet<string> AllowedStates = new(
        [
            "query",
            "proposal",
            "confirmation_required",
            "request_created",
            "cancelled",
        ],
        StringComparer.Ordinal);
    private static readonly HashSet<string> AllowedResults = new(
        [
            "query_answered",
            "proposal_ready",
            "confirmation_required",
            "request_created",
            "cancelled",
            "rejected",
            "invalid_transition",
            "duplicate_command",
            "request_status_returned",
            "case_status_returned",
            "capability_unavailable",
            "control_plane_unavailable",
        ],
        StringComparer.Ordinal);
    private static readonly HashSet<string> AllowedDecisionKinds =
        new(
            ["inform", "propose", "escalate", "reject"],
            StringComparer.Ordinal);
    private static readonly HashSet<string> AllowedProductStatuses =
        new(
            ["approved", "unlisted", "end_of_life", "prohibited"],
            StringComparer.Ordinal);
    private static readonly HashSet<string> AllowedRequestKinds =
        new(
            ["software_acquisition", "human_review"],
            StringComparer.Ordinal);
    private static readonly HashSet<string> AllowedRequestStatuses =
        new(
            ["confirmed", "completed", "failed"],
            StringComparer.Ordinal);
    private static readonly HashSet<string> AllowedJobStatuses =
        new(
            ["queued", "running", "completed", "failed"],
            StringComparer.Ordinal);
    private static readonly HashSet<string> AllowedCaseStatuses =
        new(
            ["open", "attended_waiting_user", "escalated"],
            StringComparer.Ordinal);
    private static readonly HashSet<string> AllowedCaseResults =
        new(
            ["pending", "succeeded", "failed"],
            StringComparer.Ordinal);

    public static bool IsValid(ConversationChannelInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (input.Version != ConversationChannelProtocol.Version
            || !IsBoundedIdentifier(input.MessageId, MaxIdentifierLength)
            || !IsBoundedIdentifier(input.CorrelationId, MaxIdentifierLength)
            || !IsBoundedIdentifier(input.SessionId, MaxIdentifierLength)
            || !IsBoundedIdentifier(input.ActorSubject, MaxActorLength)
            || !IsBoundedIdentifier(input.DeviceId, MaxActorLength)
            || !ConversationChannelActions.Allowed.Contains(input.Action))
        {
            return false;
        }

        return input.Action switch
        {
            ConversationChannelActions.CatalogQuery
                or ConversationChannelActions.SoftwareRequest =>
                IsBoundedIdentifier(input.ProductReference, MaxActorLength)
                && input.RequestId is null
                && input.IdempotencyKey is null,
            ConversationChannelActions.ProposalContinue
                or ConversationChannelActions.ConversationCancel =>
                input.ProductReference is null
                && input.RequestId is null
                && input.IdempotencyKey is null,
            ConversationChannelActions.RequestConfirm =>
                input.ProductReference is null
                && input.RequestId is null
                && IsBoundedIdentifier(input.IdempotencyKey, MaxIdentifierLength),
            ConversationChannelActions.RequestStatus
                or ConversationChannelActions.CaseStatus =>
                input.ProductReference is null
                && IsCanonicalUuid(input.RequestId)
                && input.IdempotencyKey is null,
            _ => false,
        };
    }

    public static bool IsValid(ConversationChannelOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        return output.Version == ConversationChannelProtocol.Version
            && IsBoundedIdentifier(output.MessageId, MaxIdentifierLength)
            && IsBoundedIdentifier(output.CorrelationId, MaxIdentifierLength)
            && IsBoundedIdentifier(output.SessionId, MaxIdentifierLength)
            && AllowedStates.Contains(output.State)
            && AllowedResults.Contains(output.ResultCode)
            && IsValid(output.Decision)
            && IsValid(output.Request)
            && IsValid(output.Status)
            && IsValid(output.Error);
    }

    private static bool IsValid(ConversationChannelDecision? decision)
    {
        if (decision is null)
        {
            return true;
        }

        return AllowedDecisionKinds.Contains(decision.Kind)
            && AllowedProductStatuses.Contains(decision.EffectiveStatus)
            && IsOptionalIdentifier(decision.ProductId, MaxActorLength)
            && IsOptionalText(decision.ProductName, 120)
            && IsOptionalIdentifier(decision.ProductVersion, MaxActorLength)
            && decision.Alternatives.Count <= 20
            && decision.Alternatives.All(
                alternative =>
                    IsBoundedIdentifier(
                        alternative.ProductId,
                        MaxActorLength)
                    && IsOptionalText(alternative.ProductName, 120)
                    && IsBoundedIdentifier(
                        alternative.ProductVersion,
                        MaxActorLength));
    }

    private static bool IsValid(ConversationChannelRequestView? request)
    {
        return request is null
            || AllowedRequestKinds.Contains(request.Kind)
            && IsBoundedIdentifier(request.ProductId, MaxActorLength)
            && IsBoundedIdentifier(request.ProductVersion, MaxActorLength)
            && IsBoundedIdentifier(
                request.IdempotencyKey,
                MaxIdentifierLength)
            && IsBoundedIdentifier(request.SyntheticReference, MaxActorLength)
            && (request.PersistedRequestId is null
                || IsCanonicalUuid(request.PersistedRequestId))
            && IsOptionalIdentifier(
                request.PersistedReference,
                MaxActorLength);
    }

    private static bool IsValid(ConversationChannelStatusView? status)
    {
        return status is null
            || IsCanonicalUuid(status.RequestId)
            && IsOptionalAllowed(status.RequestStatus, AllowedRequestStatuses)
            && IsOptionalAllowed(status.JobStatus, AllowedJobStatuses)
            && IsOptionalAllowed(status.CaseStatus, AllowedCaseStatuses)
            && IsOptionalAllowed(status.CaseResult, AllowedCaseResults)
            && IsOptionalIdentifier(
                status.ExternalTicketReference,
                MaxActorLength);
    }

    private static bool IsValid(ConversationChannelError? error)
    {
        return error is null
            || IsBoundedIdentifier(error.Code, MaxActorLength)
            && error.Message.Length is > 0 and <= 200;
    }

    private static bool IsCanonicalUuid(string? value)
    {
        return value is not null
            && Guid.TryParseExact(value, "D", out Guid parsed)
            && string.Equals(parsed.ToString("D"), value, StringComparison.Ordinal);
    }

    private static bool IsBoundedIdentifier(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)
            || value.Length > maxLength
            || !char.IsAsciiLetterOrDigit(value[0]))
        {
            return false;
        }

        return value.All(
            character => char.IsAsciiLetterOrDigit(character)
                || character is '.' or '_' or ':' or '-');
    }

    private static bool IsOptionalAllowed(
        string? value,
        HashSet<string> allowed)
    {
        return value is null || allowed.Contains(value);
    }

    private static bool IsOptionalIdentifier(string? value, int maxLength)
    {
        return value is null || IsBoundedIdentifier(value, maxLength);
    }

    private static bool IsOptionalText(string? value, int maxLength)
    {
        return value is null || value.Length is > 0 && value.Length <= maxLength;
    }
}
