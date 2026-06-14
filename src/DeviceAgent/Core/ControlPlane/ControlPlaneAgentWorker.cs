using ITSupportNative.Contracts.ControlPlane;
using ITSupportNative.DeviceAgent.Configuration;
using ITSupportNative.DeviceAgent.Jobs;
using ITSupportNative.DeviceAgent.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ITSupportNative.DeviceAgent.ControlPlane;

public sealed class ControlPlaneAgentWorker(
    ControlPlaneAgentSyncService synchronization,
    AgentJobExecutionGate executionGate,
    IOptions<DeviceAgentOptions> options,
    ILogger<ControlPlaneAgentWorker> logger) : BackgroundService
{
    private readonly DeviceAgentOptions _options = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!IsEnabled())
        {
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                ControlPlaneSupportRequest? request =
                    await synchronization.RunOnceAsync(stoppingToken);
                if (request is null)
                {
                    await Task.Delay(
                        _options.ControlPlanePollingIntervalMilliseconds,
                        stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception)
            {
                AgentRuntimeLog.SynchronizationFailure(logger);
                await Task.Delay(
                    _options.ControlPlanePollingIntervalMilliseconds,
                    stoppingToken);
            }
        }
    }

    private bool IsEnabled()
    {
        return _options.ControlPlaneSyncEnabled
            && executionGate.IsEnabled
            && string.Equals(
                _options.ExecutionProfile,
                "local-demo",
                StringComparison.Ordinal);
    }
}
