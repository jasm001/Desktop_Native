using CommunityToolkit.Mvvm.ComponentModel;
using ITSupportNative.Desktop.Navigation;

namespace ITSupportNative.Desktop.ViewModels;

public sealed class ShellViewModel : ObservableObject
{
    private string _currentRoute = ShellRouteCatalog.Home;
    private bool _isDarkTheme;

    public string CurrentRoute
    {
        get => _currentRoute;
        set => SetProperty(ref _currentRoute, value);
    }

    public bool IsDarkTheme
    {
        get => _isDarkTheme;
        set => SetProperty(ref _isDarkTheme, value);
    }
}
