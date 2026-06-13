using ITSupportNative.Contracts.ControlPlane;
using ITSupportNative.DeviceAgent.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ITSupportNative.DeviceAgent.ControlPlane;

public sealed class ControlPlaneAgentWorker(
    ControlPlaneAgentSyncService synchronization,
    IOptions<DeviceAgentOptions> options,
    ILogger<ControlPlaneAgentWorker> logger) : BackgroundService
{
    private static readonly Action<ILogger, Exception?> LogSynchronizationFailure =
        LoggerMessage.Define(
            LogLevel.Warning,
            new EventId(3001, nameof(LogSynchronizationFailure)),
            "The local control plane synchronization attempt failed.");

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
                LogSynchronizationFailure(logger, null);
                await Task.Delay(
                    _options.ControlPlanePollingIntervalMilliseconds,
                    stoppingToken);
            }
        }
    }

    private bool IsEnabled()
    {
        return _options.ControlPlaneSyncEnabled
            && string.Equals(
                _options.ExecutionProfile,
                "local-demo",
                StringComparison.Ordinal);
    }
}
