using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AsistenciaApp.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AsistenciaApp.ViewModels;
public class AgregarAsistenciaViewModel : INotifyPropertyChanged
{
    private readonly AssistanceDbContext _dbContext;
    public ObservableCollection<Registro_Asistencia> Asistencias { get; set; }

    public AgregarAsistenciaViewModel(AssistanceDbContext dbContext)
    {
        _dbContext = dbContext;
        Asistencias = new ObservableCollection<Registro_Asistencia>();
        _ = LoadAsistenciasAsync();  
    }

    private async Task LoadAsistenciasAsync()
    {
        var fechaHoy = DateTime.Today;

        var asistenciasFromDb = await _dbContext.Registro_Asistencia
            .Where(a => a.Fecha.Date == fechaHoy)
            .ToListAsync();

        Asistencias.Clear();

        foreach (var asistencia in asistenciasFromDb)
        {
            Asistencias.Add(asistencia);
        }

        OnPropertyChanged(nameof(Asistencias));
    }

    public async Task RegistrarAsistenciaAsync(Registro_Asistencia nuevaAsistencia)
    {
        _dbContext.Registro_Asistencia.Add(nuevaAsistencia);
        await _dbContext.SaveChangesAsync();

        Asistencias.Add(nuevaAsistencia);  
    }


    private DateTimeOffset _fechaActual = DateTimeOffset.Now;

    public DateTimeOffset FechaActual
    {
        get => _fechaActual;
        set
        {
            if (_fechaActual != value)
            {
                _fechaActual = value;
                OnPropertyChanged(nameof(FechaActual));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

