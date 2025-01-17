using System.ComponentModel;
using System.Windows.Input;
using AsistenciaApp.Contracts.Services;
using AsistenciaApp.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AsistenciaApp.ViewModels;

public class LoginViewModel : ObservableObject
{
    private readonly IAuthenticationService _authenticationService;
    private readonly INavigationService _navigationService;

    private string _username;
    private string _password;
    private string _errorMessage;

    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged(nameof(ErrorMessage));
        }
    }

    public ICommand LoginCommand
    {
        get;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public LoginViewModel(IAuthenticationService authenticationService, INavigationService navigationService)
    {
        _authenticationService = authenticationService;
        _navigationService = navigationService;

        LoginCommand = new RelayCommand(ExecuteLogin);
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void ExecuteLogin()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Por favor ingresa un usuario y una contraseña.";
            return;
        }

        try
        {
            // Autenticar usuario
            bool isAuthenticated = _authenticationService.AuthenticateUser(Username, Password);

            if (isAuthenticated)
            {
                // Navegar a MainPage si la autenticación es exitosa
                var shellView = App.GetService<ShellPage>();
                App.MainWindow.Content = shellView;
            }
            else
            {
                ErrorMessage = "Usuario o contraseña incorrectos.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ocurrió un error: {ex.Message}";
        }
    }
}
