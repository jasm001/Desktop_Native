using ITSupportNative.Contracts.Agent;
using ITSupportNative.DeviceAgent.Diagnostics;

namespace ITSupportNative.DeviceAgent.Jobs;

public sealed class AgentActionAuthorizationPolicy
{
    private static readonly AuthorizedAgentAction[] DevelopmentAllowlist =
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

    private readonly IReadOnlyList<AuthorizedAgentAction> _allowlist;

    public AgentActionAuthorizationPolicy()
        : this(DevelopmentAllowlist)
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
}
