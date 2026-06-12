using ITSupportNative.Contracts.Agent;

namespace ITSupportNative.ContractTests;

public sealed class AgentContractTests
{
    [Fact]
    public void StartJobRoundTripsThroughVersionedEnvelope()
    {
        var command = new StartAgentJobRequest(
            "request-1",
            "request-1:secure-transfer:6.5",
            "software.install.simulated.v1",
            "secure-transfer",
            "6.5");
        AgentRequestEnvelope envelope = AgentJson.CreateRequest(
            AgentMessageTypes.StartJob,
            "correlation-1",
            command);

        byte[] json = AgentJson.Serialize(envelope);
        AgentRequestEnvelope? restored = AgentJson.Deserialize<AgentRequestEnvelope>(json);
        StartAgentJobRequest? restoredCommand = restored is null
            ? null
            : AgentJson.DeserializePayload<StartAgentJobRequest>(restored.Payload);

        Assert.NotNull(restored);
        Assert.Equal(AgentProtocol.CurrentVersion, restored.Version);
        Assert.Equal(AgentMessageTypes.StartJob, restored.MessageType);
        Assert.Equal(command, restoredCommand);
    }

    [Fact]
    public void ErrorResponseUsesTypedCode()
    {
        AgentResponseEnvelope response = AgentJson.CreateError(
            "correlation-2",
            AgentErrorCode.UnknownMessage,
            "Unknown message.");

        AgentResponseEnvelope? restored =
            AgentJson.Deserialize<AgentResponseEnvelope>(AgentJson.Serialize(response));

        Assert.NotNull(restored);
        Assert.False(restored.Success);
        Assert.Equal(AgentErrorCode.UnknownMessage, restored.Error?.Code);
    }

    [Fact]
    public void StartJobContractDoesNotExposeFreeFormExecutionFields()
    {
        string[] propertyNames = typeof(StartAgentJobRequest)
            .GetProperties()
            .Select(property => property.Name)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                nameof(StartAgentJobRequest.ActionId),
                nameof(StartAgentJobRequest.IdempotencyKey),
                nameof(StartAgentJobRequest.RequestId),
                nameof(StartAgentJobRequest.TargetId),
                nameof(StartAgentJobRequest.TargetVersion),
            ],
            propertyNames);
        Assert.DoesNotContain(propertyNames, name => name.Contains("Command", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(propertyNames, name => name.Contains("Argument", StringComparison.OrdinalIgnoreCase));
    }
}
