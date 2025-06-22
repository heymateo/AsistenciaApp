using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using AsistenciaApp.Core.Contracts.Services;
using AsistenciaApp.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Windows.Foundation.Diagnostics;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace AsistenciaApp.ViewModels;

public partial class ConfigurationViewModel : ObservableRecipient
{
    private readonly IDataService _dataService;
    private readonly string _folderPath = "Settings";
    private readonly string _fileName = "CentroEducativo.json";
    private readonly AssistanceDbContext _dbContext;

    [ObservableProperty]
    private Centro_Educativo _centroEducativo;

    public ICommand SaveCommand
    {
        get;
    }
    public ICommand ResetCommand
    {
        get;
    }

    public ICommand UploadLogoCommand
    {
        get;
    }

    public ConfigurationViewModel(IDataService dataService, AssistanceDbContext dbContext)
    {
        _dataService = dataService;
        _dbContext = dbContext;

        // Inicializar comandos
        SaveCommand = new AsyncRelayCommand(SaveCentroEducativoAsync);
        UploadLogoCommand = new AsyncRelayCommand(UploadLogoAsync);
        ResetCommand = new RelayCommand(ResetCentroEducativo);

        // Cargar datos al iniciar
        LoadCentroEducativo();
    }

    private async Task LoadCentroEducativo()
    {
        try
        {
            _centroEducativo = await _dataService.GetCentroEducativoAsync();

            if (_centroEducativo == null)
            {
                _centroEducativo = new Centro_Educativo();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error al cargar el Centro Educativo: {ex.Message}");
            _centroEducativo = new Centro_Educativo();
        }
    }

    private async Task UploadLogoAsync()
    {
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".png");
        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");

        // Necesario en WinUI 3
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();

        if (file != null)
        {
            try
            {
                using var stream = await file.OpenStreamForReadAsync(); // Usar System.IO.Stream directamente
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);

                CentroEducativo.Logo = memoryStream.ToArray(); // Asignar el byte[] directamente// Guardar en base de datos
                var entity = await _dbContext.Centro_Educativo
                    .FirstOrDefaultAsync(c => c.Id_Centro == CentroEducativo.Id_Centro);

                if (entity != null)
                {
                    entity.Logo = CentroEducativo.Logo;
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al subir logo: {ex.Message}");
            }
        }
    }


    private async Task SaveCentroEducativoAsync()
    {
        try
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var folderPath = Path.Combine(desktopPath, "App", "AsistenciaApp", "Settings");

            Debug.WriteLine($"Ruta de la carpeta: {folderPath}");

            // Asegurarse de que la carpeta 'Settings' existe, si no, crearla
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, _fileName);
            Debug.WriteLine($"Ruta del archivo a guardar: {filePath}");


            await _dataService.SaveCentroEducativoAsync(folderPath, _fileName, _centroEducativo);
        }
        catch(Exception ex)
        {
            Debug.WriteLine($"Error al guardar los datos: {ex.Message}");
            Debug.WriteLine($"Detalles: {ex.ToString()}");
        }
        
    }
    private void ResetCentroEducativo()
    {
        LoadCentroEducativo();
    }
}
