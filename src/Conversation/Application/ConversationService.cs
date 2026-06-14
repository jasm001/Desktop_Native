using ITSupportNative.Catalog.Application;
using ITSupportNative.Conversation.Domain;

namespace ITSupportNative.Conversation.Application;

public sealed class ConversationService(CatalogDecisionService catalogDecisions)
{
    public ConversationTurn Handle(
        ConversationSession session,
        ConversationCommand command)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(command);

        if (session.HasProcessed(command.Id))
        {
            return new(
                session,
                ConversationResultCode.DuplicateCommand,
                CatalogDecision: null,
                TransitionApplied: false,
                IsDuplicate: true);
        }

        return command.Intent switch
        {
            ConversationIntent.QueryCatalog => HandleCatalogQuery(session, command),
            ConversationIntent.RequestSoftware => HandleSoftwareRequest(session, command),
            ConversationIntent.ContinueProposal => HandleProposalContinuation(session, command),
            ConversationIntent.Confirm => HandleConfirmation(session, command),
            ConversationIntent.Cancel => HandleCancellation(session, command),
            _ => CreateInvalidTransition(session, command),
        };
    }

    private ConversationTurn HandleCatalogQuery(
        ConversationSession session,
        ConversationCommand command)
    {
        if (session.State != ConversationState.Query || command.ProductReference is null)
        {
            return CreateInvalidTransition(session, command);
        }

        CatalogDecision decision = catalogDecisions.Evaluate(
            command.ProductReference,
            CatalogDecisionPurpose.Information);
        ConversationSession next = session.Advance(
            command.Id,
            ConversationState.Query,
            pendingRequest: null,
            request: null);

        return new(
            next,
            ConversationResultCode.QueryAnswered,
            decision,
            TransitionApplied: true,
            IsDuplicate: false);
    }

    private ConversationTurn HandleSoftwareRequest(
        ConversationSession session,
        ConversationCommand command)
    {
        if (session.State != ConversationState.Query || command.ProductReference is null)
        {
            return CreateInvalidTransition(session, command);
        }

        CatalogDecision decision = catalogDecisions.Evaluate(
            command.ProductReference,
            CatalogDecisionPurpose.Acquisition);

        if (decision.Kind == CatalogDecisionKind.Reject)
        {
            ConversationSession rejected = session.Advance(
                command.Id,
                ConversationState.Query,
                pendingRequest: null,
                request: null);

            return new(
                rejected,
                ConversationResultCode.Rejected,
                decision,
                TransitionApplied: true,
                IsDuplicate: false);
        }

        if (decision.Kind is not (CatalogDecisionKind.Propose or CatalogDecisionKind.Escalate))
        {
            return CreateInvalidTransition(session, command, decision);
        }

        string productReference = decision.Product?.Id ?? command.ProductReference;
        string productVersion = decision.Product?.Version.DisplayName ?? "unknown";
        ConversationRequestKind requestKind = decision.Kind == CatalogDecisionKind.Propose
            ? ConversationRequestKind.SoftwareAcquisition
            : ConversationRequestKind.HumanReview;
        var pendingRequest = new PendingConversationRequest(
            productReference,
            productVersion,
            requestKind);
        ConversationSession next = session.Advance(
            command.Id,
            ConversationState.Proposal,
            pendingRequest,
            request: null);

        return new(
            next,
            ConversationResultCode.ProposalReady,
            decision,
            TransitionApplied: true,
            IsDuplicate: false);
    }

    private static ConversationTurn HandleProposalContinuation(
        ConversationSession session,
        ConversationCommand command)
    {
        if (session.State != ConversationState.Proposal || session.PendingRequest is null)
        {
            return CreateInvalidTransition(session, command);
        }

        ConversationSession next = session.Advance(
            command.Id,
            ConversationState.ConfirmationRequired,
            session.PendingRequest,
            request: null);

        return new(
            next,
            ConversationResultCode.ConfirmationRequired,
            CatalogDecision: null,
            TransitionApplied: true,
            IsDuplicate: false);
    }

    private static ConversationTurn HandleConfirmation(
        ConversationSession session,
        ConversationCommand command)
    {
        if (session.State != ConversationState.ConfirmationRequired
            || session.PendingRequest is null)
        {
            return CreateInvalidTransition(session, command);
        }

        SyntheticRequest request = CreateSyntheticRequest(session);
        ConversationSession next = session.Advance(
            command.Id,
            ConversationState.RequestCreated,
            session.PendingRequest,
            request);

        return new(
            next,
            ConversationResultCode.RequestCreated,
            CatalogDecision: null,
            TransitionApplied: true,
            IsDuplicate: false);
    }

    private static ConversationTurn HandleCancellation(
        ConversationSession session,
        ConversationCommand command)
    {
        if (session.State is not (ConversationState.Proposal or ConversationState.ConfirmationRequired))
        {
            return CreateInvalidTransition(session, command);
        }

        ConversationSession next = session.Advance(
            command.Id,
            ConversationState.Cancelled,
            session.PendingRequest,
            request: null);

        return new(
            next,
            ConversationResultCode.Cancelled,
            CatalogDecision: null,
            TransitionApplied: true,
            IsDuplicate: false);
    }

    private static ConversationTurn CreateInvalidTransition(
        ConversationSession session,
        ConversationCommand command,
        CatalogDecision? decision = null)
    {
        ConversationSession next = session.Advance(
            command.Id,
            session.State,
            session.PendingRequest,
            session.Request);

        return new(
            next,
            ConversationResultCode.InvalidTransition,
            decision,
            TransitionApplied: false,
            IsDuplicate: false);
    }

    private static SyntheticRequest CreateSyntheticRequest(ConversationSession session)
    {
        PendingConversationRequest pendingRequest = session.PendingRequest
            ?? throw new InvalidOperationException("A pending request is required.");
        string idempotencyKey = ConversationChannelIdempotency.Create(session);
        string reference = $"SYN-{session.Id}-{pendingRequest.ProductReference}";

        return new(
            reference,
            idempotencyKey,
            pendingRequest.ProductReference,
            pendingRequest.ProductVersion,
            pendingRequest.Kind);
    }

}
