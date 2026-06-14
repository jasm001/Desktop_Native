using ITSupportNative.DeviceAgent.Logging;
using Microsoft.Extensions.Logging;

namespace ITSupportNative.IntegrationTests;

public sealed class AgentRuntimeLogTests
{
    [Fact]
    public void RuntimeFailuresUseFixedEventsWithoutExceptionDetails()
    {
        var logger = new RecordingLogger();

        AgentRuntimeLog.IpcRequestFailure(logger);
        AgentRuntimeLog.JobLoopFailure(logger);
        AgentRuntimeLog.SynchronizationFailure(logger);

        Assert.Equal([1001, 2001, 3001], logger.Entries.Select(item => item.EventId));
        Assert.All(logger.Entries, item => Assert.Null(item.Exception));
        Assert.Equal(
            [
                "A local IPC request failed before an authorized action could run.",
                "The job loop failed without exposing job payloads.",
                "The local control plane synchronization attempt failed.",
            ],
            logger.Entries.Select(item => item.Message));
    }

    private sealed class RecordingLogger : ILogger
    {
        public List<LogEntry> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Entries.Add(new(eventId.Id, formatter(state, exception), exception));
        }
    }

    private sealed record LogEntry(
        int EventId,
        string Message,
        Exception? Exception);
}
