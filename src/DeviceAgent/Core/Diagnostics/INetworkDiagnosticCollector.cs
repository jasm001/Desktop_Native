using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Diagnostics;

public interface INetworkDiagnosticCollector
{
    Task<NetworkDiagnosticResult> CollectAsync(CancellationToken cancellationToken);
}
