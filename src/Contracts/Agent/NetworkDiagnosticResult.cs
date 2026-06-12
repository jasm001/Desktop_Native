namespace ITSupportNative.Contracts.Agent;

public sealed record NetworkDiagnosticResult(
    DiagnosticCollectionStatus Status,
    string Code,
    bool? NetworkAvailable,
    DomainReachabilityStatus DomainReachability);
