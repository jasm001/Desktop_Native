namespace ITSupportNative.DeviceAgent.Execution;

public interface IProcessRunner
{
    Task<ProcessExecutionResult> RunAsync(
        ProcessExecutionRequest request,
        CancellationToken cancellationToken);
}
