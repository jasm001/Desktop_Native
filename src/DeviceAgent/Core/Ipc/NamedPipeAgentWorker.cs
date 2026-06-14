using System.IO.Pipes;
using System.Text.Json;
using ITSupportNative.Contracts.Agent;
using ITSupportNative.DeviceAgent.Configuration;
using ITSupportNative.DeviceAgent.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ITSupportNative.DeviceAgent.Ipc;

public sealed class NamedPipeAgentWorker(
    AgentRequestDispatcher dispatcher,
    IOptions<DeviceAgentOptions> options,
    ILogger<NamedPipeAgentWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string pipeName = options.Value.PipeName;

        while (!stoppingToken.IsCancellationRequested)
        {
            await using NamedPipeServerStream pipe = CreateServer(pipeName);
            try
            {
                await pipe.WaitForConnectionAsync(stoppingToken);
                await HandleConnectionAsync(pipe, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception)
            {
                AgentRuntimeLog.IpcRequestFailure(logger);
            }
        }
    }

    public static NamedPipeServerStream CreateServer(string pipeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pipeName);

        return new(
            pipeName,
            PipeDirection.InOut,
            maxNumberOfServerInstances: 1,
            PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
    }

    private async Task HandleConnectionAsync(
        NamedPipeServerStream pipe,
        CancellationToken cancellationToken)
    {
        string correlationId = "unavailable";
        AgentResponseEnvelope response;

        try
        {
            byte[] requestBytes = await AgentPipeFraming.ReadAsync(pipe, cancellationToken);
            AgentRequestEnvelope? request = AgentJson.Deserialize<AgentRequestEnvelope>(requestBytes);
            if (request is null)
            {
                response = AgentJson.CreateError(
                    correlationId,
                    AgentErrorCode.InvalidMessage,
                    "The IPC request is empty or malformed.");
            }
            else
            {
                correlationId = string.IsNullOrWhiteSpace(request.CorrelationId)
                    ? correlationId
                    : request.CorrelationId;
                response = await dispatcher.DispatchAsync(request, cancellationToken);
            }
        }
        catch (Exception exception) when (exception is InvalidDataException or EndOfStreamException or JsonException)
        {
            response = AgentJson.CreateError(
                correlationId,
                AgentErrorCode.InvalidMessage,
                "The IPC request is malformed or exceeds the protocol limits.");
        }

        await AgentPipeFraming.WriteAsync(
            pipe,
            AgentJson.Serialize(response),
            cancellationToken);
    }
}
