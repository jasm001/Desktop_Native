namespace ITSupportNative.Contracts.Agent;

public static class AgentMessageTypes
{
    public const string StartJob = "agent.job.start";
    public const string GetJob = "agent.job.get";
    public const string CancelJob = "agent.job.cancel";
    public const string JobSnapshot = "agent.job.snapshot";
    public const string Error = "agent.error";
}
