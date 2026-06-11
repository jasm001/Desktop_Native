using ITSupportNative.Desktop.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace ITSupportNative.Desktop.Views;

public sealed partial class CatalogPage : Page
{
    public CatalogPage()
    {
        ViewModel = App.GetService<CatalogViewModel>();
        InitializeComponent();
    }

    public CatalogViewModel ViewModel { get; }
}
