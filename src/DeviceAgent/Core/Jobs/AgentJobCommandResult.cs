using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Jobs;

public sealed record AgentJobCommandResult(
    AgentJobSnapshot? Job,
    AgentError? Error,
    bool IsDuplicate)
{
    public bool Success => Job is not null && Error is null;

    public static AgentJobCommandResult Accepted(
        AgentJobSnapshot job,
        bool isDuplicate = false)
    {
        return new(job, Error: null, isDuplicate);
    }

    public static AgentJobCommandResult Rejected(
        AgentErrorCode code,
        string message)
    {
        return new(Job: null, new AgentError(code, message), IsDuplicate: false);
    }
}
