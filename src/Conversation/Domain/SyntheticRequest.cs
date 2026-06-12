namespace ITSupportNative.Conversation.Domain;

public sealed record SyntheticRequest(
    string Reference,
    string IdempotencyKey,
    string ProductReference,
    string ProductVersion,
    ConversationRequestKind Kind);
