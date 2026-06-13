namespace ITSupportNative.DeviceAgent.Jobs;

public sealed class DenyAgentActionExecutor : IAgentActionExecutor
{
    public bool CanExecute(AuthorizedAgentAction action)
    {
        ArgumentNullException.ThrowIfNull(action);
        return false;
    }

    public Task<AgentActionExecutionResult> ExecuteAsync(
        AuthorizedAgentAction action,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(action);
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(new AgentActionExecutionResult(
            Success: false,
            "job.execution.adapter_unavailable",
            "The installed policy did not resolve an execution adapter."));
    }
}
