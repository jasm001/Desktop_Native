using ITSupportNative.Contracts.Agent;
using ITSupportNative.DeviceAgent.Execution;
using ITSupportNative.DeviceAgent.Jobs;

namespace ITSupportNative.IntegrationTests;

public sealed class AgentRealExecutionTests : IDisposable
{
    private readonly string _testDirectory = Path.Combine(
        Path.GetTempPath(),
        "ITSupportNative.RealJobTests",
        Guid.NewGuid().ToString("N"));

    [Theory]
    [InlineData("software.install.7zip.v1", "seven-zip", "26.01")]
    [InlineData("software.uninstall.7zip.v1", "seven-zip", "26.01")]
    public async Task ExactRealActionsAreAuthorized(
        string actionId,
        string targetId,
        string targetVersion)
    {
        AgentJobService service = CreateService(new ImmediateActionExecutor());

        AgentJobCommandResult result = await service.StartAsync(
            CreateRequest(actionId, targetId, targetVersion),
            CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(AgentJobState.Queued, result.Job?.State);
    }

    [Theory]
    [InlineData("software.install.7zip.v1", "seven-zip", "26.02")]
    [InlineData("software.install.7zip.v2", "seven-zip", "26.01")]
    [InlineData("software.install.7zip.v1", "seven-zip-x86", "26.01")]
    public async Task RealAuthorizationRemainsDenyByDefault(
        string actionId,
        string targetId,
        string targetVersion)
    {
        AgentJobService service = CreateService(new ImmediateActionExecutor());

        AgentJobCommandResult result = await service.StartAsync(
            CreateRequest(actionId, targetId, targetVersion),
            CancellationToken.None);

        Assert.Equal(AgentErrorCode.UnauthorizedAction, result.Error?.Code);
    }

    [Fact]
    public async Task RealActionsAreNotInstalledOutsideLocalDemoPolicy()
    {
        var service = new AgentJobService(
            new SqliteAgentJobStore(Path.Combine(_testDirectory, "disabled.db")),
            new AgentActionAuthorizationPolicy(),
            TimeProvider.System,
            new ImmediateActionExecutor());

        AgentJobCommandResult result = await service.StartAsync(
            CreateRequest(
                SevenZip2601X64Definition.InstallActionId,
                SevenZip2601X64Definition.TargetId,
                SevenZip2601X64Definition.TargetVersion),
            CancellationToken.None);

        Assert.Equal(AgentErrorCode.UnauthorizedAction, result.Error?.Code);
    }

    [Fact]
    public async Task RealJobUsesSelectedAdapterAndPersistsSanitizedResult()
    {
        var executor = new ImmediateActionExecutor();
        AgentJobService service = CreateService(executor);
        AgentJobCommandResult started = await service.StartAsync(
            CreateRequest(
                SevenZip2601X64Definition.InstallActionId,
                SevenZip2601X64Definition.TargetId,
                SevenZip2601X64Definition.TargetVersion),
            CancellationToken.None);

        await service.AdvancePendingJobsAsync(CancellationToken.None);
        AgentJobCommandResult completed = await service.GetAsync(
            started.Job!.JobId,
            CancellationToken.None);

        Assert.Equal(AgentJobState.Succeeded, completed.Job?.State);
        Assert.Equal(100, completed.Job?.ProgressPercent);
        AuthorizedAgentAction action = Assert.Single(executor.Actions);
        Assert.Equal(SevenZip2601X64Definition.AdapterId, action.AdapterId);
        Assert.Equal(AgentActionExecutionKind.SoftwareInstall, action.ExecutionKind);
        Assert.Contains(
            completed.Job!.Evidence,
            item => item.Code == "job.execution.verified");
        Assert.All(
            completed.Job.Evidence,
            item => Assert.DoesNotContain(
                SevenZip2601X64Definition.ProductCode,
                item.Summary,
                StringComparison.Ordinal));
    }

    [Fact]
    public async Task RunningMsiJobRejectsUnsafeCancellation()
    {
        var executor = new BlockingActionExecutor();
        AgentJobService service = CreateService(executor);
        AgentJobCommandResult started = await service.StartAsync(
            CreateRequest(
                SevenZip2601X64Definition.InstallActionId,
                SevenZip2601X64Definition.TargetId,
                SevenZip2601X64Definition.TargetVersion),
            CancellationToken.None);

        Task execution = service.AdvancePendingJobsAsync(CancellationToken.None);
        await executor.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));
        AgentJobCommandResult cancellation = await service.CancelAsync(
            started.Job!.JobId,
            CancellationToken.None);
        executor.Release.TrySetResult();
        await execution;

