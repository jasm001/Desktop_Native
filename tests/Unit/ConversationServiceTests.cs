using ITSupportNative.Catalog.Application;
using ITSupportNative.Catalog.Fixtures;
using ITSupportNative.Conversation.Application;
using ITSupportNative.Conversation.Domain;

namespace ITSupportNative.UnitTests;

public sealed class ConversationServiceTests
{
    private readonly ConversationService _service = new(
        new CatalogDecisionService(SyntheticCatalog.Products));

    [Fact]
    public void CatalogQueryNeverCreatesARequest()
    {
        ConversationSession session = ConversationSession.Start("query-demo");

        ConversationTurn turn = _service.Handle(
            session,
            new ConversationCommand(
                "query-1",
                ConversationIntent.QueryCatalog,
                "aurora-code"));

        Assert.Equal(ConversationState.Query, turn.Session.State);
        Assert.Equal(ConversationResultCode.QueryAnswered, turn.Code);
        Assert.Equal(CatalogDecisionKind.Inform, turn.CatalogDecision?.Kind);
        Assert.Null(turn.Session.Request);
        Assert.Null(turn.Session.PendingRequest);
    }

    [Fact]
    public void ApprovedProductRequiresProposalAndExplicitConfirmation()
    {
        ConversationSession session = ConversationSession.Start("approved-demo");

        ConversationTurn proposal = Request(session, "request-1", "aurora-code");
        Assert.Equal(ConversationState.Proposal, proposal.Session.State);
        Assert.Null(proposal.Session.Request);

        ConversationTurn confirmation = _service.Handle(
            proposal.Session,
            new ConversationCommand("continue-1", ConversationIntent.ContinueProposal));
        Assert.Equal(ConversationState.ConfirmationRequired, confirmation.Session.State);
        Assert.Null(confirmation.Session.Request);

        ConversationTurn created = _service.Handle(
            confirmation.Session,
            new ConversationCommand("confirm-1", ConversationIntent.Confirm));
        Assert.Equal(ConversationState.RequestCreated, created.Session.State);
        Assert.Equal(
            ConversationRequestKind.SoftwareAcquisition,
            created.Session.Request?.Kind);
        Assert.Equal("aurora-code", created.Session.Request?.ProductReference);
    }

    [Theory]
    [InlineData("insight-studio")]
    [InlineData("diagram-lab")]
    [InlineData("unknown-product")]
    public void EscalationDecisionCreatesHumanReviewOnlyAfterConfirmation(
        string productReference)
    {
        ConversationSession session = ConversationSession.Start("review-demo");
        ConversationTurn proposal = Request(session, "request-1", productReference);

        Assert.Equal(CatalogDecisionKind.Escalate, proposal.CatalogDecision?.Kind);
        Assert.Equal(
            ConversationRequestKind.HumanReview,
            proposal.Session.PendingRequest?.Kind);

        ConversationTurn confirmation = Continue(proposal.Session, "continue-1");
        ConversationTurn created = Confirm(confirmation.Session, "confirm-1");

        Assert.Equal(ConversationState.RequestCreated, created.Session.State);
        Assert.Equal(ConversationRequestKind.HumanReview, created.Session.Request?.Kind);
    }

    [Theory]
    [InlineData("legacy-transfer")]
    [InlineData("share-anywhere")]
    public void RejectedProductDoesNotEnterConfirmationFlow(string productReference)
    {
        ConversationTurn turn = Request(
            ConversationSession.Start("rejected-demo"),
            "request-1",
            productReference);

        Assert.Equal(ConversationResultCode.Rejected, turn.Code);
        Assert.Equal(ConversationState.Query, turn.Session.State);
        Assert.Null(turn.Session.PendingRequest);
        Assert.Null(turn.Session.Request);
    }

    [Fact]
    public void UserCanCancelBeforeConfirmation()
    {
        ConversationTurn proposal = Request(
            ConversationSession.Start("cancel-demo"),
            "request-1",
            "aurora-code");

        ConversationTurn cancelled = _service.Handle(
            proposal.Session,
            new ConversationCommand("cancel-1", ConversationIntent.Cancel));

        Assert.Equal(ConversationState.Cancelled, cancelled.Session.State);
        Assert.Null(cancelled.Session.Request);
    }

