using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Diagnostics.Infrastructure;

public sealed class StorageDiagnosticCollector : IStorageDiagnosticCollector
{
    private const int MaximumFixedVolumes = 32;

    public Task<StorageDiagnosticResult> CollectAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        DriveInfo[] fixedVolumes =
        [
            .. DriveInfo.GetDrives()
                .Where(drive => drive.DriveType == DriveType.Fixed && drive.IsReady)
                .OrderBy(drive => drive.Name, StringComparer.Ordinal),
        ];
        if (fixedVolumes.Length > MaximumFixedVolumes)
        {
            return Task.FromResult(
                new StorageDiagnosticResult(
                    DiagnosticCollectionStatus.Unavailable,
                    "storage.volume_limit_exceeded",
                    CapacityBytes: null,
                    AvailableBytes: null,
                    FixedVolumeCount: null));
        }

        long capacityBytes = 0;
        long availableBytes = 0;
        checked
        {
            foreach (DriveInfo volume in fixedVolumes)
            {
                cancellationToken.ThrowIfCancellationRequested();
                capacityBytes += volume.TotalSize;
                availableBytes += volume.AvailableFreeSpace;
            }
        }

        return Task.FromResult(
            new StorageDiagnosticResult(
                DiagnosticCollectionStatus.Available,
                "storage.available",
                capacityBytes,
                availableBytes,
                fixedVolumes.Length));
    }
}
