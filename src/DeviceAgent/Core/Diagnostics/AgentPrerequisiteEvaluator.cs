using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Diagnostics;

public static class AgentPrerequisiteEvaluator
{
    public static IReadOnlyList<AgentPrerequisiteResult> Evaluate(
        IReadOnlyList<AgentPrerequisiteDefinition> definitions,
        WindowsDiagnosticResult windows,
        StorageDiagnosticResult storage,
        MemoryDiagnosticResult memory,
        NetworkDiagnosticResult network)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        return
        [
            .. definitions
                .OrderBy(definition => definition.Kind)
                .Select(definition => Evaluate(definition, windows, storage, memory, network)),
        ];
    }

    private static AgentPrerequisiteResult Evaluate(
        AgentPrerequisiteDefinition definition,
        WindowsDiagnosticResult windows,
        StorageDiagnosticResult storage,
        MemoryDiagnosticResult memory,
        NetworkDiagnosticResult network)
    {
        return definition.Kind switch
        {
            AgentPrerequisiteKind.WindowsOperatingSystem => EvaluateText(
                definition,
                windows.Status == DiagnosticCollectionStatus.Available ? "windows" : null),
            AgentPrerequisiteKind.Architecture => EvaluateText(
                definition,
                windows.Status == DiagnosticCollectionStatus.Available
                    ? windows.Architecture
                    : null),
            AgentPrerequisiteKind.MinimumStorageAvailableBytes => EvaluateBytes(
                definition,
                storage.Status == DiagnosticCollectionStatus.Available
                    ? storage.AvailableBytes
                    : null),
            AgentPrerequisiteKind.MinimumMemoryAvailableBytes => EvaluateBytes(
                definition,
                memory.Status == DiagnosticCollectionStatus.Available
                    ? memory.AvailableBytes
                    : null),
            AgentPrerequisiteKind.NetworkAvailable => EvaluateBoolean(
                definition,
                network.NetworkAvailable),
            AgentPrerequisiteKind.DomainReachable => EvaluateBoolean(
                definition,
                network.DomainReachability switch
                {
                    DomainReachabilityStatus.Reachable => true,
                    DomainReachabilityStatus.Unreachable => false,
                    _ => null,
                }),
            _ => Unknown(definition),
        };
    }

    private static AgentPrerequisiteResult EvaluateBytes(
        AgentPrerequisiteDefinition definition,
        long? actualBytes)
    {
        if (definition.RequiredBytes is null || actualBytes is null)
        {
            return Unknown(definition, actualBytes: actualBytes);
        }

        return Result(
            definition,
            actualBytes >= definition.RequiredBytes
                ? AgentPrerequisiteEvaluation.Satisfied
                : AgentPrerequisiteEvaluation.NotSatisfied,
            actualBytes: actualBytes);
    }

    private static AgentPrerequisiteResult EvaluateText(
        AgentPrerequisiteDefinition definition,
        string? actualValue)
    {
        if (definition.RequiredValue is null || actualValue is null)
        {
            return Unknown(definition, actualValue: actualValue);
        }

        return Result(
            definition,
            string.Equals(definition.RequiredValue, actualValue, StringComparison.OrdinalIgnoreCase)
                ? AgentPrerequisiteEvaluation.Satisfied
                : AgentPrerequisiteEvaluation.NotSatisfied,
            actualValue: actualValue);
    }

    private static AgentPrerequisiteResult EvaluateBoolean(
        AgentPrerequisiteDefinition definition,
        bool? actualValue)
    {
        return actualValue is null
            ? Unknown(definition)
            : Result(
                definition,
                actualValue.Value
                    ? AgentPrerequisiteEvaluation.Satisfied
                    : AgentPrerequisiteEvaluation.NotSatisfied);
    }

    private static AgentPrerequisiteResult Unknown(
        AgentPrerequisiteDefinition definition,
        long? actualBytes = null,
        string? actualValue = null)
    {
        return Result(
            definition,
            AgentPrerequisiteEvaluation.Unknown,
            actualBytes,
            actualValue);
    }

    private static AgentPrerequisiteResult Result(
        AgentPrerequisiteDefinition definition,
        AgentPrerequisiteEvaluation evaluation,
        long? actualBytes = null,
        string? actualValue = null)
    {
        string code = evaluation switch
        {
            AgentPrerequisiteEvaluation.Satisfied => "prerequisite.satisfied",
            AgentPrerequisiteEvaluation.NotSatisfied => "prerequisite.not_satisfied",
            _ => "prerequisite.unknown",
        };

        return new(
            definition.Kind,
            evaluation,
            code,
            definition.RequiredBytes,
            actualBytes,
            definition.RequiredValue,
            actualValue);
    }
}
