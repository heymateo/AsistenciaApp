using System.Threading.Tasks;
using AsistenciaApp.Core.Contracts.Services;
using AsistenciaApp.Core.Models;
using AsistenciaApp.Models;
using AsistenciaApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;

public class MainViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    private Centro_Educativo _centroEducativo;
    public Centro_Educativo Centro_Educativo
    {
        get => _centroEducativo;
        set => SetProperty(ref _centroEducativo, value);
    }

    public MainViewModel(IDataService dataService)
    {
        _dataService = dataService;
        LoadCentroEducativo();
    }

    public async void LoadCentroEducativo()
    {
        Centro_Educativo = await _dataService.GetCentroEducativoAsync();
    }
}
