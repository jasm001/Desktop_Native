namespace ITSupportNative.DeviceAgent.Execution;

public enum SoftwareArtifactResolutionCode
{
    Available,
    SourceUnavailable,
    LengthMismatch,
    HashMismatch,
}

public sealed record SoftwareArtifactResolution(
    SoftwareArtifactResolutionCode Code,
    string? TrustedPath)
{
    public bool IsAvailable =>
        Code == SoftwareArtifactResolutionCode.Available
        && TrustedPath is not null;

    public static SoftwareArtifactResolution Available(string trustedPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(trustedPath);
        return new(SoftwareArtifactResolutionCode.Available, trustedPath);
    }

    public static SoftwareArtifactResolution Rejected(
        SoftwareArtifactResolutionCode code)
    {
        return new(code, TrustedPath: null);
    }
}
