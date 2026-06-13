using System.Runtime.InteropServices;

namespace ITSupportNative.DeviceAgent.Execution;

public sealed class WindowsX64ExecutionPlatform : IExecutionPlatform
{
    public bool IsWindowsX64 =>
        OperatingSystem.IsWindows()
        && RuntimeInformation.OSArchitecture == Architecture.X64;
}
