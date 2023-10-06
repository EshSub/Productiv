using Microsoft.UI.Xaml.Controls;

using ProductivWinUINav.ViewModels;

namespace ProductivWinUINav.Views;

public sealed partial class HomePage : Page
{
    public HomeViewModel ViewModel
    {
        get;
    }

    public HomePage()
    {
        ViewModel = App.GetService<HomeViewModel>();
        InitializeComponent();
    }

    private void ToggleButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    private void ToggleButton_Click_1(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }

    private void RadioButton_Checked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }
}