    [Fact]
    public void RetryingSameConfirmationCommandDoesNotDuplicateRequest()
    {
        ConversationTurn proposal = Request(
            ConversationSession.Start("retry-demo"),
            "request-1",
            "aurora-code");
        ConversationTurn confirmation = Continue(proposal.Session, "continue-1");
        ConversationCommand command = new("confirm-1", ConversationIntent.Confirm);
        ConversationTurn created = _service.Handle(confirmation.Session, command);

        ConversationTurn retry = _service.Handle(created.Session, command);

        Assert.True(retry.IsDuplicate);
        Assert.Equal(ConversationResultCode.DuplicateCommand, retry.Code);
        Assert.Same(created.Session, retry.Session);
        Assert.Same(created.Session.Request, retry.Session.Request);
    }

    [Fact]
    public void RepeatingConfirmationWithNewCommandIdStillDoesNotDuplicateRequest()
    {
        ConversationTurn proposal = Request(
            ConversationSession.Start("repeat-demo"),
            "request-1",
            "aurora-code");
        ConversationTurn confirmation = Continue(proposal.Session, "continue-1");
        ConversationTurn created = Confirm(confirmation.Session, "confirm-1");

        ConversationTurn repeated = Confirm(created.Session, "confirm-2");

        Assert.Equal(ConversationResultCode.InvalidTransition, repeated.Code);
        Assert.False(repeated.TransitionApplied);
        Assert.Equal(created.Session.Request, repeated.Session.Request);
        Assert.Equal(ConversationState.RequestCreated, repeated.Session.State);
    }

    [Fact]
    public void ConfirmationBeforeProposalIsRejected()
    {
        ConversationTurn turn = Confirm(
            ConversationSession.Start("invalid-demo"),
            "confirm-1");

        Assert.Equal(ConversationResultCode.InvalidTransition, turn.Code);
        Assert.Equal(ConversationState.Query, turn.Session.State);
        Assert.Null(turn.Session.Request);
    }

    [Fact]
    public void NewRequestCannotReplacePendingConfirmation()
    {
        ConversationTurn proposal = Request(
            ConversationSession.Start("pending-demo"),
            "request-1",
            "aurora-code");
        ConversationTurn confirmation = Continue(proposal.Session, "continue-1");

        ConversationTurn replacement = Request(
            confirmation.Session,
            "request-2",
            "secure-transfer");

        Assert.Equal(ConversationResultCode.InvalidTransition, replacement.Code);
        Assert.Equal(
            "aurora-code",
            replacement.Session.PendingRequest?.ProductReference);
        Assert.Equal(ConversationState.ConfirmationRequired, replacement.Session.State);
    }

    [Fact]
    public void SyntheticRequestUsesStableIdempotencyKey()
    {
        ConversationTurn first = CompleteApprovedRequest("stable-demo");
        ConversationTurn second = CompleteApprovedRequest("stable-demo");

        Assert.Equal(
            first.Session.Request?.IdempotencyKey,
            second.Session.Request?.IdempotencyKey);
        Assert.Equal(
            first.Session.Request?.Reference,
            second.Session.Request?.Reference);
    }

    private ConversationTurn CompleteApprovedRequest(string conversationId)
    {
        ConversationTurn proposal = Request(
            ConversationSession.Start(conversationId),
            "request-1",
            "aurora-code");
        ConversationTurn confirmation = Continue(proposal.Session, "continue-1");
        return Confirm(confirmation.Session, "confirm-1");
    }

    private ConversationTurn Request(
        ConversationSession session,
        string commandId,
        string productReference)
    {
        return _service.Handle(
            session,
            new ConversationCommand(
                commandId,
                ConversationIntent.RequestSoftware,
                productReference));
    }

    private ConversationTurn Continue(ConversationSession session, string commandId)
    {
        return _service.Handle(
            session,
            new ConversationCommand(commandId, ConversationIntent.ContinueProposal));
    }

    private ConversationTurn Confirm(ConversationSession session, string commandId)
    {
        return _service.Handle(
            session,
            new ConversationCommand(commandId, ConversationIntent.Confirm));
    }
}
