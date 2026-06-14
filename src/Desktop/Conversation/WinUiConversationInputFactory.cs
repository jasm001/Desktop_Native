using ITSupportNative.Contracts.Conversation;
using ITSupportNative.Conversation.Domain;

namespace ITSupportNative.Desktop.Conversation;

public static class WinUiConversationInputFactory
{
    private const string ActorSubject = "development-user-001";
    private const string DeviceId = "local-device-001";

    public static ConversationChannelInput Create(
        ConversationSession session,
        string messageId,
        string correlationId,
        string action,
        string? productReference = null,
        string? requestId = null)
    {
        ArgumentNullException.ThrowIfNull(session);

        return new(
            ConversationChannelProtocol.Version,
            messageId,
            correlationId,
            session.Id,
            ActorSubject,
            DeviceId,
            action,
            productReference,
            requestId,
            action == ConversationChannelActions.RequestConfirm
                ? ConversationChannelIdempotency.Create(session)
                : null);
    }
}
