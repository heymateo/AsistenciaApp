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
        var selectedAsistencia = (sender as MenuFlyoutItem).DataContext as Registro_Asistencia;
        if (selectedAsistencia != null)
        {
            // Eliminar la asistencia de la base de datos
            _dbContext.Registro_Asistencia.Remove(selectedAsistencia);

            // Guardar cambios en la base de datos
            await _dbContext.SaveChangesAsync();

            // Actualizar la lista en memoria (esto puede variar según tu implementación)
            _viewModel.Asistencias.Remove(selectedAsistencia);
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
                var nuevaAsistencia = new Registro_Asistencia
                {
                    Id_Estudiante = estudiante.Id_Estudiante,
                    Fecha = FechaAsistenciaPicker.Date.DateTime.Date,
                    Hora_Entrada = TimeOnly.FromDateTime(DateTime.Now),
                    Asistio = true
                };

                await _viewModel.RegistrarAsistenciaAsync(nuevaAsistencia);
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

