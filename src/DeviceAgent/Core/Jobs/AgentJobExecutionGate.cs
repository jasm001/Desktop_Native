namespace ITSupportNative.DeviceAgent.Jobs;

public sealed class AgentJobExecutionGate(bool isEnabled)
{
    public bool IsEnabled { get; } = isEnabled;
}
