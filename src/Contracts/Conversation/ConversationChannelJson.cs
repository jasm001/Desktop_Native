using System.Text.Json;
using System.Text.Json.Serialization;

namespace ITSupportNative.Contracts.Conversation;

public static class ConversationChannelJson
{
    public static JsonSerializerOptions SerializerOptions { get; } =
        CreateSerializerOptions();

    public static ConversationChannelInput DeserializeInput(
        ReadOnlySpan<byte> utf8Json)
    {
        ConversationChannelInput? input =
            JsonSerializer.Deserialize<ConversationChannelInput>(
                utf8Json,
                SerializerOptions);

        return input is not null && ConversationChannelContract.IsValid(input)
            ? input
            : throw new InvalidDataException(
                "The conversation channel payload is invalid.");
    }

    public static byte[] SerializeOutput(ConversationChannelOutput output)
    {
        return ConversationChannelContract.IsValid(output)
            ? JsonSerializer.SerializeToUtf8Bytes(output, SerializerOptions)
            : throw new InvalidDataException(
                "The conversation channel response is invalid.");
    }

    private static JsonSerializerOptions CreateSerializerOptions()
    {
        return new(JsonSerializerDefaults.Web)
        {
            MaxDepth = 16,
            PropertyNameCaseInsensitive = false,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        };
    }
}
