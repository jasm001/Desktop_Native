using ITSupportNative.DeviceAgent.Execution;

namespace ITSupportNative.DeviceAgent.Jobs;

public sealed class SoftwareAgentActionExecutor(
    IEnumerable<ISoftwareExecutionAdapter> adapters) : IAgentActionExecutor
{
    private readonly IReadOnlyList<ISoftwareExecutionAdapter> _adapters =
        [.. adapters];

    public bool CanExecute(AuthorizedAgentAction action)
    {
        ArgumentNullException.ThrowIfNull(action);
        return FindAdapter(action) is not null;
    }

    public async Task<AgentActionExecutionResult> ExecuteAsync(
        AuthorizedAgentAction action,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(action);
        ISoftwareExecutionAdapter? adapter = FindAdapter(action);
        if (adapter is null)
        {
            return new(
                Success: false,
                "job.execution.adapter_unavailable",
                "The installed policy did not resolve an execution adapter.");
        }

        SoftwareExecutionResult result = action.ExecutionKind switch
        {
            AgentActionExecutionKind.SoftwareInstall =>
                await adapter.InstallAsync(cancellationToken),
            AgentActionExecutionKind.SoftwareUninstall =>
                await adapter.UninstallAsync(cancellationToken),
            _ => new(SoftwareExecutionResultCode.ExitCodeRejected),
        };

        return ToActionResult(result);
    }

    private ISoftwareExecutionAdapter? FindAdapter(AuthorizedAgentAction action)
    {
        ISoftwareExecutionAdapter[] matches = [.. _adapters.Where(adapter =>
            string.Equals(adapter.AdapterId, action.AdapterId, StringComparison.Ordinal)
            && string.Equals(adapter.TargetId, action.TargetId, StringComparison.Ordinal)
            && string.Equals(adapter.TargetVersion, action.TargetVersion, StringComparison.Ordinal))
            .Take(2)];
        return matches.Length == 1 ? matches[0] : null;
    }

    private static AgentActionExecutionResult ToActionResult(
        SoftwareExecutionResult result)
    {
        return result.Code switch
        {
            SoftwareExecutionResultCode.AlreadyInDesiredState => new(
                Success: true,
                "job.execution.idempotent",
                "The target already matched the requested state."),
            SoftwareExecutionResultCode.Succeeded => new(
                Success: true,
                "job.execution.verified",
                "The adapter completed and verified the requested state."),
            SoftwareExecutionResultCode.SucceededRebootRequired => new(
                Success: true,
                "job.execution.verified_reboot_required",
                "The adapter verified the requested state and reported a pending reboot."),
            SoftwareExecutionResultCode.ProfileDisabled => Failure(
                "job.execution.profile_disabled",
                "Real execution is disabled outside the local-demo profile."),
            SoftwareExecutionResultCode.UnsupportedPlatform => Failure(
                "job.execution.unsupported_platform",
                "The adapter rejected the operating system or architecture."),
            SoftwareExecutionResultCode.ArtifactUnavailable => Failure(
                "job.execution.artifact_unavailable",
                "The allowlisted artifact was not available from the local mirror."),
            SoftwareExecutionResultCode.ArtifactLengthMismatch => Failure(
                "job.execution.artifact_length_mismatch",
                "The allowlisted artifact length did not match the manifest."),
            SoftwareExecutionResultCode.ArtifactHashMismatch => Failure(
                "job.execution.artifact_hash_mismatch",
                "The allowlisted artifact failed SHA-256 verification."),
            SoftwareExecutionResultCode.ProcessStartFailed => Failure(
                "job.execution.process_start_failed",
                "The fixed installer process could not be started."),
            SoftwareExecutionResultCode.TimedOut => Failure(
                "job.execution.timed_out",
                "The fixed installer process exceeded its declared timeout."),
            SoftwareExecutionResultCode.RebootInitiated => Failure(
                "job.execution.reboot_initiated",
                "The installer reported an unexpected reboot initiation."),
            SoftwareExecutionResultCode.VerificationFailed => Failure(
                "job.execution.verification_failed",
                "The postcondition could not be verified."),
            SoftwareExecutionResultCode.DetectionUnavailable => Failure(
                "job.execution.detection_unavailable",
                "The installed state could not be determined safely."),
            _ => Failure(
                "job.execution.exit_code_rejected",
                "The installer returned an exit code outside the allowlist."),
        };
    }

    private static AgentActionExecutionResult Failure(string code, string summary)
    {
        return new(Success: false, code, summary);
    }
}
