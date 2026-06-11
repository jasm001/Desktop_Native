using ITSupportNative.Desktop.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace ITSupportNative.Desktop.Views;

public sealed partial class RequestsPage : Page
{
    public RequestsPage()
    {
        ViewModel = App.GetService<RequestsViewModel>();
        InitializeComponent();
    }

    public RequestsViewModel ViewModel { get; }
}
