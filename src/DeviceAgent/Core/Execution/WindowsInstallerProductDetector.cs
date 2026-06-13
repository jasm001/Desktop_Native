using Microsoft.Win32;

namespace ITSupportNative.DeviceAgent.Execution;

public sealed class WindowsInstallerProductDetector : ISoftwareProductDetector
{
    private const string UninstallRegistryPath =
        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

    public Task<SoftwareDetectionResult> DetectAsync(
        string productCode,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productCode);
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            using RegistryKey baseKey = RegistryKey.OpenBaseKey(
                RegistryHive.LocalMachine,
                RegistryView.Registry64);
            using RegistryKey? productKey = baseKey.OpenSubKey(
                $@"{UninstallRegistryPath}\{productCode}");
            if (productKey is null)
            {
                return Task.FromResult(SoftwareDetectionResult.Absent());
            }

            string? version = productKey.GetValue("DisplayVersion") as string;
            return string.IsNullOrWhiteSpace(version)
                ? Task.FromResult(SoftwareDetectionResult.Unavailable())
                : Task.FromResult(SoftwareDetectionResult.Installed(version));
        }
        catch (Exception exception) when (
            exception is IOException
            or System.Security.SecurityException
            or UnauthorizedAccessException)
        {
            return Task.FromResult(SoftwareDetectionResult.Unavailable());
        }
    }
}
