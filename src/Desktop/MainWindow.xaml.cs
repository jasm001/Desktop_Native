using ITSupportNative.Desktop.Navigation;
using ITSupportNative.Desktop.ViewModels;
using ITSupportNative.Desktop.Views;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WinRT.Interop;

namespace ITSupportNative.Desktop;

public sealed partial class MainWindow : Window
{
    private readonly ShellViewModel _viewModel;

    public MainWindow(ShellViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();

        RootGrid.RequestedTheme = ElementTheme.Light;
        SystemBackdrop = new MicaBackdrop();
        ResizeWindow();
        ShellNavigation.SelectedItem = HomeNavigationItem;
        Navigate(ShellRouteCatalog.Home);
    }

    private void OnNavigationSelectionChanged(
        NavigationView sender,
        NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItemContainer?.Tag is string route)
        {
            Navigate(route);
        }
    }

    private void OnNavigationDisplayModeChanged(
        NavigationView sender,
        NavigationViewDisplayModeChangedEventArgs args)
    {
        DevelopmentFooter.Visibility = args.DisplayMode == NavigationViewDisplayMode.Expanded
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void OnCommandHeaderSizeChanged(object sender, SizeChangedEventArgs args)
    {
        bool isCompact = args.NewSize.Width < 900;
        GlobalSearch.Visibility = isCompact ? Visibility.Collapsed : Visibility.Visible;
        EnvironmentStatus.Visibility = isCompact ? Visibility.Collapsed : Visibility.Visible;
        SearchColumn.Width = isCompact
            ? new GridLength(0)
            : new GridLength(320);
        CommandHeader.Padding = isCompact
            ? new Thickness(16, 12, 16, 12)
            : new Thickness(24, 16, 24, 16);
    }

    private void OnThemeToggled(object sender, RoutedEventArgs args)
    {
        _viewModel.IsDarkTheme = ThemeToggle.IsOn;
        RootGrid.RequestedTheme = _viewModel.IsDarkTheme
            ? ElementTheme.Dark
            : ElementTheme.Light;
    }

    private void Navigate(string route)
    {
        Type pageType = route switch
        {
            ShellRouteCatalog.Home => typeof(HomePage),
            ShellRouteCatalog.Catalog => typeof(CatalogPage),
            ShellRouteCatalog.Assistant => typeof(AssistantPage),
            ShellRouteCatalog.Requests => typeof(RequestsPage),
            ShellRouteCatalog.DeviceHealth => typeof(DeviceHealthPage),
            _ => typeof(HomePage),
        };

        _viewModel.CurrentRoute = route;

        if (ContentFrame.CurrentSourcePageType != pageType)
        {
            ContentFrame.Navigate(pageType);
        }
    }

    private void ResizeWindow()
    {
        nint windowHandle = WindowNative.GetWindowHandle(this);
        WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
        AppWindow.GetFromWindowId(windowId).Resize(new Windows.Graphics.SizeInt32(1280, 820));
    }
}
