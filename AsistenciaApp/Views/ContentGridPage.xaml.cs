using AsistenciaApp.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace AsistenciaApp.Views;

public sealed partial class ContentGridPage : Page
{
    public ContentGridViewModel ViewModel
    {
        get;
    }

    public ContentGridPage()
    {
        ViewModel = App.GetService<ContentGridViewModel>();
        InitializeComponent();
    }
}
