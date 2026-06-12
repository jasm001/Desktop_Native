namespace ITSupportNative.Contracts.Agent;

public sealed record AgentJobSnapshot(
    string JobId,
    string RequestId,
    string ActionId,
    string TargetId,
    string TargetVersion,
    AgentJobState State,
    int ProgressPercent,
    int RecoveryCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<AgentJobEvidence> Evidence);
