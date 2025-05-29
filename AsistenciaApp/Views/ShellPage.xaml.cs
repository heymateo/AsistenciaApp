using AsistenciaApp.Contracts.Services;
using AsistenciaApp.Helpers;
using AsistenciaApp.Services;
using AsistenciaApp.ViewModels;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

using Windows.System;

namespace AsistenciaApp.Views;

public sealed partial class ShellPage : Page
{
    private readonly ContentDialogService _contentDialogService;

    public ShellViewModel ViewModel
    {
        get;
    }

    public ShellPage()
    {
        ViewModel = App.GetService<ShellViewModel>();
        // Obtener el servicio
        var dialogService = App.GetService<ContentDialogService>();

        // Inicializar con el XamlRoot de la ventana
        dialogService.Initialize(this.Content.XamlRoot);
        InitializeComponent();
    }

    public async Task ShowGlobalDialogAsync(string title, string message)
    {
        await _contentDialogService.ShowDialogAsync(title, message);
    }

    public ShellPage(ShellViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();

        ViewModel.NavigationService.Frame = NavigationFrame;

        // TODO: Set the title bar icon by updating /Assets/WindowIcon.ico.
        // A custom title bar is required for full window theme and Mica support.
        // https://docs.microsoft.com/windows/apps/develop/title-bar?tabs=winui3#full-customization
        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        App.MainWindow.Activated += MainWindow_Activated;
        AppTitleBarText.Text = "AppDisplayName".GetLocalized();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Cargar MainPage en el Frame
        NavigationFrame.Navigate(typeof(MainPage));

        TitleBarHelper.UpdateTitleBar(RequestedTheme);

        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));

        ShellMenuBarSettingsButton.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(ShellMenuBarSettingsButton_PointerPressed), true);
        ShellMenuBarSettingsButton.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(ShellMenuBarSettingsButton_PointerReleased), true);
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        App.AppTitlebar = AppTitleBarText as UIElement;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        ShellMenuBarSettingsButton.RemoveHandler(UIElement.PointerPressedEvent, (PointerEventHandler)ShellMenuBarSettingsButton_PointerPressed);
        ShellMenuBarSettingsButton.RemoveHandler(UIElement.PointerReleasedEvent, (PointerEventHandler)ShellMenuBarSettingsButton_PointerReleased);
    }

    private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
    {
        var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

        if (modifiers.HasValue)
        {
            keyboardAccelerator.Modifiers = modifiers.Value;
        }

        keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;

        return keyboardAccelerator;
    }

    private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        var navigationService = App.GetService<INavigationService>();

        var result = navigationService.GoBack();

        args.Handled = result;
    }

    private void ShellMenuBarSettingsButton_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        AnimatedIcon.SetState((UIElement)sender, "PointerOver");
    }

    private void ShellMenuBarSettingsButton_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        AnimatedIcon.SetState((UIElement)sender, "Pressed");
    }

    private void ShellMenuBarSettingsButton_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        AnimatedIcon.SetState((UIElement)sender, "Normal");
    }

    private void ShellMenuBarSettingsButton_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        AnimatedIcon.SetState((UIElement)sender, "Normal");
    }
}
