using ITSupportNative.Contracts.ControlPlane;

namespace ITSupportNative.Desktop.ControlPlane;

public sealed class DisabledControlPlaneRequestClient : IControlPlaneRequestClient
{
    public Task<CreateSoftwareInstallationData?> CreateSoftwareInstallationAsync(
        string correlationId,
        string idempotencyKey,
        string deviceId,
        string productId,
        string productVersion,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<CreateSoftwareInstallationData?>(null);
    }

    public Task<ControlPlaneSupportRequest?> GetSupportRequestAsync(
        string correlationId,
        string requestId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<ControlPlaneSupportRequest?>(null);
    }

    public Task<ControlPlaneBotCase?> GetBotCaseAsync(
        string correlationId,
        string requestId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<ControlPlaneBotCase?>(null);
    }
}
