namespace ITSupportNative.DeviceAgent.Execution;

public static class SevenZip2601X64Definition
{
    public const string AdapterId = "seven-zip.msi.v1";
    public const string TargetId = "seven-zip";
    public const string TargetVersion = "26.01";
    public const string PackageVersion = "26.01.00.0";
    public const string ProductCode = "{23170F69-40C1-2702-2601-000001000000}";
    public const string InstallActionId = "software.install.7zip.v1";
    public const string UninstallActionId = "software.uninstall.7zip.v1";

    public static readonly SoftwareArtifactDefinition Artifact = new(
        "seven-zip-26.01-x64-msi",
        "7z2601-x64.msi",
        2_002_432,
        "A47EA8DCF8BC08E6DE474CAE77C828E031FA22CB528F6095DEFFFEBF11CD02F2");

    public static readonly TimeSpan OperationTimeout = TimeSpan.FromMinutes(5);
}
