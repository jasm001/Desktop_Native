namespace ITSupportNative.DeviceAgent.Execution;

public sealed record SoftwareArtifactDefinition(
    string ArtifactId,
    string FileName,
    long Length,
    string Sha256);
