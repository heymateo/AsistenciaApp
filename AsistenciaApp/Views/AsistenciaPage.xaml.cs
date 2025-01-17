using AsistenciaApp.Core.Models;
using AsistenciaApp.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace AsistenciaApp.Views;
public sealed partial class AsistenciaPage : Page
{
    public AsistenciaViewModel ViewModel
    {
        get;
    }

    public AsistenciaPage()
    {
        InitializeComponent();
        ViewModel = new AsistenciaViewModel();
        DataContext = ViewModel;
    }


}
