using System.Text;
using System.Text.Json;
using ITSupportNative.Catalog.Application;
using ITSupportNative.Catalog.Fixtures;
using ITSupportNative.Contracts.ControlPlane;
using ITSupportNative.Contracts.Conversation;
using ITSupportNative.Conversation.Application;
using ITSupportNative.Conversation.Channels;
using ITSupportNative.Conversation.Domain;
using ITSupportNative.Desktop.Assistant;
using ITSupportNative.Desktop.ControlPlane;
using ITSupportNative.Desktop.Conversation;
using ITSupportNative.Desktop.ViewModels;

namespace ITSupportNative.WindowsUi.Tests;

public sealed class AssistantViewModelTests
{
    [Fact]
    public async Task QueryCommandKeepsRequestEmpty()
    {
        AssistantViewModel viewModel = CreateViewModel();

        await viewModel.QueryApprovedCommand.ExecuteAsync(null);

        Assert.Equal("Consulta", viewModel.StateLabel);
        Assert.Contains("No se creó ninguna solicitud", viewModel.Response);
        Assert.Equal("Sin solicitud creada", viewModel.RequestReference);
        Assert.Equal(3, viewModel.ChatMessages.Count);
        Assert.Equal("Tu", viewModel.ChatMessages[1].Author);
        Assert.Equal("Consulta", viewModel.ChatMessages[2].Author);
    }

    [Fact]
    public void FreeTextIsDisabledWhenHermesIsNotConfigured()
    {
        AssistantViewModel viewModel = CreateViewModel();

        viewModel.DraftMessage = "Hola";

        Assert.False(viewModel.IsFreeTextEnabled);
        Assert.False(viewModel.SendMessageCommand.CanExecute(null));
        Assert.Equal("Hermes local no esta configurado", viewModel.FreeTextPlaceholder);
    }

    [Fact]
    public async Task FreeTextUsesAssistantProviderWithoutCreatingRequest()
    {
        var provider = new RecordingAssistantProvider();
        AssistantViewModel viewModel = CreateViewModel(assistantProvider: provider);

        viewModel.DraftMessage = "Que puedo instalar?";
        await viewModel.SendMessageCommand.ExecuteAsync(null);

        Assert.Equal("Que puedo instalar?", provider.Message);
        Assert.Equal("Hermes local", viewModel.StateLabel);
        Assert.Equal("Respuesta conectada a Hermes.", viewModel.Response);
        Assert.Equal(
            "Fuente: hermes-local; sin solicitud ni accion creada",
            viewModel.RequestReference);
        Assert.Null(viewModel.LastChannelOutput);
        Assert.Equal(3, viewModel.ChatMessages.Count);
        Assert.Equal("Tu", viewModel.ChatMessages[1].Author);
        Assert.Equal("Que puedo instalar?", viewModel.ChatMessages[1].Body);
        Assert.Equal("Hermes local", viewModel.ChatMessages[2].Author);
        Assert.Equal("Respuesta conectada a Hermes.", viewModel.ChatMessages[2].Body);
    }

    [Fact]
    public void HermesOptionsRequireExplicitLocalConfiguration()
    {
        Dictionary<string, string?> environment = new(StringComparer.Ordinal)
        {
            [HermesAssistantOptions.EnabledVariable] = "true",
            [HermesAssistantOptions.ApiKeyVariable] = "test-key",
        };

        HermesAssistantOptions? options =
            HermesAssistantOptions.FromEnvironment(environment);

        Assert.NotNull(options);
        Assert.Equal("http://127.0.0.1:8765/v1/", options.BaseEndpoint.AbsoluteUri);
        Assert.Equal("it-support", options.Model);
        Assert.Equal(TimeSpan.FromSeconds(120), options.Timeout);
    }

    [Fact]
    public void HermesOptionsRejectNonLoopbackEndpoint()
    {
        Dictionary<string, string?> environment = new(StringComparer.Ordinal)
        {
            [HermesAssistantOptions.EnabledVariable] = "true",
            [HermesAssistantOptions.ApiKeyVariable] = "test-key",
            [HermesAssistantOptions.BaseEndpointVariable] =
                "https://example.com/v1",
        };

        Assert.Null(HermesAssistantOptions.FromEnvironment(environment));
    }

    [Fact]
    public async Task ApprovedRequestRequiresContinueAndConfirm()
    {
        AssistantViewModel viewModel = CreateViewModel();

        await viewModel.RequestApprovedCommand.ExecuteAsync(null);
        Assert.Equal("Propuesta", viewModel.StateLabel);
        Assert.True(viewModel.ContinueCommand.CanExecute(null));
        Assert.False(viewModel.ConfirmCommand.CanExecute(null));

        await viewModel.ContinueCommand.ExecuteAsync(null);
        Assert.Equal("Confirmación requerida", viewModel.StateLabel);
        Assert.True(viewModel.ConfirmCommand.CanExecute(null));

        await viewModel.ConfirmCommand.ExecuteAsync(null);
        Assert.Equal("Solicitud creada", viewModel.StateLabel);
        Assert.StartsWith(
            "Referencia sintética: SYN-",
            viewModel.RequestReference);
        Assert.False(viewModel.ConfirmCommand.CanExecute(null));
        Assert.Equal(7, viewModel.ChatMessages.Count);
    }

    [Fact]
    public async Task EnabledControlPlanePersistsConfirmedRequest()
    {
        var client = new RecordingControlPlaneClient();
        AssistantViewModel viewModel = CreateViewModel(client);

        await viewModel.RequestApprovedCommand.ExecuteAsync(null);
        await viewModel.ContinueCommand.ExecuteAsync(null);
        await viewModel.ConfirmCommand.ExecuteAsync(null);

        Assert.Equal("secure-transfer", client.ProductId);
        Assert.StartsWith("Referencia local: REQ-", viewModel.RequestReference);
        Assert.Contains("solicitud local", viewModel.Response);
    }

