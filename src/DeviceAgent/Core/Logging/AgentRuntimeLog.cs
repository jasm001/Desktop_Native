using Microsoft.Extensions.Logging;

namespace ITSupportNative.DeviceAgent.Logging;

public static class AgentRuntimeLog
{
    private static readonly Action<ILogger, Exception?> LogIpcRequestFailure =
        LoggerMessage.Define(
            LogLevel.Warning,
            new EventId(1001, nameof(IpcRequestFailure)),
            "A local IPC request failed before an authorized action could run.");

    private static readonly Action<ILogger, Exception?> LogJobLoopFailure =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(2001, nameof(JobLoopFailure)),
            "The job loop failed without exposing job payloads.");

    private static readonly Action<ILogger, Exception?> LogSynchronizationFailure =
        LoggerMessage.Define(
            LogLevel.Warning,
            new EventId(3001, nameof(SynchronizationFailure)),
            "The local control plane synchronization attempt failed.");

    public static void IpcRequestFailure(ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        LogIpcRequestFailure(logger, null);
    }

    public static void JobLoopFailure(ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        LogJobLoopFailure(logger, null);
    }

    public static void SynchronizationFailure(ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        LogSynchronizationFailure(logger, null);
    }
}
