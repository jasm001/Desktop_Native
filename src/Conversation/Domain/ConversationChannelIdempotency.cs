namespace ITSupportNative.Conversation.Domain;

public static class ConversationChannelIdempotency
{
    public static string Create(ConversationSession session)
    {
        ArgumentNullException.ThrowIfNull(session);
        PendingConversationRequest pendingRequest = session.PendingRequest
            ?? throw new InvalidOperationException(
                "A pending request is required.");

        return string.Join(
            ':',
            session.Id,
            pendingRequest.ProductReference,
            pendingRequest.ProductVersion,
            pendingRequest.Kind);
    }
}
