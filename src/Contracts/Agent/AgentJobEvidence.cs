namespace ITSupportNative.Contracts.Agent;

public sealed record AgentJobEvidence(
    string Code,
    string Summary,
    DateTimeOffset RecordedAt);
