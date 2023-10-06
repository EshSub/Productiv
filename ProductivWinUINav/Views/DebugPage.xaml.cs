using Microsoft.UI.Xaml.Controls;

using ProductivWinUINav.ViewModels;

namespace ProductivWinUINav.Views;

public sealed partial class DebugPage : Page
{
    public DebugViewModel ViewModel
    {
        get;
    }

    public DebugPage()
    {
        ViewModel = App.GetService<DebugViewModel>();
        InitializeComponent();
    }
}
