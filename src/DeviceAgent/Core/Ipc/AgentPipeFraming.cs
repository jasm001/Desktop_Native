using System.Buffers.Binary;
using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.DeviceAgent.Ipc;

public static class AgentPipeFraming
{
    private const int HeaderLength = sizeof(int);

    public static async Task<byte[]> ReadAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stream);

        byte[] header = new byte[HeaderLength];
        await ReadExactlyAsync(stream, header, cancellationToken);
        int length = BinaryPrimitives.ReadInt32LittleEndian(header);
        if (length <= 0 || length > AgentProtocol.MaximumMessageBytes)
        {
            throw new InvalidDataException("The IPC message length is outside the allowed range.");
        }

        byte[] payload = new byte[length];
        await ReadExactlyAsync(stream, payload, cancellationToken);
        return payload;
    }

    public static async Task WriteAsync(
        Stream stream,
        ReadOnlyMemory<byte> payload,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (payload.Length <= 0 || payload.Length > AgentProtocol.MaximumMessageBytes)
        {
            throw new InvalidDataException("The IPC message length is outside the allowed range.");
        }

        byte[] header = new byte[HeaderLength];
        BinaryPrimitives.WriteInt32LittleEndian(header, payload.Length);
        await stream.WriteAsync(header, cancellationToken);
        await stream.WriteAsync(payload, cancellationToken);
        await stream.FlushAsync(cancellationToken);
    }

    private static async Task ReadExactlyAsync(
        Stream stream,
        Memory<byte> buffer,
        CancellationToken cancellationToken)
    {
        int offset = 0;
        while (offset < buffer.Length)
        {
            int read = await stream.ReadAsync(buffer[offset..], cancellationToken);
            if (read == 0)
            {
                throw new EndOfStreamException("The IPC connection closed before the message completed.");
            }

            offset += read;
        }
    }
}
