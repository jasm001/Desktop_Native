namespace ITSupportNative.DeviceAgent.Execution;

public interface ISoftwareExecutionAdapter
{
    string AdapterId { get; }

    string TargetId { get; }

    string TargetVersion { get; }

    Task<SoftwareExecutionResult> DetectAsync(CancellationToken cancellationToken);

    Task<SoftwareExecutionResult> PreflightAsync(CancellationToken cancellationToken);

    Task<SoftwareExecutionResult> InstallAsync(CancellationToken cancellationToken);

    Task<SoftwareExecutionResult> VerifyAsync(CancellationToken cancellationToken);

    Task<SoftwareExecutionResult> UninstallAsync(CancellationToken cancellationToken);
}
