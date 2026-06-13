namespace ITSupportNative.DeviceAgent.Execution;

public interface ISoftwareProductDetector
{
    Task<SoftwareDetectionResult> DetectAsync(
        string productCode,
        CancellationToken cancellationToken);
}
