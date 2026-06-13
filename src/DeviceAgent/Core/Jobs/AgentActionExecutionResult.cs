namespace ITSupportNative.DeviceAgent.Jobs;

public sealed record AgentActionExecutionResult(
    bool Success,
    string EvidenceCode,
    string Summary);
