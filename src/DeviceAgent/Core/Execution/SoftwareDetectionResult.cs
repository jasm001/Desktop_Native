namespace ITSupportNative.DeviceAgent.Execution;

public enum SoftwareDetectionState
{
    Absent,
    Installed,
    Unavailable,
}

public sealed record SoftwareDetectionResult(
    SoftwareDetectionState State,
    string? Version)
{
    public static SoftwareDetectionResult Absent()
    {
        return new(SoftwareDetectionState.Absent, Version: null);
    }

    public static SoftwareDetectionResult Installed(string version)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        return new(SoftwareDetectionState.Installed, version);
    }

    public static SoftwareDetectionResult Unavailable()
    {
        return new(SoftwareDetectionState.Unavailable, Version: null);
    }
}
