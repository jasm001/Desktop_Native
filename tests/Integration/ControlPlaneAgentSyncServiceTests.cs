using ITSupportNative.Contracts.ControlPlane;
using ITSupportNative.DeviceAgent.ControlPlane;
using ITSupportNative.DeviceAgent.Jobs;

namespace ITSupportNative.IntegrationTests;

public sealed class ControlPlaneAgentSyncServiceTests : IDisposable
{
    private readonly string _testDirectory = Path.Combine(
        Path.GetTempPath(),
        "ITSupportNative.Tests",
        Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task ClaimsSimulatedJobAndReportsSanitizedEvidence()
    {
        var controlPlane = new RecordingControlPlaneAgentClient();
        using AgentJobService jobs = new(
            new SqliteAgentJobStore(Path.Combine(_testDirectory, "jobs.db")),
            new AgentActionAuthorizationPolicy(),
            TimeProvider.System);
        var synchronization = new ControlPlaneAgentSyncService(
            controlPlane,
            jobs);

        ControlPlaneSupportRequest? result =
            await synchronization.RunOnceAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("completed", result.Status);
        Assert.Equal("succeeded", controlPlane.Result);
        Assert.Equal(
            [
                "job.accepted",
                "job.simulation.started",
                "job.simulation.verified",
            ],
            controlPlane.Evidence.Select(item => item.Code));
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }

        GC.SuppressFinalize(this);
    }

    private sealed class RecordingControlPlaneAgentClient
        : IControlPlaneAgentClient
    {
        private readonly ClaimedAgentJob _job = new(
            Guid.NewGuid().ToString(),
            "request-control-plane-sync",
            "request-control-plane-sync:secure-transfer:6.5",
            "software.install.simulated.v1",
            "secure-transfer",
            "6.5",
            Guid.NewGuid().ToString(),
            DateTimeOffset.UtcNow.AddSeconds(30));

        public string? Result { get; private set; }

        public IReadOnlyList<ReportAgentEvidence> Evidence { get; private set; } =
            [];

        public Task<ClaimedAgentJob?> ClaimNextAsync(
            CancellationToken cancellationToken)
        {
            return Task.FromResult<ClaimedAgentJob?>(_job);
        }

        public Task<ReportAgentJobResultData> ReportResultAsync(
            ClaimedAgentJob job,
            string result,
            IReadOnlyList<ReportAgentEvidence> evidence,
            CancellationToken cancellationToken)
        {
            Result = result;
            Evidence = evidence;
            var request = new ControlPlaneSupportRequest(
                job.RequestId,
                $"REQ-{job.RequestId}",
                "control-plane-sync-correlation",
                result == "succeeded" ? "completed" : "failed",
                "local-device-001",
                job.TargetId,
                job.TargetVersion,
                job.ActionId,
                DateTimeOffset.UtcNow,
                new(job.JobId, "completed", []));
            return Task.FromResult(
                new ReportAgentJobResultData(request, Replayed: false));
        }
    }
}
