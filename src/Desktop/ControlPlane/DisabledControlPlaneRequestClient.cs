using ITSupportNative.Contracts.ControlPlane;

namespace ITSupportNative.Desktop.ControlPlane;

public sealed class DisabledControlPlaneRequestClient : IControlPlaneRequestClient
{
    public Task<CreateSoftwareInstallationData?> CreateSoftwareInstallationAsync(
        string idempotencyKey,
        string productId,
        string productVersion,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<CreateSoftwareInstallationData?>(null);
    }

    public Task<ControlPlaneSupportRequest?> GetSupportRequestAsync(
        string requestId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<ControlPlaneSupportRequest?>(null);
    }
}
