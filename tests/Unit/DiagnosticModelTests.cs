using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.UnitTests;

public sealed class DiagnosticModelTests
{
    [Fact]
    public void SnapshotKeepsTypedSectionsByteUnitsAndPrerequisiteState()
    {
        var snapshot = new AgentDiagnosticSnapshot(
            DateTimeOffset.UnixEpoch,
            new(
                DiagnosticCollectionStatus.Available,
                "windows.available",
                new(10, 0, 26100, 0),
                "x64"),
            new(
                DiagnosticCollectionStatus.Available,
                "storage.available",
                CapacityBytes: 8192,
                AvailableBytes: 4096,
                FixedVolumeCount: 1),
            new(
                DiagnosticCollectionStatus.Available,
                "memory.available",
                TotalBytes: 4096,
                AvailableBytes: 2048),
            new(
                DiagnosticCollectionStatus.Available,
                "network.domain_not_applicable",
                NetworkAvailable: true,
                DomainReachabilityStatus.NotApplicable),
            new(
                DiagnosticCollectionStatus.Available,
                "agent.version.available",
                "1.0.0.0"),
            [
                new(
                    AgentPrerequisiteKind.MinimumStorageAvailableBytes,
                    AgentPrerequisiteEvaluation.Satisfied,
                    "prerequisite.satisfied",
                    RequiredBytes: 1024,
                    ActualBytes: 4096,
                    RequiredValue: null,
                    ActualValue: null),
            ]);

        Assert.Equal(4096, snapshot.Storage.AvailableBytes);
        Assert.Equal(2048, snapshot.Memory.AvailableBytes);
        Assert.Equal(
            AgentPrerequisiteEvaluation.Satisfied,
            snapshot.ActionPrerequisites.Single().Evaluation);
        Assert.Null(snapshot.ActionPrerequisites.Single().RequiredValue);
    }
}
