using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Configuration;

public sealed class DeviceAgentOptions
{
    public const string SectionName = "DeviceAgent";

    public string PipeName { get; set; } = AgentProtocol.DevelopmentPipeName;

    public string StateFilePath { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ITSupportNative",
        "DeviceAgent",
        "jobs.db");

    public int SimulationIntervalMilliseconds { get; set; } = 250;
}
