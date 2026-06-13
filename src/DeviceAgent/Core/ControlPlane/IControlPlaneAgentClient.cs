using ITSupportNative.Contracts.ControlPlane;

namespace ITSupportNative.DeviceAgent.ControlPlane;

public interface IControlPlaneAgentClient
{
    Task<ClaimedAgentJob?> ClaimNextAsync(CancellationToken cancellationToken);

    Task<ReportAgentJobResultData> ReportResultAsync(
        ClaimedAgentJob job,
        string result,
        IReadOnlyList<ReportAgentEvidence> evidence,
        CancellationToken cancellationToken);
}
