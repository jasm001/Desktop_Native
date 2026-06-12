using ITSupportNative.Contracts.Agent;
using ITSupportNative.DeviceAgent.Jobs;

namespace ITSupportNative.IntegrationTests;

public sealed class AgentJobServiceTests : IDisposable
{
    private readonly string _testDirectory = Path.Combine(
        Path.GetTempPath(),
        "ITSupportNative.Tests",
        Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task StartIsIdempotentAndConflictingReuseIsRejected()
    {
        AgentJobService service = CreateService();
        StartAgentJobRequest request = CreateAuthorizedRequest();

        AgentJobCommandResult first = await service.StartAsync(request, CancellationToken.None);
        AgentJobCommandResult duplicate = await service.StartAsync(request, CancellationToken.None);
        AgentJobCommandResult conflict = await service.StartAsync(
            request with { RequestId = "request-2" },
            CancellationToken.None);

        Assert.True(first.Success);
        Assert.True(duplicate.Success);
        Assert.True(duplicate.IsDuplicate);
        Assert.Equal(first.Job?.JobId, duplicate.Job?.JobId);
        Assert.Equal(AgentErrorCode.IdempotencyConflict, conflict.Error?.Code);
    }

    [Fact]
    public async Task UnknownActionIsDeniedBeforeJobCreation()
    {
        AgentJobService service = CreateService();

        AgentJobCommandResult result = await service.StartAsync(
            CreateAuthorizedRequest() with { ActionId = "shell.execute" },
            CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(AgentErrorCode.UnauthorizedAction, result.Error?.Code);
    }

    [Theory]
    [InlineData("software.install.simulated.v1", "unknown-product", "6.5")]
    [InlineData("software.install.simulated.v1", "secure-transfer", "9.9")]
    [InlineData("software.uninstall.simulated.v1", "secure-transfer", "6.5")]
    public async Task AuthorizationRequiresExactActionTargetAndVersion(
        string actionId,
        string targetId,
        string targetVersion)
    {
        AgentJobService service = CreateService();
        StartAgentJobRequest request = CreateAuthorizedRequest() with
        {
            ActionId = actionId,
            TargetId = targetId,
            TargetVersion = targetVersion,
        };

        AgentJobCommandResult result = await service.StartAsync(
            request,
            CancellationToken.None);

        Assert.Equal(AgentErrorCode.UnauthorizedAction, result.Error?.Code);
    }

    [Fact]
    public async Task JobProgressesAndProducesOnlySanitizedEvidence()
    {
        AgentJobService service = CreateService();
        AgentJobCommandResult started = await service.StartAsync(
            CreateAuthorizedRequest(),
            CancellationToken.None);

        for (int index = 0; index < 4; index++)
        {
            await service.AdvancePendingJobsAsync(CancellationToken.None);
        }

        AgentJobCommandResult completed = await service.GetAsync(
            started.Job!.JobId,
            CancellationToken.None);

        Assert.Equal(AgentJobState.Succeeded, completed.Job?.State);
        Assert.Equal(100, completed.Job?.ProgressPercent);
        Assert.Contains(
            completed.Job!.Evidence,
            evidence => evidence.Code == "job.simulation.verified");
        Assert.All(
            completed.Job.Evidence,
            evidence => Assert.DoesNotContain("request-1", evidence.Summary, StringComparison.Ordinal));
    }

    [Fact]
    public async Task QueuedJobCanBeCancelledWithoutDeviceChange()
    {
        AgentJobService service = CreateService();
        AgentJobCommandResult started = await service.StartAsync(
            CreateAuthorizedRequest(),
            CancellationToken.None);

        AgentJobCommandResult cancelled = await service.CancelAsync(
            started.Job!.JobId,
            CancellationToken.None);
        await service.AdvancePendingJobsAsync(CancellationToken.None);

        Assert.Equal(AgentJobState.Cancelled, cancelled.Job?.State);
        Assert.Equal(0, cancelled.Job?.ProgressPercent);
        Assert.Contains(
            cancelled.Job!.Evidence,
            evidence => evidence.Code == "job.cancelled");
    }

    [Fact]
    public async Task RunningJobIsRecoveredAfterServiceRestart()
    {
        AgentJobService firstService = CreateService();
        AgentJobCommandResult started = await firstService.StartAsync(
            CreateAuthorizedRequest(),
            CancellationToken.None);
        await firstService.AdvancePendingJobsAsync(CancellationToken.None);

        AgentJobService restartedService = CreateService();
        AgentJobCommandResult recovered = await restartedService.GetAsync(
            started.Job!.JobId,
            CancellationToken.None);

        Assert.Equal(AgentJobState.Queued, recovered.Job?.State);
        Assert.Equal(1, recovered.Job?.RecoveryCount);
        Assert.Contains(
            recovered.Job!.Evidence,
            evidence => evidence.Code == "job.recovered");

        for (int index = 0; index < 4; index++)
        {
            await restartedService.AdvancePendingJobsAsync(CancellationToken.None);
        }

        AgentJobCommandResult completed = await restartedService.GetAsync(
            started.Job.JobId,
            CancellationToken.None);
        Assert.Equal(AgentJobState.Succeeded, completed.Job?.State);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }

        GC.SuppressFinalize(this);
    }

    private AgentJobService CreateService()
    {
        string stateFile = Path.Combine(_testDirectory, "jobs.db");
        return new(
            new SqliteAgentJobStore(stateFile),
            new AgentActionAuthorizationPolicy(),
            TimeProvider.System);
    }

    private static StartAgentJobRequest CreateAuthorizedRequest()
    {
        return new(
            "request-1",
            "request-1:secure-transfer:6.5",
            "software.install.simulated.v1",
            "secure-transfer",
            "6.5");
    }
}
