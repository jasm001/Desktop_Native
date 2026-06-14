using ITSupportNative.Contracts.Agent;
using ITSupportNative.Contracts.ControlPlane;
using ITSupportNative.DeviceAgent.Jobs;

namespace ITSupportNative.DeviceAgent.ControlPlane;

public sealed class ControlPlaneAgentSyncService(
    IControlPlaneAgentClient controlPlane,
    AgentJobService jobs,
    AgentJobExecutionGate executionGate)
{
    private static readonly HashSet<string> AllowedEvidenceCodes =
    [
        "job.accepted",
        "job.simulation.started",
        "job.simulation.verified",
        "job.simulation.failed",
    ];

    public async Task<ControlPlaneSupportRequest?> RunOnceAsync(
        CancellationToken cancellationToken)
    {
        if (!executionGate.IsEnabled)
        {
            return null;
        }

        ClaimedAgentJob? claimed =
            await controlPlane.ClaimNextAsync(cancellationToken);
        if (claimed is null)
        {
            return null;
        }

        AgentJobCommandResult started = await jobs.StartAsync(
            new StartAgentJobRequest(
                claimed.RequestId,
                claimed.IdempotencyKey,
                claimed.ActionId,
                claimed.TargetId,
                claimed.TargetVersion),
            cancellationToken);
        if (!started.Success || started.Job is null)
        {
            return await ReportFailureAsync(claimed, cancellationToken);
        }

        AgentJobSnapshot snapshot = started.Job;
        for (int attempt = 0; attempt < 8 && !IsTerminal(snapshot.State); attempt++)
        {
            await jobs.AdvancePendingJobsAsync(cancellationToken);
            AgentJobCommandResult current = await jobs.GetAsync(
                snapshot.JobId,
                cancellationToken);
            snapshot = current.Job
                ?? throw new InvalidOperationException(
                    "The simulated agent job disappeared.");
        }

        string result = snapshot.State == AgentJobState.Succeeded
            ? "succeeded"
            : "failed";
        ReportAgentEvidence[] evidence =
        [
            .. snapshot.Evidence
                .Where(item => AllowedEvidenceCodes.Contains(item.Code))
                .Select(item => new ReportAgentEvidence(
                    item.Code,
                    item.RecordedAt)),
        ];
        ReportAgentJobResultData reported = await controlPlane.ReportResultAsync(
            claimed,
            result,
            evidence,
            cancellationToken);
        return reported.Request;
    }

    private async Task<ControlPlaneSupportRequest> ReportFailureAsync(
        ClaimedAgentJob claimed,
        CancellationToken cancellationToken)
    {
        ReportAgentJobResultData reported = await controlPlane.ReportResultAsync(
            claimed,
            "failed",
            [
                new(
                    "job.simulation.failed",
                    DateTimeOffset.UtcNow),
            ],
            cancellationToken);
        return reported.Request;
    }

    private static bool IsTerminal(AgentJobState state)
    {
        return state is AgentJobState.Succeeded
            or AgentJobState.Failed
            or AgentJobState.Cancelled;
    }
}
