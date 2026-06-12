namespace ITSupportNative.Contracts.Agent;

public sealed record AgentDiagnosticSnapshot(
    DateTimeOffset CapturedAtUtc,
    WindowsDiagnosticResult Windows,
    StorageDiagnosticResult Storage,
    MemoryDiagnosticResult Memory,
    NetworkDiagnosticResult Network,
    AgentVersionDiagnosticResult Agent,
    IReadOnlyList<AgentPrerequisiteResult> ActionPrerequisites);
