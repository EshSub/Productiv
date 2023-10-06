using Microsoft.UI.Xaml.Controls;

using ProductivWinUINav.ViewModels;

namespace ProductivWinUINav.Views;

public sealed partial class AboutPage : Page
{
    public AboutViewModel ViewModel
    {
        get;
    }

    public AboutPage()
    {
        ViewModel = App.GetService<AboutViewModel>();
        InitializeComponent();
    }
}
