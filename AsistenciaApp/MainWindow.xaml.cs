using AsistenciaApp.Helpers;
using AsistenciaApp.Services;
using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;
using WinRT;
using CommunityToolkit.WinUI;

namespace AsistenciaApp;

public sealed partial class MainWindow : WindowEx
{
    private Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue;
    private UISettings settings;
    private AppWindow _appWindow;
    private DesktopAcrylicController _acrylicController;
    private SystemBackdropConfiguration _backdropConfiguration;

    public MainWindow()
    {
        InitializeComponent();

        TrySetAcrylicBackdrop();

        // Configuración inicial
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        Content = null;
        Title = CommunityToolkit.WinUI.StringExtensions.GetLocalized("AppDisplayName");


        // Obtener AppWindow para modificar propiedades de la ventana
        _appWindow = GetAppWindowForCurrentWindow();

        // Configurar la ventana en pantalla completa
        _appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);

        // Tema del sistema
        dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        settings = new UISettings();
        settings.ColorValuesChanged += Settings_ColorValuesChanged; // no se puede usar FrameworkElement.ActualThemeChanged

        this.Activated += OnWindowActivated;
        this.Closed += OnWindowClosed;
    }

    private void TrySetAcrylicBackdrop()
    {
        if (DesktopAcrylicController.IsSupported())
        {
            _backdropConfiguration = new SystemBackdropConfiguration
            {
                IsInputActive = true,
                Theme = SystemBackdropTheme.Default
            };

            _acrylicController = new DesktopAcrylicController();

            // Use WindowEx's SystemBackdrop support
            var compositorTarget = this.As<ICompositionSupportsSystemBackdrop>();

            if (compositorTarget != null)
            {
                _acrylicController.AddSystemBackdropTarget(compositorTarget);
                _acrylicController.SetSystemBackdropConfiguration(_backdropConfiguration);
            }
        }
    }

    private void OnWindowActivated(object sender, WindowActivatedEventArgs e)
    {
        if (this.Content != null)
        {
            var dialogService = App.GetService<ContentDialogService>();
            dialogService.Initialize(this.Content.XamlRoot);
        }
    }

    private void OnWindowClosed(object sender, WindowEventArgs e)
    {
        this.Activated -= OnWindowActivated; // Desuscribirse del evento para evitar errores
    }

    // Método para obtener el AppWindow de la ventana actual
    private AppWindow GetAppWindowForCurrentWindow()
    {
        IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(myWndId);
    }

    // this handles updating the caption button colors correctly when indows system theme is changed
    // while the app is open
    private void Settings_ColorValuesChanged(UISettings sender, object args)
    {
        // This calls comes off-thread, hence we will need to dispatch it to current app's thread
        dispatcherQueue.TryEnqueue(() =>
        {
            TitleBarHelper.ApplySystemThemeToCaptionButtons();
        });
    }
}
