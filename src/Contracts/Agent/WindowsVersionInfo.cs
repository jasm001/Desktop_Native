namespace ITSupportNative.Contracts.Agent;

public sealed record WindowsVersionInfo(
    int Major,
    int Minor,
    int Build,
    int Revision);
