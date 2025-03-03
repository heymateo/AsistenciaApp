using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace AsistenciaApp.Services;

public class ContentDialogService
{
    private Microsoft.UI.Xaml.XamlRoot _xamlRoot;

    // Método para establecer el XamlRoot desde la ventana principal
    public void Initialize(Microsoft.UI.Xaml.XamlRoot xamlRoot)
    {
        _xamlRoot = xamlRoot;
    }

    public async Task ShowDialogAsync(string title, string message)
    {
        if (_xamlRoot == null)
        {
            return; 
        }

        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "Aceptar",
            XamlRoot = _xamlRoot 
        };

        await dialog.ShowAsync();
    }
}
