using System;
using AsistenciaApp.Activation;
using AsistenciaApp.Contracts.Services;
using AsistenciaApp.Core.Contracts.Services;
using AsistenciaApp.Core.Models;
using AsistenciaApp.Core.Services;
using AsistenciaApp.Helpers;
using AsistenciaApp.Models;
using AsistenciaApp.Notifications;
using AsistenciaApp.Services;
using AsistenciaApp.ViewModels;
using AsistenciaApp.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

namespace AsistenciaApp;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar { get; set; }

    public App()
    {
        InitializeComponent();

        Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "es-ES";

        var dbFilePath = Path.Combine(AppContext.BaseDirectory, "DB_ASSISTANCE.db");

        System.Diagnostics.Debug.WriteLine($"DB path: {dbFilePath}");

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // DbContext - add both (optional) AddDbContext and AddDbContextFactory.
            // Prefer using IDbContextFactory<T> from singletons to create short-lived contexts.
            services.AddDbContext<AssistanceDbContext>(options =>
                options.UseSqlite($"Data Source={dbFilePath}"));

            services.AddDbContextFactory<AssistanceDbContext>(options =>
                options.UseSqlite($"Data Source={dbFilePath}"));

            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers
            services.AddTransient<IActivationHandler, AppNotificationActivationHandler>();

            // Services
            services.AddSingleton<IAppNotificationService, AppNotificationService>();
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddSingleton<IDataService, DataService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();


            // Views and ViewModels
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<EstudiantesDataGridViewModel>();
            services.AddTransient<RestorePasswordPage>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<LoginPage>();
            services.AddTransient<AgregarCalendarioPage>();
            services.AddTransient<AgregarAsistenciaViewModel>();
            services.AddTransient<ConfigurationPage>();
            services.AddTransient<ConfigurationViewModel>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));

            // Import Excel
            services.AddSingleton<ImportExcelViewModel>();
            services.AddSingleton<ShellViewModel>();

            // Content Dialog
            services.AddSingleton<ContentDialogService>();
        }).
        Build();

        App.GetService<IAppNotificationService>().Initialize();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
        Console.WriteLine(e.Exception.Message);
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        var rootFrame = new Microsoft.UI.Xaml.Controls.Frame();
        MainWindow.Content = rootFrame;

        var authenticationService = App.GetService<IAuthenticationService>();
        if (!authenticationService.IsUserAuthenticated())
        {
            rootFrame.Navigate(typeof(LoginPage));
        }
        else
        {
            rootFrame.Navigate(typeof(MainPage));
        }

        MainWindow.Activate();

        App.GetService<IAppNotificationService>().Show(string.Format("AppNotificationSamplePayload".GetLocalized(), AppContext.BaseDirectory));

        await App.GetService<IActivationService>().ActivateAsync(args);
    }
}
