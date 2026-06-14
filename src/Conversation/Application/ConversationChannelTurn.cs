using ITSupportNative.Contracts.Conversation;
using ITSupportNative.Conversation.Domain;

namespace ITSupportNative.Conversation.Application;

public sealed record ConversationChannelTurn(
    ConversationSession Session,
    ConversationChannelOutput Output);
