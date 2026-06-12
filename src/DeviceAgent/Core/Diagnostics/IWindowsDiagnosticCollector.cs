using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Diagnostics;

public interface IWindowsDiagnosticCollector
{
    Task<WindowsDiagnosticResult> CollectAsync(CancellationToken cancellationToken);
}
