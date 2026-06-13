using ITSupportNative.Catalog.Application;
using ITSupportNative.Catalog.Fixtures;
using ITSupportNative.Contracts.ControlPlane;
using ITSupportNative.Conversation.Application;
using ITSupportNative.Desktop.ControlPlane;
using ITSupportNative.Desktop.ViewModels;

namespace ITSupportNative.WindowsUi.Tests;

public sealed class AssistantViewModelTests
{
    [Fact]
    public void QueryCommandKeepsRequestEmpty()
    {
        AssistantViewModel viewModel = CreateViewModel();

        viewModel.QueryApprovedCommand.Execute(null);

        Assert.Equal("Consulta", viewModel.StateLabel);
        Assert.Contains("No se creó ninguna solicitud", viewModel.Response);
        Assert.Equal("Sin solicitud creada", viewModel.RequestReference);
    }

    [Fact]
    public async Task ApprovedRequestRequiresContinueAndConfirm()
    {
        AssistantViewModel viewModel = CreateViewModel();

        viewModel.RequestApprovedCommand.Execute(null);
        Assert.Equal("Propuesta", viewModel.StateLabel);
        Assert.True(viewModel.ContinueCommand.CanExecute(null));
        Assert.False(viewModel.ConfirmCommand.CanExecute(null));

        viewModel.ContinueCommand.Execute(null);
        Assert.Equal("Confirmación requerida", viewModel.StateLabel);
        Assert.True(viewModel.ConfirmCommand.CanExecute(null));

        await viewModel.ConfirmCommand.ExecuteAsync(null);
        Assert.Equal("Solicitud creada", viewModel.StateLabel);
        Assert.StartsWith("Referencia sintética: SYN-", viewModel.RequestReference);
        Assert.False(viewModel.ConfirmCommand.CanExecute(null));
    }

    [Fact]
    public async Task EnabledControlPlanePersistsConfirmedRequest()
    {
        var client = new RecordingControlPlaneClient();
        AssistantViewModel viewModel = CreateViewModel(client);

        viewModel.RequestApprovedCommand.Execute(null);
        viewModel.ContinueCommand.Execute(null);
        await viewModel.ConfirmCommand.ExecuteAsync(null);

        Assert.Equal("secure-transfer", client.ProductId);
        Assert.StartsWith("Referencia local: REQ-", viewModel.RequestReference);
        Assert.Contains("solicitud local", viewModel.Response);
    }

    [Fact]
    public void ProhibitedRequestShowsAlternativeWithoutRequest()
    {
        AssistantViewModel viewModel = CreateViewModel();

        viewModel.RequestProhibitedCommand.Execute(null);

        Assert.Equal("Consulta", viewModel.StateLabel);
        Assert.Contains("Secure Transfer", viewModel.Response);
        Assert.Equal("Sin solicitud creada", viewModel.RequestReference);
    }

    private static AssistantViewModel CreateViewModel(
        IControlPlaneRequestClient? controlPlane = null)
    {
        var catalogDecisions = new CatalogDecisionService(SyntheticCatalog.Products);
        return new AssistantViewModel(
            new ConversationService(catalogDecisions),
            controlPlane ?? new DisabledControlPlaneRequestClient());
    }

    private sealed class RecordingControlPlaneClient : IControlPlaneRequestClient
    {
        public string? ProductId { get; private set; }

        public Task<CreateSoftwareInstallationData?>
            CreateSoftwareInstallationAsync(
                string idempotencyKey,
                string productId,
                string productVersion,
                CancellationToken cancellationToken)
        {
            ProductId = productId;
            var request = new ControlPlaneSupportRequest(
                Guid.NewGuid().ToString(),
                $"REQ-{Guid.NewGuid()}",
                "desktop-test-correlation",
                "confirmed",
                "local-device-001",
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
            string requestId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<ControlPlaneSupportRequest?>(null);
        }
    }
}
