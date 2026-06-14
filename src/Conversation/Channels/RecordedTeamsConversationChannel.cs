using System.Collections.ObjectModel;
using System.Text.Json;
using ITSupportNative.Contracts.Conversation;
using ITSupportNative.Conversation.Application;

namespace ITSupportNative.Conversation.Channels;

public sealed class RecordedTeamsConversationChannel : IConversationChannel
{
    private readonly List<ConversationChannelInput> _inputs = [];
    private readonly List<ConversationChannelOutput> _outputs = [];

    public IReadOnlyList<ConversationChannelInput> Inputs =>
        new ReadOnlyCollection<ConversationChannelInput>(_inputs);

    public IReadOnlyList<ConversationChannelOutput> Outputs =>
        new ReadOnlyCollection<ConversationChannelOutput>(_outputs);

    public ConversationChannelInput Read(ReadOnlySpan<byte> payload)
    {
        try
        {
            ConversationChannelInput input =
                ConversationChannelJson.DeserializeInput(payload);
            _inputs.Add(input);
            return input;
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException(
                "The conversation channel payload is invalid.",
                exception);
        }
    }

    public byte[] Write(ConversationChannelOutput output)
    {
        byte[] payload = ConversationChannelJson.SerializeOutput(output);
        _outputs.Add(output);
        return payload;
    }
}
