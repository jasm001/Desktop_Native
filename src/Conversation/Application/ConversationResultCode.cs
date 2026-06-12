namespace ITSupportNative.Conversation.Application;

public enum ConversationResultCode
{
    QueryAnswered,
    ProposalReady,
    ConfirmationRequired,
    RequestCreated,
    Cancelled,
    Rejected,
    InvalidTransition,
    DuplicateCommand,
}
