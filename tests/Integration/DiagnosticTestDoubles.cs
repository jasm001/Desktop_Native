using ITSupportNative.Contracts.Agent;
using ITSupportNative.DeviceAgent.Diagnostics;
using ITSupportNative.DeviceAgent.Jobs;

namespace ITSupportNative.IntegrationTests;

internal static class DiagnosticTestDoubles
{
    public static AgentDiagnosticsService CreateService(
        IWindowsDiagnosticCollector? windows = null,
        IStorageDiagnosticCollector? storage = null,
        IMemoryDiagnosticCollector? memory = null,
        INetworkDiagnosticCollector? network = null,
        IAgentVersionDiagnosticCollector? agent = null,
        TimeProvider? timeProvider = null)
    {
        return new(
            windows ?? new StubWindowsCollector(_ => Task.FromResult(AvailableWindows())),
            storage ?? new StubStorageCollector(_ => Task.FromResult(AvailableStorage())),
            memory ?? new StubMemoryCollector(_ => Task.FromResult(AvailableMemory())),
            network ?? new StubNetworkCollector(_ => Task.FromResult(AvailableNetwork())),
            agent ?? new StubAgentVersionCollector(_ => Task.FromResult(AvailableAgentVersion())),
            new AgentActionAuthorizationPolicy(),
            timeProvider ?? TimeProvider.System);
    }

    public static GetAgentDiagnosticsRequest AuthorizedRequest()
    {
        return new(
            "software.install.simulated.v1",
            "secure-transfer",
            "6.5");
    }

    public static WindowsDiagnosticResult AvailableWindows()
    {
        return new(
            DiagnosticCollectionStatus.Available,
            "windows.available",
            new(10, 0, 26100, 0),
            "x64");
    }

    public static StorageDiagnosticResult AvailableStorage()
    {
        return new(
            DiagnosticCollectionStatus.Available,
            "storage.available",
            CapacityBytes: 16L * 1024 * 1024 * 1024,
            AvailableBytes: 8L * 1024 * 1024 * 1024,
            FixedVolumeCount: 1);
    }

    public static MemoryDiagnosticResult AvailableMemory()
    {
        return new(
            DiagnosticCollectionStatus.Available,
            "memory.available",
            TotalBytes: 8L * 1024 * 1024 * 1024,
            AvailableBytes: 4L * 1024 * 1024 * 1024);
    }

    public static NetworkDiagnosticResult AvailableNetwork()
    {
        return new(
            DiagnosticCollectionStatus.Available,
            "network.domain_reachable",
            NetworkAvailable: true,
            DomainReachabilityStatus.Reachable);
    }

    public static AgentVersionDiagnosticResult AvailableAgentVersion()
    {
        return new(
            DiagnosticCollectionStatus.Available,
            "agent.version.available",
            "1.0.0.0");
    }
}

internal sealed class StubWindowsCollector(
    Func<CancellationToken, Task<WindowsDiagnosticResult>> collect)
    : IWindowsDiagnosticCollector
{
    public Task<WindowsDiagnosticResult> CollectAsync(CancellationToken cancellationToken)
    {
        return collect(cancellationToken);
    }
}

internal sealed class StubStorageCollector(
    Func<CancellationToken, Task<StorageDiagnosticResult>> collect)
    : IStorageDiagnosticCollector
{
    public Task<StorageDiagnosticResult> CollectAsync(CancellationToken cancellationToken)
    {
        return collect(cancellationToken);
    }
}

internal sealed class StubMemoryCollector(
    Func<CancellationToken, Task<MemoryDiagnosticResult>> collect)
    : IMemoryDiagnosticCollector
{
    public Task<MemoryDiagnosticResult> CollectAsync(CancellationToken cancellationToken)
    {
        return collect(cancellationToken);
    }
}

internal sealed class StubNetworkCollector(
    Func<CancellationToken, Task<NetworkDiagnosticResult>> collect)
    : INetworkDiagnosticCollector
{
    public Task<NetworkDiagnosticResult> CollectAsync(CancellationToken cancellationToken)
    {
        return collect(cancellationToken);
    }
}

internal sealed class StubAgentVersionCollector(
    Func<CancellationToken, Task<AgentVersionDiagnosticResult>> collect)
    : IAgentVersionDiagnosticCollector
{
    public Task<AgentVersionDiagnosticResult> CollectAsync(CancellationToken cancellationToken)
    {
        return collect(cancellationToken);
    }
}

internal sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    public override DateTimeOffset GetUtcNow()
    {
        return utcNow;
    }
}
