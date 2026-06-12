namespace ITSupportNative.Conversation.Domain;

public enum ConversationState
{
    Query,
    Proposal,
    ConfirmationRequired,
    RequestCreated,
    Cancelled,
}
