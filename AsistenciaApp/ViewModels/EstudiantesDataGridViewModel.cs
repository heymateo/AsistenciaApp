using System.Collections.ObjectModel;
using System.ComponentModel;

using AsistenciaApp.Contracts.ViewModels;
using AsistenciaApp.Core.Contracts.Services;
using AsistenciaApp.Core.Models;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AsistenciaApp.ViewModels;

// Falta programar el filtrado de los estudiantes por seccion
public partial class EstudiantesDataGridViewModel : ObservableRecipient, INavigationAware, INotifyPropertyChanged
{
    private readonly IDataService _dataService;

    private ObservableCollection<Estudiante> _filtradoEstudiantes;

    public ObservableCollection<string> SeccionesDisponibles { get; } = new ObservableCollection<string>();

    private string _seccionSeleccionada;

    public ObservableCollection<Estudiante> Source { get; } = new ObservableCollection<Estudiante>();

    public EstudiantesDataGridViewModel(IDataService DataService)
    {
        _dataService = DataService;
    }

    public string SeccionSeleccionada
    {
        get => _seccionSeleccionada;
        set
        {
            SetProperty(ref _seccionSeleccionada, value);
            FiltrarEstudiantes();
        }
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();

        // TODO: Replace with real data.
        var data = await _dataService.GetGridDataAsync();

        foreach (var item in data)
        {
            Source.Add(item);
        }

        // Popular la coleccion de seccionesDisponibles
        var sections = Source.Select(s => s.Seccion).Distinct().ToList();
        SeccionesDisponibles.Clear();
        foreach (var section in sections)
        {
            SeccionesDisponibles.Add(section);
        }

    }

    public void OnNavigatedFrom()
    {
    }

    // Filtrar estudiantes basado en el valor de 'SeccionSeleccionada'
    private void FiltrarEstudiantes()
    {
        // Filtrar los estudiantes según la sección seleccionada
        var estudiantesFiltrados = string.IsNullOrEmpty(SeccionSeleccionada)
            ? Source
            : Source.Where(e => e.Seccion == SeccionSeleccionada);

        FiltradoEstudiantes = new ObservableCollection<Estudiante>(estudiantesFiltrados);

        // Notificar cambio para que el DataGrid actualice su contenido
        OnPropertyChanged(nameof(FiltradoEstudiantes));
    }

    public ObservableCollection<Estudiante> FiltradoEstudiantes
    {
        get => _filtradoEstudiantes;
        set
        {
            _filtradoEstudiantes = value;
            OnPropertyChanged();
        }
    }
}
