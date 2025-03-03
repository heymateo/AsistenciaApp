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

        // Escuchar cambios en la lista de asistencias
        Asistencias.CollectionChanged += (s, e) =>
        {
            HeaderText = Asistencias.Any() ? "Asistencias Registradas" : "No se encontraron asistencias.";
        };

        _ = LoadAsistenciasAsync();  
    }

    private async Task LoadAsistenciasAsync()
    {
        var fechaHoy = DateTime.Today;

        var asistenciasFromDb = await _dbContext.Registro_Asistencia
            .Include(a => a.Estudiante)  // Cargar relación con Estudiante
            .Where(a => a.Fecha.Date == fechaHoy)
            .ToListAsync();

        Asistencias.Clear();

        foreach (var asistencia in asistenciasFromDb)
        {
            asistencia.NombreEstudiante = asistencia.Estudiante?.Nombre ?? "Desconocido"; // Asignar nombre manualmente
            Asistencias.Add(asistencia);
        }

        // Actualizar el encabezado dinámicamente
        HeaderText = Asistencias.Any() ? "Asistencias Registradas" : "No se encontraron asistencias.";

        OnPropertyChanged(nameof(Asistencias));
    }


    public async Task RegistrarAsistenciaAsync(Registro_Asistencia nuevaAsistencia)
    {
        var existingAsistencia = await _dbContext.Registro_Asistencia
            .Where(a => a.Id_Registro == nuevaAsistencia.Id_Registro)
            .FirstOrDefaultAsync();

        if (existingAsistencia != null)
        {
            existingAsistencia.Asistio = nuevaAsistencia.Asistio;
            existingAsistencia.Fecha = nuevaAsistencia.Fecha;
            existingAsistencia.Hora_Entrada = nuevaAsistencia.Hora_Entrada;
        }
        else
        {
            _dbContext.Registro_Asistencia.Add(nuevaAsistencia);
        }

        await _dbContext.SaveChangesAsync();

        nuevaAsistencia.Estudiante = await _dbContext.Estudiante.FindAsync(nuevaAsistencia.Id_Estudiante);
        nuevaAsistencia.NombreEstudiante = nuevaAsistencia.Estudiante?.Nombre ?? "Desconocido";

        Asistencias.Add(nuevaAsistencia);

        HeaderText = "Asistencias Registradas";
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

    private string _headerText = "No se encontraron asistencias.";
    public string HeaderText
    {
        get => _headerText;
        set
        {
            if (_headerText != value)
            {
                _headerText = value;
                OnPropertyChanged(nameof(HeaderText));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

