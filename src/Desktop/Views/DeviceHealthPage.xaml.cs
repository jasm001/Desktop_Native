using ITSupportNative.Desktop.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace ITSupportNative.Desktop.Views;

public sealed partial class DeviceHealthPage : Page
{
    public DeviceHealthPage()
    {
        ViewModel = App.GetService<DeviceHealthViewModel>();
        InitializeComponent();
    }

    public DeviceHealthViewModel ViewModel { get; }
}
