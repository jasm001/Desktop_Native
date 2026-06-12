using ITSupportNative.Contracts.Agent;
using ITSupportNative.DeviceAgent.Diagnostics.Infrastructure;

namespace ITSupportNative.IntegrationTests;

public sealed class DiagnosticInfrastructureTests
{
    private static readonly string[] SupportedArchitectures = ["x64", "x86", "arm64", "arm"];

    [Fact]
    public async Task LocalCollectorsReturnBoundedAggregateData()
    {
        var windowsCollector = new WindowsDiagnosticCollector();
        var storageCollector = new StorageDiagnosticCollector();
        var memoryCollector = new MemoryDiagnosticCollector();
        var versionCollector = new AgentVersionDiagnosticCollector();

        WindowsDiagnosticResult windows =
            await windowsCollector.CollectAsync(CancellationToken.None);
        StorageDiagnosticResult storage =
            await storageCollector.CollectAsync(CancellationToken.None);
        MemoryDiagnosticResult memory =
            await memoryCollector.CollectAsync(CancellationToken.None);
        AgentVersionDiagnosticResult version =
            await versionCollector.CollectAsync(CancellationToken.None);

        Assert.Equal(DiagnosticCollectionStatus.Available, windows.Status);
        Assert.NotNull(windows.Version);
        Assert.Contains(windows.Architecture, SupportedArchitectures);
        Assert.Equal(DiagnosticCollectionStatus.Available, storage.Status);
        Assert.True(storage.CapacityBytes > 0);
        Assert.InRange(storage.FixedVolumeCount ?? 0, 1, 32);
        Assert.Equal(DiagnosticCollectionStatus.Available, memory.Status);
        Assert.True(memory.TotalBytes > 0);
        Assert.True(memory.AvailableBytes >= 0);
        Assert.Equal(DiagnosticCollectionStatus.Available, version.Status);
        Assert.Matches(@"^\d+\.\d+\.\d+\.\d+$", version.Version);
    }

    [Fact]
    public async Task DomainTimeoutProducesExplicitTypedState()
    {
        var collector = new NetworkDiagnosticCollector(
            new StubDomainProbe(DomainReachabilityStatus.TimedOut));

        NetworkDiagnosticResult result =
            await collector.CollectAsync(CancellationToken.None);

        if (result.NetworkAvailable is false)
        {
            Assert.Equal("network.offline", result.Code);
            return;
        }

        Assert.Equal(DiagnosticCollectionStatus.TimedOut, result.Status);
        Assert.Equal(DomainReachabilityStatus.TimedOut, result.DomainReachability);
        Assert.Equal("network.domain_timeout", result.Code);
    }

    private sealed class StubDomainProbe(DomainReachabilityStatus result)
        : IDomainReachabilityProbe
    {
        public Task<DomainReachabilityStatus> ProbeAsync(
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(result);
        }
    }
}
