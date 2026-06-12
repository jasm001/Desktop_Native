using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Diagnostics;

public sealed record AgentDiagnosticCommandResult(
    AgentDiagnosticSnapshot? Snapshot,
    AgentError? Error)
{
    public bool Success => Snapshot is not null && Error is null;

    public static AgentDiagnosticCommandResult Available(AgentDiagnosticSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        return new(snapshot, Error: null);
    }

    public static AgentDiagnosticCommandResult Rejected(
        AgentErrorCode code,
        string message)
    {
        return new(Snapshot: null, new AgentError(code, message));
    }
}
