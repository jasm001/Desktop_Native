namespace ITSupportNative.Contracts.Agent;

public sealed record GetAgentDiagnosticsRequest(
    string ActionId,
    string TargetId,
    string TargetVersion);
