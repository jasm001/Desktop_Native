using System.Text.Json;

namespace ITSupportNative.Contracts.Agent;

public sealed record AgentRequestEnvelope(
    int Version,
    string MessageType,
    string CorrelationId,
    JsonElement Payload);
