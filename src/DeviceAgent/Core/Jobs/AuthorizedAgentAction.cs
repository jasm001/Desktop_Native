using ITSupportNative.DeviceAgent.Diagnostics;

namespace ITSupportNative.DeviceAgent.Jobs;

public sealed record AuthorizedAgentAction(
    string ActionId,
    string TargetId,
    string TargetVersion,
    IReadOnlyList<AgentPrerequisiteDefinition> Prerequisites);
