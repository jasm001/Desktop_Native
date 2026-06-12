namespace ITSupportNative.Conversation.Domain;

public sealed record PendingConversationRequest(
    string ProductReference,
    string ProductVersion,
    ConversationRequestKind Kind);
