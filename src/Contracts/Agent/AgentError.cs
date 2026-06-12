namespace ITSupportNative.Contracts.Agent;

public sealed record AgentError(
    AgentErrorCode Code,
    string Message);
