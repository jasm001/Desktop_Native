namespace ITSupportNative.Contracts.Agent;

public sealed record StartAgentJobRequest(
    string RequestId,
    string IdempotencyKey,
    string ActionId,
    string TargetId,
    string TargetVersion);
