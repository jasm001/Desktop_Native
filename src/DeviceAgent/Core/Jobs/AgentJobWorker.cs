using ITSupportNative.DeviceAgent.Configuration;
using ITSupportNative.DeviceAgent.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ITSupportNative.DeviceAgent.Jobs;

public sealed class AgentJobWorker(
    AgentJobService jobs,
    IOptions<DeviceAgentOptions> options,
    ILogger<AgentJobWorker> logger) : BackgroundService
{
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
            catch (Exception)
            {
                AgentRuntimeLog.JobLoopFailure(logger);
            }
        }
    }
}
