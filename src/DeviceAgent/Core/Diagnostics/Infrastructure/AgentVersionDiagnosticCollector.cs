using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Diagnostics.Infrastructure;

public sealed class AgentVersionDiagnosticCollector : IAgentVersionDiagnosticCollector
{
    public Task<AgentVersionDiagnosticResult> CollectAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? version = typeof(AgentDiagnosticsService).Assembly
            .GetName()
            .Version?
            .ToString();
        AgentVersionDiagnosticResult result = version is null
            ? new(
                DiagnosticCollectionStatus.Unavailable,
                "agent.version.unavailable",
                Version: null)
            : new(
                DiagnosticCollectionStatus.Available,
                "agent.version.available",
                version);
        return Task.FromResult(result);
    }
}
