using System.Diagnostics;
using AsistenciaApp.Core.Models;
using AsistenciaApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AsistenciaApp.Views;
public sealed partial class AgregarAsistenciaPage : Page
{
    public AgregarAsistenciaViewModel _viewModel { get; set; }
    public AssistanceDbContext _dbContext;
    public AgregarAsistenciaPage()
    {
        this.InitializeComponent();
        _viewModel = App.GetService<AgregarAsistenciaViewModel>();
        this.DataContext = _viewModel;
    }

    private async void EliminarAsistenciaMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        var selectedAsistencia = (sender as MenuFlyoutItem)?.DataContext as Registro_Asistencia;

        if (selectedAsistencia == null)
        {
            Debug.WriteLine("El registro seleccionado (selectedAsistencia) es nulo.");
            return;
        }

        try
        {
            using (var dbContext = new AssistanceDbContext())
            {
                if (dbContext.Entry(selectedAsistencia).State == EntityState.Detached)
                {
                    dbContext.Registro_Asistencia.Attach(selectedAsistencia);
                }

                dbContext.Registro_Asistencia.Remove(selectedAsistencia);

                await dbContext.SaveChangesAsync();
            }

            _viewModel.Asistencias.Remove(selectedAsistencia);

            Debug.WriteLine("El registro fue eliminado correctamente.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ocurrió un error al eliminar el registro: {ex.Message}");
        }
    }


    private async void RegistrarAsistenciaButton_Click(object sender, RoutedEventArgs e)
    {
        string cedula = CedulaTextBox.Text;

        if (string.IsNullOrEmpty(cedula))
        {
            await MostrarDialogo("Error", "Ingrese una cédula válida.");
            return;
        }

        using (var db = new AssistanceDbContext())
        {
            var estudiante = db.Estudiante.FirstOrDefault(e => e.Identificacion == cedula);

            if (estudiante != null)
            {
                var fechaHoy = FechaAsistenciaPicker.Date.DateTime.Date;

                bool existeAsistencia = db.Registro_Asistencia.Any(a => a.Id_Estudiante == estudiante.Id_Estudiante && a.Fecha == fechaHoy);

                if (existeAsistencia)
                {
                    await MostrarDialogo("Error", "El estudiante ya tiene una asistencia registrada para hoy");
                    return;
                }

                var nuevaAsistencia = new Registro_Asistencia
                {
                    Id_Estudiante = estudiante.Id_Estudiante,
                    Fecha = FechaAsistenciaPicker.Date.DateTime.Date,
                    Hora_Entrada = TimeOnly.FromDateTime(DateTime.Now),
                    NombreEstudiante = estudiante.Nombre,
                    Asistio = true
                };

                await _viewModel.RegistrarAsistenciaAsync(nuevaAsistencia);

                await MostrarDialogo("Éxito", "La asistencia fue registrada correctamente.");
            }

            else
            {
                await MostrarDialogo("Error", "Estudiante no encontrado.");
            }
        }
    }

    private async Task MostrarDialogo(string titulo, string contenido)
    {
        var dialog = new ContentDialog
        {
            Title = titulo,
            Content = contenido,
            CloseButtonText = "Aceptar",
            XamlRoot = this.XamlRoot
        };

        await dialog.ShowAsync();
    }
}