    [Fact]
    public async Task ProhibitedRequestShowsAlternativeWithoutRequest()
    {
        AssistantViewModel viewModel = CreateViewModel();

        await viewModel.RequestProhibitedCommand.ExecuteAsync(null);

        Assert.Equal("Consulta", viewModel.StateLabel);
        Assert.Contains("Secure Transfer", viewModel.Response);
        Assert.Equal("Sin solicitud creada", viewModel.RequestReference);
    }

    [Fact]
    public async Task RecordedTeamsAndWinUiProduceTheSameObservableDecision()
    {
        using JsonDocument fixture = LoadFixture();
        var recordedChannel = new RecordedTeamsConversationChannel();
        ConversationChannelService teamsService = CreateChannelService();
        ConversationChannelService winUiService = CreateChannelService();
        ConversationSession teamsSession =
            ConversationSession.Start("parity-session-1");
        ConversationSession winUiSession =
            ConversationSession.Start("parity-session-1");

        foreach (string fixtureName in new[]
        {
            "software-request",
            "proposal-continue",
            "request-confirm",
        })
        {
            JsonElement fixtureInput = FindFixtureInput(fixture, fixtureName);
            ConversationChannelInput teamsInput = recordedChannel.Read(
                Encoding.UTF8.GetBytes(fixtureInput.GetRawText()));
            ConversationChannelInput winUiInput =
                WinUiConversationInputFactory.Create(
                    winUiSession,
                    teamsInput.MessageId,
                    teamsInput.CorrelationId,
                    teamsInput.Action,
                    teamsInput.ProductReference,
                    teamsInput.RequestId);

            Assert.Equal(teamsInput, winUiInput);

            ConversationChannelTurn teamsTurn =
                await teamsService.HandleAsync(
                    teamsSession,
                    teamsInput,
                    CancellationToken.None);
            ConversationChannelTurn winUiTurn =
                await winUiService.HandleAsync(
                    winUiSession,
                    winUiInput,
                    CancellationToken.None);

            Assert.Equal(
                recordedChannel.Write(teamsTurn.Output),
                ConversationChannelJson.SerializeOutput(winUiTurn.Output));

            teamsSession = teamsTurn.Session;
            winUiSession = winUiTurn.Session;
        }

        Assert.Equal(3, recordedChannel.Inputs.Count);
        Assert.Equal(3, recordedChannel.Outputs.Count);
        Assert.Equal(ConversationState.RequestCreated, teamsSession.State);
        Assert.Equal(ConversationState.RequestCreated, winUiSession.State);
    }

    private static AssistantViewModel CreateViewModel(
        IControlPlaneRequestClient? controlPlane = null,
        IAssistantProvider? assistantProvider = null)
    {
        return new(
            new ConversationChannelService(
                new ConversationService(
                    new CatalogDecisionService(SyntheticCatalog.Products)),
                controlPlane ?? new DisabledControlPlaneRequestClient()),
            assistantProvider ?? new DisabledAssistantProvider());
    }

    private static ConversationChannelService CreateChannelService()
    {
        return new(
            new ConversationService(
                new CatalogDecisionService(SyntheticCatalog.Products)),
            new DisabledControlPlaneRequestClient());
    }

    private static JsonElement FindFixtureInput(
        JsonDocument fixture,
        string name)
    {
        return fixture.RootElement
            .GetProperty("validInputs")
            .EnumerateArray()
            .Single(
                item => string.Equals(
                    item.GetProperty("name").GetString(),
                    name,
                    StringComparison.Ordinal))
            .GetProperty("input");
    }

    private static JsonDocument LoadFixture()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            string candidate = Path.Combine(
                directory.FullName,
                "tests",
                "Fixtures",
                "conversation-channel-v1.json");
            if (File.Exists(candidate))
            {
                return JsonDocument.Parse(File.ReadAllBytes(candidate));
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException(
            "The shared conversation channel fixture was not found.");
    }

    private sealed class RecordingControlPlaneClient
        : IControlPlaneRequestClient
    {
        public string? ProductId { get; private set; }

        public Task<CreateSoftwareInstallationData?>
            CreateSoftwareInstallationAsync(
                string correlationId,
                string idempotencyKey,
                string deviceId,
                string productId,
                string productVersion,
                CancellationToken cancellationToken)
        {
            ProductId = productId;
            var request = new ControlPlaneSupportRequest(
                Guid.NewGuid().ToString(),
                $"REQ-{Guid.NewGuid()}",
                correlationId,
                "confirmed",
                deviceId,
                productId,
                productVersion,
                "software.install.simulated.v1",
                DateTimeOffset.UtcNow,
                new(
                    Guid.NewGuid().ToString(),
                    "queued",
                    []));
            return Task.FromResult<CreateSoftwareInstallationData?>(
                new(request, Replayed: false));
        }

        public Task<ControlPlaneSupportRequest?> GetSupportRequestAsync(
            string correlationId,
            string requestId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<ControlPlaneSupportRequest?>(null);
        }

        public Task<ControlPlaneBotCase?> GetBotCaseAsync(
            string correlationId,
            string requestId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<ControlPlaneBotCase?>(null);
        }
    }

    private sealed class RecordingAssistantProvider : IAssistantProvider
    {
        public bool IsAvailable => true;

        public string? Message { get; private set; }

        public Task<AssistantProviderReply> GetResponseAsync(
            string message,
            CancellationToken cancellationToken)
        {
            Message = message;
            return Task.FromResult(
                new AssistantProviderReply(
                    "Respuesta conectada a Hermes.",
                    "hermes-local"));
        }
    }
}
