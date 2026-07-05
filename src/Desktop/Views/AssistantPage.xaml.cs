using ITSupportNative.Desktop.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace ITSupportNative.Desktop.Views;

public sealed partial class AssistantPage : Page
{
    public AssistantPage()
    {
        ViewModel = App.GetService<AssistantViewModel>();
        InitializeComponent();
        ViewModel.ChatMessages.CollectionChanged += (_, _) =>
        {
            _ = DispatcherQueue.TryEnqueue(
                () => ChatScrollViewer.ChangeView(
                    null,
                    ChatScrollViewer.ScrollableHeight,
                    null));
        };
    }

    public AssistantViewModel ViewModel { get; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "CA1822:Mark members as static",
        Justification = "WinUI x:Bind uses this instance method as a view converter.")]
    public Visibility ToVisibility(bool value)
    {
        return value
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void MessageTextBox_KeyDown(
        object sender,
        KeyRoutedEventArgs args)
    {
        if (args.Key != VirtualKey.Enter
            || !ViewModel.SendMessageCommand.CanExecute(null))
        {
            return;
        }

        args.Handled = true;
        ViewModel.SendMessageCommand.Execute(null);
    }
}
