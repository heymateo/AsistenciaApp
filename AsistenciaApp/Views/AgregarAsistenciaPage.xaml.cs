using AsistenciaApp.Core.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AsistenciaApp.Views;
public sealed partial class AgregarAsistenciaPage : Page
{
    public AgregarAsistenciaPage()
    {
        this.InitializeComponent();
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
                NombreTextBlock.Text = $"Nombre: {estudiante.Nombre}";
                SeccionTextBlock.Text = $"Sección: {estudiante.Seccion}";
                EstudiantePanel.Visibility = Visibility.Visible;

                var nuevaAsistencia = new Registro_Asistencia
                {
                    Id_Estudiante = estudiante.Id_Estudiante,
                    Fecha = FechaAsistenciaPicker.Date.DateTime,
                    Hora_Entrada = TimeOnly.FromDateTime(DateTime.Now),
                    Asistio = true
                };

                db.Registro_Asistencia.Add(nuevaAsistencia);
                await db.SaveChangesAsync();

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

