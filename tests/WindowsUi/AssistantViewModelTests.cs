using ITSupportNative.Catalog.Application;
using ITSupportNative.Catalog.Fixtures;
using ITSupportNative.Conversation.Application;
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
    public void ApprovedRequestRequiresContinueAndConfirm()
    {
        AssistantViewModel viewModel = CreateViewModel();

        viewModel.RequestApprovedCommand.Execute(null);
        Assert.Equal("Propuesta", viewModel.StateLabel);
        Assert.True(viewModel.ContinueCommand.CanExecute(null));
        Assert.False(viewModel.ConfirmCommand.CanExecute(null));

        viewModel.ContinueCommand.Execute(null);
        Assert.Equal("Confirmación requerida", viewModel.StateLabel);
        Assert.True(viewModel.ConfirmCommand.CanExecute(null));

        viewModel.ConfirmCommand.Execute(null);
        Assert.Equal("Solicitud creada", viewModel.StateLabel);
        Assert.StartsWith("Referencia sintética: SYN-", viewModel.RequestReference);
        Assert.False(viewModel.ConfirmCommand.CanExecute(null));
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

    private static AssistantViewModel CreateViewModel()
    {
        var catalogDecisions = new CatalogDecisionService(SyntheticCatalog.Products);
        return new AssistantViewModel(new ConversationService(catalogDecisions));
    }
}
