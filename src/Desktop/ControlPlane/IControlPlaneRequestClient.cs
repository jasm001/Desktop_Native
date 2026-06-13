using ITSupportNative.Contracts.ControlPlane;

namespace ITSupportNative.Desktop.ControlPlane;

public interface IControlPlaneRequestClient
{
    Task<CreateSoftwareInstallationData?> CreateSoftwareInstallationAsync(
        string idempotencyKey,
        string productId,
        string productVersion,
        CancellationToken cancellationToken);

    Task<ControlPlaneSupportRequest?> GetSupportRequestAsync(
        string requestId,
        CancellationToken cancellationToken);
}
