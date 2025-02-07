using Microsoft.UI.Xaml.Controls;
using AsistenciaApp.ViewModels;

namespace AsistenciaApp.Views
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent(); 
            DataContext = App.GetService<LoginViewModel>();
        }

    }
}
