using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Jobs;

public sealed class AgentActionAuthorizationPolicy
{
    private static readonly AuthorizedAgentAction[] DevelopmentAllowlist =
    [
        new(
            "software.install.simulated.v1",
            "secure-transfer",
            "6.5"),
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

        return _allowlist.Any(action =>
            string.Equals(action.ActionId, request.ActionId, StringComparison.Ordinal)
            && string.Equals(action.TargetId, request.TargetId, StringComparison.Ordinal)
            && string.Equals(action.TargetVersion, request.TargetVersion, StringComparison.Ordinal));
    }
}
