namespace ITSupportNative.DeviceAgent.Execution;

public interface ISoftwareArtifactSource
{
    Task<SoftwareArtifactResolution> ResolveAsync(
        SoftwareArtifactDefinition artifact,
        CancellationToken cancellationToken);
}
