using System.Runtime.InteropServices;
using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Diagnostics.Infrastructure;

public sealed partial class MemoryDiagnosticCollector : IMemoryDiagnosticCollector
{
    public Task<MemoryDiagnosticResult> CollectAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var status = new MemoryStatus
        {
            Length = (uint)Marshal.SizeOf<MemoryStatus>(),
        };
        if (!GlobalMemoryStatusEx(ref status)
            || status.TotalPhysicalBytes > long.MaxValue
            || status.AvailablePhysicalBytes > long.MaxValue)
        {
            return Task.FromResult(
                new MemoryDiagnosticResult(
                    DiagnosticCollectionStatus.Unavailable,
                    "memory.unavailable",
                    TotalBytes: null,
                    AvailableBytes: null));
        }

        return Task.FromResult(
            new MemoryDiagnosticResult(
                DiagnosticCollectionStatus.Available,
                "memory.available",
                (long)status.TotalPhysicalBytes,
                (long)status.AvailablePhysicalBytes));
    }

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GlobalMemoryStatusEx(ref MemoryStatus buffer);

    [StructLayout(LayoutKind.Sequential)]
    private struct MemoryStatus
    {
        public uint Length;
        public uint MemoryLoadPercent;
        public ulong TotalPhysicalBytes;
        public ulong AvailablePhysicalBytes;
        public ulong TotalPageFileBytes;
        public ulong AvailablePageFileBytes;
        public ulong TotalVirtualBytes;
        public ulong AvailableVirtualBytes;
        public ulong AvailableExtendedVirtualBytes;
    }
}
