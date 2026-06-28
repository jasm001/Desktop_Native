using ITSupportNative.Catalog.Application;
using ITSupportNative.Catalog.Domain;
using ITSupportNative.Catalog.Fixtures;
using ITSupportNative.Conversation.Application;
using ITSupportNative.Desktop.Assistant;
using ITSupportNative.Desktop.ControlPlane;
using ITSupportNative.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

namespace ITSupportNative.Desktop;

public partial class App : Application
{
    private Window? _window;

    public App()
    {
        InitializeComponent();

        Services = new ServiceCollection()
            .AddSingleton<IReadOnlyList<SoftwareProduct>>(SyntheticCatalog.Products)
            .AddSingleton<CatalogSearchService>()
            .AddSingleton<CatalogDecisionService>()
            .AddSingleton<ConversationService>()
            .AddSingleton<IControlPlaneRequestClient>(_ =>
            {
                ControlPlaneClientOptions? options =
                    ControlPlaneClientOptions.FromEnvironment();
                return options is null
                    ? new DisabledControlPlaneRequestClient()
                    : new HttpControlPlaneRequestClient(new HttpClient(), options);
            })
            .AddSingleton<IConversationControlPlane>(
                provider => provider.GetRequiredService<IControlPlaneRequestClient>())
            .AddSingleton<IAssistantProvider>(_ =>
            {
                HermesAssistantOptions? options =
                    HermesAssistantOptions.FromEnvironment();
                return options is null
                    ? new DisabledAssistantProvider()
                    : new HermesAssistantProvider(new HttpClient(), options);
            })
            .AddSingleton<ConversationChannelService>()
            .AddSingleton<ShellViewModel>()
            .AddSingleton<HomeViewModel>()
            .AddSingleton<CatalogViewModel>()
            .AddSingleton<AssistantViewModel>()
            .AddSingleton<RequestsViewModel>()
            .AddSingleton<DeviceHealthViewModel>()
            .AddSingleton<MainWindow>()
            .BuildServiceProvider();
    }

    public IServiceProvider Services { get; }

    public static App CurrentApp => (App)Current;

    public static T GetService<T>()
        where T : notnull
    {
        return CurrentApp.Services.GetRequiredService<T>();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = GetService<MainWindow>();
        _window.Activate();
    }
}
