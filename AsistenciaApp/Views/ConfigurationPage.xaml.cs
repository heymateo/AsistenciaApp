using AsistenciaApp.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace AsistenciaApp.Views;
public sealed partial class ConfigurationPage : Page
{
    public ConfigurationPage()
    {
        this.InitializeComponent();
        DataContext = App.GetService<ConfigurationViewModel>();
    }
}
