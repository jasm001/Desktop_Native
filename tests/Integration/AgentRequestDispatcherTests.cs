using System.Text.Json;
using ITSupportNative.Contracts.Agent;
using ITSupportNative.DeviceAgent.Ipc;
using ITSupportNative.DeviceAgent.Jobs;

namespace ITSupportNative.IntegrationTests;

public sealed class AgentRequestDispatcherTests : IDisposable
{
    private readonly string _testDirectory = Path.Combine(
        Path.GetTempPath(),
        "ITSupportNative.Tests",
        Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task UnknownMessageTypeIsRejected()
    {
        AgentRequestDispatcher dispatcher = CreateDispatcher();
        var request = new AgentRequestEnvelope(
            AgentProtocol.CurrentVersion,
            "agent.shell.execute",
            "correlation-1",
            JsonSerializer.SerializeToElement(new { command = "whoami" }));

        AgentResponseEnvelope response = await dispatcher.DispatchAsync(
            request,
            CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal(AgentErrorCode.UnknownMessage, response.Error?.Code);
    }

    [Fact]
    public async Task UnsupportedVersionIsRejected()
    {
        AgentRequestDispatcher dispatcher = CreateDispatcher();
        AgentRequestEnvelope request = AgentJson.CreateRequest(
            AgentMessageTypes.GetJob,
            "correlation-2",
            new GetAgentJobRequest("job-1")) with
        {
            Version = AgentProtocol.CurrentVersion + 1,
        };

        AgentResponseEnvelope response = await dispatcher.DispatchAsync(
            request,
            CancellationToken.None);

        Assert.False(response.Success);
        Assert.Equal(AgentErrorCode.UnsupportedVersion, response.Error?.Code);
    }

    [Fact]
    public async Task AuthorizedTypedMessageCreatesQueuedJob()
    {
        AgentRequestDispatcher dispatcher = CreateDispatcher();
        AgentRequestEnvelope request = AgentJson.CreateRequest(
            AgentMessageTypes.StartJob,
            "correlation-3",
            new StartAgentJobRequest(
                "request-1",
                "request-1:secure-transfer:6.5",
                "software.install.simulated.v1",
                "secure-transfer",
                "6.5"));

        AgentResponseEnvelope response = await dispatcher.DispatchAsync(
            request,
            CancellationToken.None);
        AgentJobSnapshot? snapshot =
            AgentJson.DeserializePayload<AgentJobSnapshot>(response.Payload);

        Assert.True(response.Success);
        Assert.Equal(AgentMessageTypes.JobSnapshot, response.MessageType);
        Assert.Equal(AgentJobState.Queued, snapshot?.State);
    }

    [Fact]
    public async Task InvalidCorrelationIdIsNotReflected()
    {
        AgentRequestDispatcher dispatcher = CreateDispatcher();
        AgentRequestEnvelope request = AgentJson.CreateRequest(
            AgentMessageTypes.GetJob,
            "invalid correlation\n",
            new GetAgentJobRequest("job-1"));

        AgentResponseEnvelope response = await dispatcher.DispatchAsync(
            request,
            CancellationToken.None);

        Assert.Equal("unavailable", response.CorrelationId);
        Assert.False(response.Success);
        Assert.Equal(AgentErrorCode.JobNotFound, response.Error?.Code);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }

        GC.SuppressFinalize(this);
    }

    private AgentRequestDispatcher CreateDispatcher()
    {
        string stateFile = Path.Combine(_testDirectory, "jobs.db");
        var service = new AgentJobService(
            new SqliteAgentJobStore(stateFile),
            new AgentActionAuthorizationPolicy(),
            TimeProvider.System);
        return new AgentRequestDispatcher(service);
    }
}
