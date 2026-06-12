using ITSupportNative.Contracts.Agent;
using ITSupportNative.DeviceAgent.Diagnostics;

namespace ITSupportNative.IntegrationTests;

public sealed class AgentPrerequisiteEvaluatorTests
{
    [Fact]
    public void ResultsAreOrderedAndUseBytesForCapacityRules()
    {
        AgentPrerequisiteDefinition[] definitions =
        [
            AgentPrerequisiteDefinition.ForRequiredBytes(
                AgentPrerequisiteKind.MinimumMemoryAvailableBytes,
                1024),
            AgentPrerequisiteDefinition.ForRequiredValue(
                AgentPrerequisiteKind.Architecture,
                "x64"),
            AgentPrerequisiteDefinition.ForRequiredBytes(
                AgentPrerequisiteKind.MinimumStorageAvailableBytes,
                4096),
        ];

        IReadOnlyList<AgentPrerequisiteResult> results =
            AgentPrerequisiteEvaluator.Evaluate(
                definitions,
                AvailableWindows("x64"),
                AvailableStorage(8192),
                AvailableMemory(512),
                AvailableNetwork(networkAvailable: true));

        Assert.Equal(
            [
                AgentPrerequisiteKind.Architecture,
                AgentPrerequisiteKind.MinimumStorageAvailableBytes,
                AgentPrerequisiteKind.MinimumMemoryAvailableBytes,
            ],
            results.Select(result => result.Kind));
        Assert.Equal(AgentPrerequisiteEvaluation.Satisfied, results[0].Evaluation);
        Assert.Equal(4096, results[1].RequiredBytes);
        Assert.Equal(8192, results[1].ActualBytes);
        Assert.Equal(AgentPrerequisiteEvaluation.NotSatisfied, results[2].Evaluation);
    }

    [Fact]
    public void MissingDiagnosticDataProducesUnknownWithoutHidingKnownNetworkState()
    {
        AgentPrerequisiteDefinition[] definitions =
        [
            AgentPrerequisiteDefinition.Required(
                AgentPrerequisiteKind.NetworkAvailable),
            AgentPrerequisiteDefinition.ForRequiredBytes(
                AgentPrerequisiteKind.MinimumStorageAvailableBytes,
                4096),
        ];

        IReadOnlyList<AgentPrerequisiteResult> results =
            AgentPrerequisiteEvaluator.Evaluate(
                definitions,
                AvailableWindows("x64"),
                new(
                    DiagnosticCollectionStatus.Unavailable,
                    "storage.unavailable",
                    CapacityBytes: null,
                    AvailableBytes: null,
                    FixedVolumeCount: null),
                AvailableMemory(8192),
                new(
                    DiagnosticCollectionStatus.TimedOut,
                    "network.domain_timeout",
                    NetworkAvailable: true,
                    DomainReachabilityStatus.TimedOut));

        Assert.Equal(
            AgentPrerequisiteEvaluation.Satisfied,
            results.Single(result =>
                result.Kind == AgentPrerequisiteKind.NetworkAvailable).Evaluation);
        Assert.Equal(
            AgentPrerequisiteEvaluation.Unknown,
            results.Single(result =>
                result.Kind == AgentPrerequisiteKind.MinimumStorageAvailableBytes).Evaluation);
    }

    private static WindowsDiagnosticResult AvailableWindows(string architecture)
    {
        return new(
            DiagnosticCollectionStatus.Available,
            "windows.available",
            new(10, 0, 26100, 0),
            architecture);
    }

    private static StorageDiagnosticResult AvailableStorage(long availableBytes)
    {
        return new(
            DiagnosticCollectionStatus.Available,
            "storage.available",
            CapacityBytes: availableBytes * 2,
            availableBytes,
            FixedVolumeCount: 1);
    }

    private static MemoryDiagnosticResult AvailableMemory(long availableBytes)
    {
        return new(
            DiagnosticCollectionStatus.Available,
            "memory.available",
            TotalBytes: availableBytes * 2,
            availableBytes);
    }

    private static NetworkDiagnosticResult AvailableNetwork(bool networkAvailable)
    {
        return new(
            DiagnosticCollectionStatus.Available,
            "network.domain_not_applicable",
            networkAvailable,
            DomainReachabilityStatus.NotApplicable);
    }
}
