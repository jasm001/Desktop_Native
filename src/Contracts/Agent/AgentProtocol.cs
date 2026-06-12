namespace ITSupportNative.Contracts.Agent;

public static class AgentProtocol
{
    public const int CurrentVersion = ContractVersion.Current;
    public const int MaximumMessageBytes = 64 * 1024;
    public const string DevelopmentPipeName = "ITSupportNative.DeviceAgent.dev.v1";
}
