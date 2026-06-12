using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Jobs;

public sealed record AgentJobRecord(
    string JobId,
    string RequestId,
    string IdempotencyKey,
    string ActionId,
    string TargetId,
    string TargetVersion,
    AgentJobState State,
    int ProgressPercent,
    int RecoveryCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<AgentJobEvidence> Evidence)
{
    public AgentJobSnapshot ToSnapshot()
    {
        return new(
            JobId,
            RequestId,
            ActionId,
            TargetId,
            TargetVersion,
            State,
            ProgressPercent,
            RecoveryCount,
            CreatedAt,
            UpdatedAt,
            Evidence);
    }
}
