namespace ITSupportNative.DeviceAgent.Execution;

public sealed record ProcessExecutionRequest(
    string FileName,
    IReadOnlyList<string> Arguments,
    TimeSpan Timeout);
