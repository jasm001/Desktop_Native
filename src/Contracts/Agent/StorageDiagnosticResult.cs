namespace ITSupportNative.Contracts.Agent;

public sealed record StorageDiagnosticResult(
    DiagnosticCollectionStatus Status,
    string Code,
    long? CapacityBytes,
    long? AvailableBytes,
    int? FixedVolumeCount);
