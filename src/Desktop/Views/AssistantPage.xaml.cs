using ITSupportNative.Desktop.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace ITSupportNative.Desktop.Views;

public sealed partial class AssistantPage : Page
{
    public AssistantPage()
    {
        ViewModel = App.GetService<AssistantViewModel>();
        InitializeComponent();
    }

    public AssistantViewModel ViewModel { get; }
}