        Assert.Equal(AgentErrorCode.JobNotCancellable, cancellation.Error?.Code);
        AgentJobCommandResult completed = await service.GetAsync(
            started.Job.JobId,
            CancellationToken.None);
        Assert.Equal(AgentJobState.Succeeded, completed.Job?.State);
    }

    [Fact]
    public async Task QueuedRealJobCanBeCancelledBeforeExecution()
    {
        var executor = new ImmediateActionExecutor();
        AgentJobService service = CreateService(executor);
        AgentJobCommandResult started = await service.StartAsync(
            CreateRequest(
                SevenZip2601X64Definition.InstallActionId,
                SevenZip2601X64Definition.TargetId,
                SevenZip2601X64Definition.TargetVersion),
            CancellationToken.None);

        AgentJobCommandResult cancelled = await service.CancelAsync(
            started.Job!.JobId,
            CancellationToken.None);
        await service.AdvancePendingJobsAsync(CancellationToken.None);

        Assert.Equal(AgentJobState.Cancelled, cancelled.Job?.State);
        Assert.Empty(executor.Actions);
    }

    [Fact]
    public async Task MissingAdapterFailsClosed()
    {
        AgentJobService service = CreateService(new DenyAgentActionExecutor());
        AgentJobCommandResult started = await service.StartAsync(
            CreateRequest(
                SevenZip2601X64Definition.InstallActionId,
                SevenZip2601X64Definition.TargetId,
                SevenZip2601X64Definition.TargetVersion),
            CancellationToken.None);

        await service.AdvancePendingJobsAsync(CancellationToken.None);
        AgentJobCommandResult completed = await service.GetAsync(
            started.Job!.JobId,
            CancellationToken.None);

        Assert.Equal(AgentJobState.Failed, completed.Job?.State);
        Assert.Contains(
            completed.Job!.Evidence,
            item => item.Code == "job.execution.adapter_unavailable");
    }

    [Fact]
    public async Task SoftwareExecutorSelectsExactAdapterAndOperation()
    {
        var adapter = new RecordingSoftwareAdapter();
        var executor = new SoftwareAgentActionExecutor([adapter]);
        var action = new AuthorizedAgentAction(
            SevenZip2601X64Definition.UninstallActionId,
            SevenZip2601X64Definition.TargetId,
            SevenZip2601X64Definition.TargetVersion,
            [])
        {
            ExecutionKind = AgentActionExecutionKind.SoftwareUninstall,
            AdapterId = SevenZip2601X64Definition.AdapterId,
            SupportsRunningCancellation = false,
        };

        AgentActionExecutionResult result = await executor.ExecuteAsync(
            action,
            CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal(0, adapter.InstallCalls);
        Assert.Equal(1, adapter.UninstallCalls);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }

        GC.SuppressFinalize(this);
    }

    private AgentJobService CreateService(IAgentActionExecutor executor)
    {
        return new(
            new SqliteAgentJobStore(Path.Combine(_testDirectory, "jobs.db")),
            new AgentActionAuthorizationPolicy("local-demo"),
            TimeProvider.System,
            executor);
    }

    private static StartAgentJobRequest CreateRequest(
        string actionId,
        string targetId,
        string targetVersion)
    {
        return new(
            $"request-{Guid.NewGuid():N}",
            $"idempotency-{Guid.NewGuid():N}",
            actionId,
            targetId,
            targetVersion);
    }

    private sealed class ImmediateActionExecutor : IAgentActionExecutor
    {
        public List<AuthorizedAgentAction> Actions { get; } = [];

        public bool CanExecute(AuthorizedAgentAction action)
        {
            return true;
        }

        public Task<AgentActionExecutionResult> ExecuteAsync(
            AuthorizedAgentAction action,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Actions.Add(action);
            return Task.FromResult(new AgentActionExecutionResult(
                Success: true,
                "job.execution.verified",
                "The adapter completed and verified the requested state."));
        }
    }

    private sealed class BlockingActionExecutor : IAgentActionExecutor
    {
        public TaskCompletionSource Started { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource Release { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public bool CanExecute(AuthorizedAgentAction action)
        {
            return true;
        }

        public async Task<AgentActionExecutionResult> ExecuteAsync(
            AuthorizedAgentAction action,
            CancellationToken cancellationToken)
        {
            Started.TrySetResult();
            await Release.Task.WaitAsync(cancellationToken);
            return new(
                Success: true,
                "job.execution.verified",
                "The adapter completed and verified the requested state.");
        }
    }

    private sealed class RecordingSoftwareAdapter : ISoftwareExecutionAdapter
    {
        public string AdapterId => SevenZip2601X64Definition.AdapterId;

        public string TargetId => SevenZip2601X64Definition.TargetId;

        public string TargetVersion => SevenZip2601X64Definition.TargetVersion;

        public int InstallCalls { get; private set; }

        public int UninstallCalls { get; private set; }

        public Task<SoftwareExecutionResult> DetectAsync(
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new SoftwareExecutionResult(
                SoftwareExecutionResultCode.Detected));
        }

        public Task<SoftwareExecutionResult> PreflightAsync(
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new SoftwareExecutionResult(
                SoftwareExecutionResultCode.Ready));
        }

        public Task<SoftwareExecutionResult> InstallAsync(
            CancellationToken cancellationToken)
        {
            InstallCalls++;
            return Task.FromResult(new SoftwareExecutionResult(
                SoftwareExecutionResultCode.Succeeded));
        }

        public Task<SoftwareExecutionResult> VerifyAsync(
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new SoftwareExecutionResult(
                SoftwareExecutionResultCode.Detected));
        }

        public Task<SoftwareExecutionResult> UninstallAsync(
            CancellationToken cancellationToken)
        {
            UninstallCalls++;
            return Task.FromResult(new SoftwareExecutionResult(
                SoftwareExecutionResultCode.Succeeded));
        }
    }
}
