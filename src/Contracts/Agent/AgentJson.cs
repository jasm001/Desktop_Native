using System.Text.Json;
using System.Text.Json.Serialization;

namespace ITSupportNative.Contracts.Agent;

public static class AgentJson
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
        },
    };

    public static AgentRequestEnvelope CreateRequest<TPayload>(
        string messageType,
        string correlationId,
        TPayload payload)
    {
        return new(
            AgentProtocol.CurrentVersion,
            messageType,
            correlationId,
            JsonSerializer.SerializeToElement(payload, SerializerOptions));
    }

    public static AgentResponseEnvelope CreateSuccess<TPayload>(
        string messageType,
        string correlationId,
        TPayload payload)
    {
        return new(
            AgentProtocol.CurrentVersion,
            messageType,
            correlationId,
            Success: true,
            JsonSerializer.SerializeToElement(payload, SerializerOptions),
            Error: null);
    }

    public static AgentResponseEnvelope CreateError(
        string correlationId,
        AgentErrorCode code,
        string message)
    {
        return new(
            AgentProtocol.CurrentVersion,
            AgentMessageTypes.Error,
            correlationId,
            Success: false,
            JsonSerializer.SerializeToElement(new { }, SerializerOptions),
            new AgentError(code, message));
    }

    public static byte[] Serialize<T>(T value)
    {
        return JsonSerializer.SerializeToUtf8Bytes(value, SerializerOptions);
    }

    public static T? Deserialize<T>(ReadOnlySpan<byte> utf8Json)
    {
        return JsonSerializer.Deserialize<T>(utf8Json, SerializerOptions);
    }

    public static TPayload? DeserializePayload<TPayload>(JsonElement payload)
    {
        return payload.Deserialize<TPayload>(SerializerOptions);
    }
}
