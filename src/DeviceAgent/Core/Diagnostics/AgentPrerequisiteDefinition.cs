using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Diagnostics;

public sealed record AgentPrerequisiteDefinition(
    AgentPrerequisiteKind Kind,
    long? RequiredBytes,
    string? RequiredValue)
{
    public static AgentPrerequisiteDefinition Required(AgentPrerequisiteKind kind)
    {
        return new(kind, RequiredBytes: null, RequiredValue: null);
    }

    public static AgentPrerequisiteDefinition ForRequiredBytes(
        AgentPrerequisiteKind kind,
        long requiredBytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(requiredBytes);
        return new(kind, requiredBytes, RequiredValue: null);
    }

    public static AgentPrerequisiteDefinition ForRequiredValue(
        AgentPrerequisiteKind kind,
        string requiredValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(requiredValue);
        return new(kind, RequiredBytes: null, requiredValue);
    }
}
