using ITSupportNative.Contracts.Agent;
using ITSupportNative.DeviceAgent.Diagnostics;
using ITSupportNative.DeviceAgent.Execution;

namespace ITSupportNative.DeviceAgent.Jobs;

public sealed class AgentActionAuthorizationPolicy
{
    private const string LocalDemoProfile = "local-demo";

    private static readonly AuthorizedAgentAction[] SyntheticAllowlist =
    [
        new(
            "software.install.simulated.v1",
            "secure-transfer",
            "6.5",
            [
                AgentPrerequisiteDefinition.ForRequiredValue(
                    AgentPrerequisiteKind.WindowsOperatingSystem,
                    "windows"),
                AgentPrerequisiteDefinition.ForRequiredValue(
                    AgentPrerequisiteKind.Architecture,
                    "x64"),
                AgentPrerequisiteDefinition.ForRequiredBytes(
                    AgentPrerequisiteKind.MinimumStorageAvailableBytes,
                    1L * 1024 * 1024 * 1024),
                AgentPrerequisiteDefinition.ForRequiredBytes(
                    AgentPrerequisiteKind.MinimumMemoryAvailableBytes,
                    512L * 1024 * 1024),
                AgentPrerequisiteDefinition.Required(
                    AgentPrerequisiteKind.NetworkAvailable),
            ]),
    ];

    private static readonly AuthorizedAgentAction[] LocalDemoAllowlist =
    [
        .. SyntheticAllowlist,
        new(
            SevenZip2601X64Definition.InstallActionId,
            SevenZip2601X64Definition.TargetId,
            SevenZip2601X64Definition.TargetVersion,
            SevenZipPrerequisites())
        {
            ExecutionKind = AgentActionExecutionKind.SoftwareInstall,
            AdapterId = SevenZip2601X64Definition.AdapterId,
            SupportsRunningCancellation = false,
        },
        new(
            SevenZip2601X64Definition.UninstallActionId,
            SevenZip2601X64Definition.TargetId,
            SevenZip2601X64Definition.TargetVersion,
            SevenZipPrerequisites())
        {
            ExecutionKind = AgentActionExecutionKind.SoftwareUninstall,
            AdapterId = SevenZip2601X64Definition.AdapterId,
            SupportsRunningCancellation = false,
        },
    ];

    private readonly IReadOnlyList<AuthorizedAgentAction> _allowlist;

    public AgentActionAuthorizationPolicy()
        : this(SyntheticAllowlist)
    {
    }

    public AgentActionAuthorizationPolicy(string executionProfile)
        : this(string.Equals(executionProfile, LocalDemoProfile, StringComparison.Ordinal)
            ? LocalDemoAllowlist
            : SyntheticAllowlist)
    {
    }

    public AgentActionAuthorizationPolicy(IReadOnlyList<AuthorizedAgentAction> allowlist)
    {
        ArgumentNullException.ThrowIfNull(allowlist);
        _allowlist = allowlist;
    }

    public bool IsAuthorized(StartAgentJobRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return Find(request.ActionId, request.TargetId, request.TargetVersion) is not null;
    }

    public AuthorizedAgentAction? Find(
        string actionId,
        string targetId,
        string targetVersion)
    {
        return _allowlist.FirstOrDefault(action =>
            string.Equals(action.ActionId, actionId, StringComparison.Ordinal)
            && string.Equals(action.TargetId, targetId, StringComparison.Ordinal)
            && string.Equals(action.TargetVersion, targetVersion, StringComparison.Ordinal));
    }

    private static AgentPrerequisiteDefinition[] SevenZipPrerequisites()
    {
        return
        [
            AgentPrerequisiteDefinition.ForRequiredValue(
                AgentPrerequisiteKind.WindowsOperatingSystem,
                "windows"),
            AgentPrerequisiteDefinition.ForRequiredValue(
                AgentPrerequisiteKind.Architecture,
                "x64"),
            AgentPrerequisiteDefinition.ForRequiredBytes(
                AgentPrerequisiteKind.MinimumStorageAvailableBytes,
                64L * 1024 * 1024),
            AgentPrerequisiteDefinition.ForRequiredBytes(
                AgentPrerequisiteKind.MinimumMemoryAvailableBytes,
                256L * 1024 * 1024),
        ];
    }
}
