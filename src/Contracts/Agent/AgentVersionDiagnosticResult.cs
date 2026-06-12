namespace ITSupportNative.Contracts.Agent;

public sealed record AgentVersionDiagnosticResult(
    DiagnosticCollectionStatus Status,
    string Code,
    string? Version);
