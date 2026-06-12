namespace ITSupportNative.Contracts.Agent;

public sealed record MemoryDiagnosticResult(
    DiagnosticCollectionStatus Status,
    string Code,
    long? TotalBytes,
    long? AvailableBytes);
