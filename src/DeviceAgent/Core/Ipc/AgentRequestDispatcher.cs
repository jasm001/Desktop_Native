using System.Text.Json;
using ITSupportNative.Contracts.Agent;
using ITSupportNative.DeviceAgent.Diagnostics;
using ITSupportNative.DeviceAgent.Jobs;

namespace ITSupportNative.DeviceAgent.Ipc;

public sealed class AgentRequestDispatcher(
    AgentJobService jobs,
    AgentDiagnosticsService diagnostics)
{
    public async Task<AgentResponseEnvelope> DispatchAsync(
        AgentRequestEnvelope request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        string correlationId = NormalizeCorrelationId(request.CorrelationId);
        if (request.Version != AgentProtocol.CurrentVersion)
        {
            return AgentJson.CreateError(
                correlationId,
                AgentErrorCode.UnsupportedVersion,
                $"Protocol version {request.Version} is not supported.");
        }

        if (string.IsNullOrWhiteSpace(request.MessageType))
        {
            return AgentJson.CreateError(
                correlationId,
                AgentErrorCode.InvalidMessage,
                "A message type is required.");
        }

        return request.MessageType switch
        {
            AgentMessageTypes.StartJob => await HandleStartAsync(
                request.Payload,
                correlationId,
                cancellationToken),
            AgentMessageTypes.GetJob => await HandleGetAsync(
                request.Payload,
                correlationId,
                cancellationToken),
            AgentMessageTypes.CancelJob => await HandleCancelAsync(
                request.Payload,
                correlationId,
                cancellationToken),
            AgentMessageTypes.GetDiagnostics => await HandleDiagnosticsAsync(
                request.Payload,
                correlationId,
                cancellationToken),
            _ => AgentJson.CreateError(
                correlationId,
                AgentErrorCode.UnknownMessage,
                "The message type is not allowlisted by this agent version."),
        };
    }

    private async Task<AgentResponseEnvelope> HandleStartAsync(
        JsonElement payload,
        string correlationId,
        CancellationToken cancellationToken)
    {
        StartAgentJobRequest? command = DeserializePayload<StartAgentJobRequest>(payload);
        if (command is null)
        {
            return InvalidPayload(correlationId);
        }

        AgentJobCommandResult result = await jobs.StartAsync(command, cancellationToken);
        return ToResponse(result, correlationId);
    }

    private async Task<AgentResponseEnvelope> HandleGetAsync(
        JsonElement payload,
        string correlationId,
        CancellationToken cancellationToken)
    {
        GetAgentJobRequest? command = DeserializePayload<GetAgentJobRequest>(payload);
        if (command is null)
        {
            return InvalidPayload(correlationId);
        }

        AgentJobCommandResult result = await jobs.GetAsync(command.JobId, cancellationToken);
        return ToResponse(result, correlationId);
    }

    private async Task<AgentResponseEnvelope> HandleCancelAsync(
        JsonElement payload,
        string correlationId,
        CancellationToken cancellationToken)
    {
        CancelAgentJobRequest? command = DeserializePayload<CancelAgentJobRequest>(payload);
        if (command is null)
        {
            return InvalidPayload(correlationId);
        }

        AgentJobCommandResult result = await jobs.CancelAsync(command.JobId, cancellationToken);
        return ToResponse(result, correlationId);
    }

    private async Task<AgentResponseEnvelope> HandleDiagnosticsAsync(
        JsonElement payload,
        string correlationId,
        CancellationToken cancellationToken)
    {
        GetAgentDiagnosticsRequest? command =
            DeserializePayload<GetAgentDiagnosticsRequest>(payload);
        if (command is null)
        {
            return InvalidPayload(correlationId);
        }

        AgentDiagnosticCommandResult result =
            await diagnostics.CollectAsync(command, cancellationToken);
        if (result.Success && result.Snapshot is not null)
        {
            return AgentJson.CreateSuccess(
                AgentMessageTypes.DiagnosticSnapshot,
                correlationId,
                result.Snapshot);
        }

        AgentError error = result.Error
            ?? new AgentError(
                AgentErrorCode.InternalError,
                "The agent did not produce a valid diagnostic result.");
        return AgentJson.CreateError(correlationId, error.Code, error.Message);
    }

    private static TPayload? DeserializePayload<TPayload>(JsonElement payload)
    {
        try
        {
            return AgentJson.DeserializePayload<TPayload>(payload);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    private static AgentResponseEnvelope ToResponse(
        AgentJobCommandResult result,
        string correlationId)
    {
        if (result.Success && result.Job is not null)
        {
            return AgentJson.CreateSuccess(
                AgentMessageTypes.JobSnapshot,
                correlationId,
                result.Job);
        }

        AgentError error = result.Error
            ?? new AgentError(AgentErrorCode.InternalError, "The agent did not produce a valid result.");
        return AgentJson.CreateError(correlationId, error.Code, error.Message);
    }

    private static AgentResponseEnvelope InvalidPayload(string correlationId)
    {
        return AgentJson.CreateError(
            correlationId,
            AgentErrorCode.InvalidMessage,
            "The payload does not match the declared message type.");
    }

    private static string NormalizeCorrelationId(string correlationId)
    {
        if (string.IsNullOrWhiteSpace(correlationId) || correlationId.Length > 128)
        {
            return "unavailable";
        }

        return correlationId.All(character =>
                char.IsAsciiLetterOrDigit(character) ||
                character is '.' or '-' or '_' or ':')
            ? correlationId
            : "unavailable";
    }
}
