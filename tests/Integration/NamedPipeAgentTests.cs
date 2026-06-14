using System.IO.Pipes;
using System.Security.Principal;
using ITSupportNative.Contracts.Agent;
using ITSupportNative.DeviceAgent.Ipc;
using ITSupportNative.DeviceAgent.Jobs;

namespace ITSupportNative.IntegrationTests;

public sealed class NamedPipeAgentTests : IDisposable
{
    private readonly string _testDirectory = Path.Combine(
        Path.GetTempPath(),
        "ITSupportNative.Tests",
        Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task CurrentUserPipeExchangesTypedRequestAndResponse()
    {
        string pipeName = $"ITSupportNative.Tests.{Guid.NewGuid():N}";
        AgentRequestDispatcher dispatcher = CreateDispatcher();
        using var timeout = new CancellationTokenSource();
        timeout.CancelAfter(TimeSpan.FromSeconds(10));

        Task serverTask = RunSingleRequestServerAsync(pipeName, dispatcher, timeout.Token);
        await using var client = new NamedPipeClientStream(
            ".",
            pipeName,
            PipeDirection.InOut,
            PipeOptions.Asynchronous,
            TokenImpersonationLevel.Impersonation);
        await client.ConnectAsync(timeout.Token);

        AgentRequestEnvelope request = AgentJson.CreateRequest(
            AgentMessageTypes.StartJob,
            "correlation-pipe",
            new StartAgentJobRequest(
                "request-pipe",
                "request-pipe:secure-transfer:6.5",
                "software.install.simulated.v1",
                "secure-transfer",
                "6.5"));
        await AgentPipeFraming.WriteAsync(
            client,
            AgentJson.Serialize(request),
            timeout.Token);
        byte[] responseBytes = await AgentPipeFraming.ReadAsync(client, timeout.Token);
        AgentResponseEnvelope? response =
            AgentJson.Deserialize<AgentResponseEnvelope>(responseBytes);

        await serverTask;
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal("correlation-pipe", response.CorrelationId);
    }

    [Fact]
    public async Task CurrentUserPipeReturnsTypedDiagnosticSnapshot()
    {
        string pipeName = $"ITSupportNative.Tests.{Guid.NewGuid():N}";
        AgentRequestDispatcher dispatcher = CreateDispatcher();
        using var timeout = new CancellationTokenSource();
        timeout.CancelAfter(TimeSpan.FromSeconds(10));

        Task serverTask = RunSingleRequestServerAsync(pipeName, dispatcher, timeout.Token);
        await using var client = new NamedPipeClientStream(
            ".",
            pipeName,
            PipeDirection.InOut,
            PipeOptions.Asynchronous,
            TokenImpersonationLevel.Impersonation);
        await client.ConnectAsync(timeout.Token);

        AgentRequestEnvelope request = AgentJson.CreateRequest(
            AgentMessageTypes.GetDiagnostics,
            "correlation-diagnostic-pipe",
            DiagnosticTestDoubles.AuthorizedRequest());
        await AgentPipeFraming.WriteAsync(
            client,
            AgentJson.Serialize(request),
            timeout.Token);
        byte[] responseBytes = await AgentPipeFraming.ReadAsync(client, timeout.Token);
        AgentResponseEnvelope? response =
            AgentJson.Deserialize<AgentResponseEnvelope>(responseBytes);
        AgentDiagnosticSnapshot? snapshot = response is null
            ? null
            : AgentJson.DeserializePayload<AgentDiagnosticSnapshot>(response.Payload);

        await serverTask;
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotNull(snapshot);
        Assert.Equal(AgentMessageTypes.DiagnosticSnapshot, response.MessageType);
    }

    [Fact]
    public void DevelopmentServerCanBeCreatedForCurrentUser()
    {
        using NamedPipeServerStream server =
            NamedPipeAgentWorker.CreateServer($"ITSupportNative.Tests.{Guid.NewGuid():N}");

        Assert.False(server.IsConnected);
        Assert.True(OperatingSystem.IsWindows());
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
            new AgentJobExecutionGate(isEnabled: true),
            TimeProvider.System);
        return new AgentRequestDispatcher(
            service,
            DiagnosticTestDoubles.CreateService());
    }

    private static async Task RunSingleRequestServerAsync(
        string pipeName,
        AgentRequestDispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        await using NamedPipeServerStream server = NamedPipeAgentWorker.CreateServer(pipeName);
        await server.WaitForConnectionAsync(cancellationToken);
        byte[] requestBytes = await AgentPipeFraming.ReadAsync(server, cancellationToken);
        AgentRequestEnvelope request =
            AgentJson.Deserialize<AgentRequestEnvelope>(requestBytes)
            ?? throw new InvalidDataException("The test request was invalid.");
        AgentResponseEnvelope response = await dispatcher.DispatchAsync(
            request,
            cancellationToken);
        await AgentPipeFraming.WriteAsync(
            server,
            AgentJson.Serialize(response),
            cancellationToken);
    }
}
