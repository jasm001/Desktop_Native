using System.Text;
using ITSupportNative.Contracts.Agent;
using ITSupportNative.DeviceAgent.Diagnostics;

namespace ITSupportNative.IntegrationTests;

public sealed class AgentDiagnosticsServiceTests
{
    [Fact]
    public async Task SnapshotHasDeterministicSectionsOrderUnitsAndPrerequisites()
    {
        var capturedAt = new DateTimeOffset(2026, 6, 12, 12, 0, 0, TimeSpan.Zero);
        AgentDiagnosticsService service = DiagnosticTestDoubles.CreateService(
            timeProvider: new FixedTimeProvider(capturedAt));

        AgentDiagnosticCommandResult result = await service.CollectAsync(
            DiagnosticTestDoubles.AuthorizedRequest(),
            CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(capturedAt, result.Snapshot?.CapturedAtUtc);
        Assert.Equal(8L * 1024 * 1024 * 1024, result.Snapshot?.Storage.AvailableBytes);
        Assert.Equal(4L * 1024 * 1024 * 1024, result.Snapshot?.Memory.AvailableBytes);
        Assert.Equal(
            [
                AgentPrerequisiteKind.WindowsOperatingSystem,
                AgentPrerequisiteKind.Architecture,
                AgentPrerequisiteKind.MinimumStorageAvailableBytes,
                AgentPrerequisiteKind.MinimumMemoryAvailableBytes,
                AgentPrerequisiteKind.NetworkAvailable,
            ],
            result.Snapshot?.ActionPrerequisites.Select(item => item.Kind));
        Assert.All(
            result.Snapshot!.ActionPrerequisites,
            prerequisite => Assert.Equal(
                AgentPrerequisiteEvaluation.Satisfied,
                prerequisite.Evaluation));
    }

    [Fact]
    public async Task CollectorFailureIsSanitizedWithoutAbortingSnapshot()
    {
        const string sensitiveMessage = "secret-domain.internal";
        var failingStorage = new StubStorageCollector(
            _ => throw new InvalidOperationException(sensitiveMessage));
        AgentDiagnosticsService service = DiagnosticTestDoubles.CreateService(
            storage: failingStorage);

        AgentDiagnosticCommandResult result = await service.CollectAsync(
            DiagnosticTestDoubles.AuthorizedRequest(),
            CancellationToken.None);
        byte[] serialized = AgentJson.Serialize(result.Snapshot);
        string json = Encoding.UTF8.GetString(serialized);

        Assert.True(result.Success);
        Assert.Equal(
            DiagnosticCollectionStatus.Unavailable,
            result.Snapshot?.Storage.Status);
        Assert.Equal("storage.unavailable", result.Snapshot?.Storage.Code);
        Assert.Equal(
            AgentPrerequisiteEvaluation.Unknown,
            result.Snapshot?.ActionPrerequisites.Single(item =>
                item.Kind == AgentPrerequisiteKind.MinimumStorageAvailableBytes).Evaluation);
        Assert.DoesNotContain(sensitiveMessage, json, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CallerCancellationStopsTheWholeSnapshot()
    {
        var waitingWindows = new StubWindowsCollector(
            async cancellationToken =>
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
                return DiagnosticTestDoubles.AvailableWindows();
            });
        AgentDiagnosticsService service = DiagnosticTestDoubles.CreateService(
            windows: waitingWindows);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => service.CollectAsync(
                DiagnosticTestDoubles.AuthorizedRequest(),
                cancellation.Token));
    }

    [Fact]
    public async Task UnknownOrOversizedActionReferenceIsRejectedBeforeCollection()
    {
        AgentDiagnosticsService service = DiagnosticTestDoubles.CreateService();

        AgentDiagnosticCommandResult unknown = await service.CollectAsync(
            DiagnosticTestDoubles.AuthorizedRequest() with { ActionId = "shell.execute" },
            CancellationToken.None);
        AgentDiagnosticCommandResult oversized = await service.CollectAsync(
            DiagnosticTestDoubles.AuthorizedRequest() with { TargetId = new string('a', 129) },
            CancellationToken.None);

        Assert.Equal(AgentErrorCode.UnauthorizedAction, unknown.Error?.Code);
        Assert.Equal(AgentErrorCode.InvalidMessage, oversized.Error?.Code);
    }
}
