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

    public int DomainProbeTimeoutMilliseconds { get; set; } = 2000;

    public string ExecutionProfile { get; set; } = "disabled";

    public bool JobExecutionEnabled { get; set; }

    public string LocalArtifactRootPath { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ITSupportNative",
        "DeviceAgent",
        "artifacts");

    public bool ControlPlaneSyncEnabled { get; set; }

    public string ControlPlaneBaseUrl { get; set; } = "http://127.0.0.1:3000";

    public string ControlPlaneDeviceId { get; set; } = "local-device-001";

    public string ControlPlaneAgentId { get; set; } = "local-agent-001";

    public int ControlPlanePollingIntervalMilliseconds { get; set; } = 1000;
}
