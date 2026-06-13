namespace ITSupportNative.DeviceAgent.Execution;

public enum ProcessExecutionResultCode
{
    Completed,
    StartFailed,
    TimedOut,
}

public sealed record ProcessExecutionResult(
    ProcessExecutionResultCode Code,
    int? ExitCode)
{
    public static ProcessExecutionResult Completed(int exitCode)
    {
        return new(ProcessExecutionResultCode.Completed, exitCode);
    }

    public static ProcessExecutionResult StartFailed()
    {
        return new(ProcessExecutionResultCode.StartFailed, ExitCode: null);
    }

    public static ProcessExecutionResult TimedOut()
    {
        return new(ProcessExecutionResultCode.TimedOut, ExitCode: null);
    }
}
