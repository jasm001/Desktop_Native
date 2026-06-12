using System.Runtime.InteropServices;
using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Diagnostics.Infrastructure;

public sealed class WindowsDiagnosticCollector : IWindowsDiagnosticCollector
{
    public Task<WindowsDiagnosticResult> CollectAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Version version = Environment.OSVersion.Version;
        var result = new WindowsDiagnosticResult(
            DiagnosticCollectionStatus.Available,
            "windows.available",
            new(
                version.Major,
                version.Minor,
                version.Build,
                version.Revision),
            RuntimeInformation.OSArchitecture.ToString().ToLowerInvariant());
        return Task.FromResult(result);
    }
}
