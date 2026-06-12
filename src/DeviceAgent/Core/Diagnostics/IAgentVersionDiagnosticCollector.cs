using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Diagnostics;

public interface IAgentVersionDiagnosticCollector
{
    Task<AgentVersionDiagnosticResult> CollectAsync(CancellationToken cancellationToken);
}
