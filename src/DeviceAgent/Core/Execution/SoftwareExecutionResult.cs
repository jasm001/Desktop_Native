namespace ITSupportNative.DeviceAgent.Execution;

public enum SoftwareExecutionResultCode
{
    Detected,
    Absent,
    Ready,
    AlreadyInDesiredState,
    Succeeded,
    SucceededRebootRequired,
    ProfileDisabled,
    UnsupportedPlatform,
    DetectionUnavailable,
    ArtifactUnavailable,
    ArtifactLengthMismatch,
    ArtifactHashMismatch,
    ProcessStartFailed,
    TimedOut,
    ExitCodeRejected,
    RebootInitiated,
    VerificationFailed,
}

public sealed record SoftwareExecutionResult(
    SoftwareExecutionResultCode Code,
    int? ExitCode = null)
{
    public bool Success => Code is
        SoftwareExecutionResultCode.Detected
        or SoftwareExecutionResultCode.Absent
        or SoftwareExecutionResultCode.Ready
        or SoftwareExecutionResultCode.AlreadyInDesiredState
        or SoftwareExecutionResultCode.Succeeded
        or SoftwareExecutionResultCode.SucceededRebootRequired;

    public bool RebootRequired =>
        Code == SoftwareExecutionResultCode.SucceededRebootRequired;
}
