using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Diagnostics;

public interface IMemoryDiagnosticCollector
{
    Task<MemoryDiagnosticResult> CollectAsync(CancellationToken cancellationToken);
}
