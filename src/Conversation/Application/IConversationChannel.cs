using ITSupportNative.Contracts.Conversation;

namespace ITSupportNative.Conversation.Application;

public interface IConversationChannel
{
    ConversationChannelInput Read(ReadOnlySpan<byte> payload);

    byte[] Write(ConversationChannelOutput output);
}
