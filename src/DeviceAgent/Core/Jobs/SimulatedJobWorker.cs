using ITSupportNative.DeviceAgent.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ITSupportNative.DeviceAgent.Jobs;

public sealed class SimulatedJobWorker(
    AgentJobService jobs,
    IOptions<DeviceAgentOptions> options,
    ILogger<SimulatedJobWorker> logger) : BackgroundService
{
    private static readonly Action<ILogger, Exception?> LogSimulationFailure =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(2001, nameof(LogSimulationFailure)),
            "The simulated job loop failed without exposing job payloads.");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TimeSpan interval = TimeSpan.FromMilliseconds(
            Math.Max(50, options.Value.SimulationIntervalMilliseconds));

        using var timer = new PeriodicTimer(interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await jobs.AdvancePendingJobsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                LogSimulationFailure(logger, exception);
            }
        }
    }
}
