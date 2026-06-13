namespace ITSupportNative.DeviceAgent.Jobs;

public interface IAgentActionExecutor
{
    bool CanExecute(AuthorizedAgentAction action);

    Task<AgentActionExecutionResult> ExecuteAsync(
        AuthorizedAgentAction action,
        CancellationToken cancellationToken);
}
