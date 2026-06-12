using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Diagnostics;

public interface IStorageDiagnosticCollector
{
    Task<StorageDiagnosticResult> CollectAsync(CancellationToken cancellationToken);
}
