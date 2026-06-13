using ITSupportNative.DeviceAgent.Diagnostics;

namespace ITSupportNative.DeviceAgent.Jobs;

public sealed record AuthorizedAgentAction(
    string ActionId,
    string TargetId,
    string TargetVersion,
    IReadOnlyList<AgentPrerequisiteDefinition> Prerequisites)
{
    public AgentActionExecutionKind ExecutionKind { get; init; } =
        AgentActionExecutionKind.Simulated;

    public string? AdapterId { get; init; }

    public bool SupportsRunningCancellation { get; init; } = true;
}
