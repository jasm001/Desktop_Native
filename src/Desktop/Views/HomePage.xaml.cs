using ITSupportNative.Desktop.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace ITSupportNative.Desktop.Views;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        ViewModel = App.GetService<HomeViewModel>();
        InitializeComponent();
    }

    public HomeViewModel ViewModel { get; }
}
