namespace ITSupportNative.Contracts.Agent;

public sealed record AgentPrerequisiteResult(
    AgentPrerequisiteKind Kind,
    AgentPrerequisiteEvaluation Evaluation,
    string Code,
    long? RequiredBytes,
    long? ActualBytes,
    string? RequiredValue,
    string? ActualValue);
