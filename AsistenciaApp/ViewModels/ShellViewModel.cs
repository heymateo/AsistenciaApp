using System.Windows.Input;

using AsistenciaApp.Contracts.Services;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Navigation;

namespace AsistenciaApp.ViewModels;

public partial class ShellViewModel : ObservableRecipient
{
    [ObservableProperty]
    private bool isBackEnabled;

    public ICommand MenuFileExitCommand
    {
        get;
    }

    public ICommand MenuSettingsCommand
    {
        get;
    }

    public ICommand MenuViewsStudentsCommand
    {
        get;
    }

    public ICommand MenuViewsMainCommand
    {
        get;
    }

    public ICommand MenuViewsAssistancePageCommand
    {
        get;
    }

    public ICommand MenuViewsCreateAssistancePageCommand
    {
        get;
    }

    public INavigationService NavigationService
    {
        get;
    }

    public ShellViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;

        MenuFileExitCommand = new RelayCommand(OnMenuFileExit);
        MenuSettingsCommand = new RelayCommand(OnMenuSettings);
        MenuViewsAssistancePageCommand = new RelayCommand(OnMenuViewsAssistance);
        MenuViewsStudentsCommand = new RelayCommand(OnMenuViewsStudents);
        MenuViewsMainCommand = new RelayCommand(OnMenuViewsMain);
        MenuViewsCreateAssistancePageCommand = new RelayCommand(OnMenuCreateAssistance);
    }

    private void OnNavigated(object sender, NavigationEventArgs e) => IsBackEnabled = NavigationService.CanGoBack;

    private void OnMenuFileExit() => Application.Current.Exit();

    private void OnMenuSettings() => NavigationService.NavigateTo(typeof(SettingsViewModel).FullName!);

    private void OnMenuViewsAssistance() => NavigationService.NavigateTo(typeof(AsistenciaViewModel).FullName!);

    private void OnMenuViewsStudents() => NavigationService.NavigateTo(typeof(EstudiantesDataGridViewModel).FullName!);

    private void OnMenuViewsMain() => NavigationService.NavigateTo(typeof(MainViewModel).FullName!);

    private void OnMenuCreateAssistance() => NavigationService.NavigateTo(typeof(CreateAsistenciaViewModel).FullName!);
}
