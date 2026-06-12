namespace ITSupportNative.Contracts.Agent;

public sealed record WindowsDiagnosticResult(
    DiagnosticCollectionStatus Status,
    string Code,
    WindowsVersionInfo? Version,
    string? Architecture);
