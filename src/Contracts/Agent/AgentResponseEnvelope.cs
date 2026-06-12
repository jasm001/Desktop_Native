using System.Text.Json;

namespace ITSupportNative.Contracts.Agent;

public sealed record AgentResponseEnvelope(
    int Version,
    string MessageType,
    string CorrelationId,
    bool Success,
    JsonElement Payload,
    AgentError? Error);
