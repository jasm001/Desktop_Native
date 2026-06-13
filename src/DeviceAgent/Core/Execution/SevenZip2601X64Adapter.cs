namespace ITSupportNative.DeviceAgent.Execution;

public sealed class SevenZip2601X64Adapter(
    string executionProfile,
    ISoftwareArtifactSource artifactSource,
    ISoftwareProductDetector productDetector,
    IProcessRunner processRunner,
    IExecutionPlatform platform) : ISoftwareExecutionAdapter
{
    private const string LocalDemoProfile = "local-demo";
    private const int SuccessExitCode = 0;
    private const int RebootInitiatedExitCode = 1641;
    private const int RebootRequiredExitCode = 3010;

    public string AdapterId => SevenZip2601X64Definition.AdapterId;

    public string TargetId => SevenZip2601X64Definition.TargetId;

    public string TargetVersion => SevenZip2601X64Definition.TargetVersion;

    public async Task<SoftwareExecutionResult> DetectAsync(
        CancellationToken cancellationToken)
    {
        SoftwareDetectionResult detection = await productDetector.DetectAsync(
            SevenZip2601X64Definition.ProductCode,
            cancellationToken);
        return detection.State switch
        {
            SoftwareDetectionState.Absent =>
                new(SoftwareExecutionResultCode.Absent),
            SoftwareDetectionState.Installed when string.Equals(
                detection.Version,
                SevenZip2601X64Definition.PackageVersion,
                StringComparison.Ordinal) =>
                new(SoftwareExecutionResultCode.Detected),
            _ => new(SoftwareExecutionResultCode.DetectionUnavailable),
        };
    }

    public async Task<SoftwareExecutionResult> PreflightAsync(
        CancellationToken cancellationToken)
    {
        if (!string.Equals(executionProfile, LocalDemoProfile, StringComparison.Ordinal))
        {
            return new(SoftwareExecutionResultCode.ProfileDisabled);
        }

        if (!platform.IsWindowsX64)
        {
            return new(SoftwareExecutionResultCode.UnsupportedPlatform);
        }

        SoftwareArtifactResolution artifact = await artifactSource.ResolveAsync(
            SevenZip2601X64Definition.Artifact,
            cancellationToken);
        return artifact.Code switch
        {
            SoftwareArtifactResolutionCode.Available =>
                new(SoftwareExecutionResultCode.Ready),
            SoftwareArtifactResolutionCode.LengthMismatch =>
                new(SoftwareExecutionResultCode.ArtifactLengthMismatch),
            SoftwareArtifactResolutionCode.HashMismatch =>
                new(SoftwareExecutionResultCode.ArtifactHashMismatch),
            _ => new(SoftwareExecutionResultCode.ArtifactUnavailable),
        };
    }

    public async Task<SoftwareExecutionResult> InstallAsync(
        CancellationToken cancellationToken)
    {
        SoftwareExecutionResult detection = await DetectAsync(cancellationToken);
        if (detection.Code == SoftwareExecutionResultCode.Detected)
        {
            return new(SoftwareExecutionResultCode.AlreadyInDesiredState);
        }

        if (detection.Code != SoftwareExecutionResultCode.Absent)
        {
            return detection;
        }

        SoftwareExecutionResult preflight = await PreflightAsync(cancellationToken);
        if (!preflight.Success)
        {
            return preflight;
        }

        SoftwareArtifactResolution artifact = await artifactSource.ResolveAsync(
            SevenZip2601X64Definition.Artifact,
            cancellationToken);
        if (!artifact.IsAvailable)
        {
            return MapArtifactFailure(artifact.Code);
        }

        cancellationToken.ThrowIfCancellationRequested();
        ProcessExecutionResult process = await processRunner.RunAsync(
            CreateInstallRequest(artifact.TrustedPath!),
            CancellationToken.None);
        SoftwareExecutionResult execution = MapProcessResult(process);
        if (!execution.Success)
        {
            return execution;
        }

        SoftwareExecutionResult verification = await VerifyAsync(CancellationToken.None);
        if (!verification.Success)
        {
            return new(
                SoftwareExecutionResultCode.VerificationFailed,
                process.ExitCode);
        }

        return execution;
    }

    public async Task<SoftwareExecutionResult> VerifyAsync(
        CancellationToken cancellationToken)
    {
        SoftwareExecutionResult detection = await DetectAsync(cancellationToken);
        return detection.Code == SoftwareExecutionResultCode.Detected
            ? detection
            : new(SoftwareExecutionResultCode.VerificationFailed);
    }

    public async Task<SoftwareExecutionResult> UninstallAsync(
        CancellationToken cancellationToken)
    {
        SoftwareExecutionResult detection = await DetectAsync(cancellationToken);
        if (detection.Code == SoftwareExecutionResultCode.Absent)
        {
            return new(SoftwareExecutionResultCode.AlreadyInDesiredState);
        }

        if (detection.Code != SoftwareExecutionResultCode.Detected)
        {
            return detection;
        }

        if (!string.Equals(executionProfile, LocalDemoProfile, StringComparison.Ordinal))
        {
            return new(SoftwareExecutionResultCode.ProfileDisabled);
        }

        cancellationToken.ThrowIfCancellationRequested();
        ProcessExecutionResult process = await processRunner.RunAsync(
            CreateUninstallRequest(),
            CancellationToken.None);
        SoftwareExecutionResult execution = MapProcessResult(process);
        if (!execution.Success)
        {
            return execution;
        }

        SoftwareExecutionResult after = await DetectAsync(CancellationToken.None);
        return after.Code == SoftwareExecutionResultCode.Absent
            ? execution
            : new(
                SoftwareExecutionResultCode.VerificationFailed,
                process.ExitCode);
    }

    private static ProcessExecutionRequest CreateInstallRequest(string artifactPath)
    {
        return new(
            Path.Combine(Environment.SystemDirectory, "msiexec.exe"),
            ["/i", artifactPath, "/qn", "/norestart"],
            SevenZip2601X64Definition.OperationTimeout);
    }

    private static ProcessExecutionRequest CreateUninstallRequest()
    {
        return new(
            Path.Combine(Environment.SystemDirectory, "msiexec.exe"),
            [
                "/x",
                SevenZip2601X64Definition.ProductCode,
                "/qn",
                "/norestart",
            ],
            SevenZip2601X64Definition.OperationTimeout);
    }

    private static SoftwareExecutionResult MapArtifactFailure(
        SoftwareArtifactResolutionCode code)
    {
        return code switch
        {
            SoftwareArtifactResolutionCode.LengthMismatch =>
                new(SoftwareExecutionResultCode.ArtifactLengthMismatch),
            SoftwareArtifactResolutionCode.HashMismatch =>
                new(SoftwareExecutionResultCode.ArtifactHashMismatch),
            _ => new(SoftwareExecutionResultCode.ArtifactUnavailable),
        };
    }

    private static SoftwareExecutionResult MapProcessResult(
        ProcessExecutionResult result)
    {
        return result.Code switch
        {
            ProcessExecutionResultCode.StartFailed =>
                new(SoftwareExecutionResultCode.ProcessStartFailed),
            ProcessExecutionResultCode.TimedOut =>
                new(SoftwareExecutionResultCode.TimedOut),
            _ when result.ExitCode == SuccessExitCode =>
                new(SoftwareExecutionResultCode.Succeeded, result.ExitCode),
            _ when result.ExitCode == RebootRequiredExitCode =>
                new(
                    SoftwareExecutionResultCode.SucceededRebootRequired,
                    result.ExitCode),
            _ when result.ExitCode == RebootInitiatedExitCode =>
                new(SoftwareExecutionResultCode.RebootInitiated, result.ExitCode),
            _ => new(SoftwareExecutionResultCode.ExitCodeRejected, result.ExitCode),
        };
    }
}
