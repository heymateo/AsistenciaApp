using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AsistenciaApp.Core.Models;

namespace AsistenciaApp.ViewModels;

public class AsistenciaViewModel : BindableBase // Implementa INotifyPropertyChanged
{
    private readonly AssistanceDbContext _context;
    private DateTime? _selectedDate;
    private ObservableCollection<Registro_Asistencia> _filteredEvents;
    private string _resultText;

    public AsistenciaViewModel()
    {
        _context = new AssistanceDbContext();
        FilteredEvents = new ObservableCollection<Registro_Asistencia>();
        ResultText = "Selecciona una fecha.";
    }

    public DateTime? SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (SetProperty(ref _selectedDate, value))
            {
                LoadFilteredEvents();
            }
        }
    }

    public ObservableCollection<Registro_Asistencia> FilteredEvents
    {
        get => _filteredEvents;
        set => SetProperty(ref _filteredEvents, value);
    }

    public string ResultText
    {
        get => _resultText;
        set => SetProperty(ref _resultText, value);
    }

    private void LoadFilteredEvents()
    {
        if (SelectedDate.HasValue)
        {
            var filtered = _context.Registro_Asistencia
                .Where(e => e.Fecha.Date == SelectedDate.Value.Date)
                .ToList();

            FilteredEvents.Clear();
            foreach (var item in filtered)
            {
                FilteredEvents.Add(item);
            }

            ResultText = filtered.Any()
                ? $"{filtered.Count} registros encontrados para {SelectedDate:dd/MM/yyyy}."
                : "No se encontraron registros para la fecha seleccionada.";
        }
        else
        {
            FilteredEvents.Clear();
            ResultText = "Selecciona una fecha.";
        }
    }
}
