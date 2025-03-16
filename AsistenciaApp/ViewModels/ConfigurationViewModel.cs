using System.Threading.Tasks;
using System.Windows.Input;
using AsistenciaApp.Core.Models;
using AsistenciaApp.Core.Contracts.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Windows.Foundation.Diagnostics;
using System.Diagnostics;

namespace AsistenciaApp.ViewModels;

public partial class ConfigurationViewModel : ObservableRecipient
{
    private readonly IDataService _dataService;
    private readonly string _folderPath = "Settings";
    private readonly string _fileName = "CentroEducativo.json";

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

    public ConfigurationViewModel(IDataService dataService)
    {
        _dataService = dataService;

        // Inicializar comandos
        SaveCommand = new AsyncRelayCommand(SaveCentroEducativoAsync);
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

    private async Task SaveCentroEducativoAsync()
    {
        try
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string folderPath = Path.Combine(desktopPath, "App", "AsistenciaApp", "Settings");

            Debug.WriteLine($"Ruta de la carpeta: {folderPath}");

            // Asegurarse de que la carpeta 'Settings' existe, si no, crearla
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, _fileName);
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
