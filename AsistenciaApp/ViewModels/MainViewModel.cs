using System.Threading.Tasks;
using AsistenciaApp.Core.Contracts.Services;
using AsistenciaApp.Core.Models;
using AsistenciaApp.Models;
using AsistenciaApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using System.IO;
using Windows.Storage.Streams;

public class MainViewModel : ObservableObject
{
    private readonly IDataService _dataService;

    private Centro_Educativo _centroEducativo;
    public Centro_Educativo Centro_Educativo
    {
        get => _centroEducativo;
        set
        {
            SetProperty(ref _centroEducativo, value);
            OnPropertyChanged(nameof(LogoImage)); // Notifica al binding
        }
    }

    public MainViewModel(IDataService dataService)
    {
        _dataService = dataService;
        LoadCentroEducativo();
    }

    public BitmapImage? LogoImage
    {
        get
        {
            if (Centro_Educativo?.Logo != null && Centro_Educativo.Logo.Length > 0)
            {
                using var stream = new MemoryStream(Centro_Educativo.Logo);
                var image = new BitmapImage();
                image.SetSource(stream.AsRandomAccessStream());
                return image;
            }
            return null;
        }
    }

    public async void LoadCentroEducativo()
    {
        Centro_Educativo = await _dataService.GetCentroEducativoAsync();
    }
}
