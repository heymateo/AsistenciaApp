using AsistenciaApp.Core.Models;
using AsistenciaApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExcelDataReader;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Prism.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace AsistenciaApp.ViewModels;

public partial class ImportExcelViewModel : ObservableRecipient
{
    public IRelayCommand ImportExcelCommand
    {
        get;
    }

    private readonly ContentDialogService _dialogService;

    private ObservableCollection<Estudiante> _estudiantesImportados = new();
    public ObservableCollection<Estudiante> EstudiantesImportados
    {
        get => _estudiantesImportados;
        set => SetProperty(ref _estudiantesImportados, value);
    }

    public ImportExcelViewModel(ContentDialogService dialogService)
    {
        ImportExcelCommand = new AsyncRelayCommand(ImportExcelFile);
        _dialogService = dialogService;
    }

    public async Task ImportExcelFile()
    {
        try
        {
            var filePath = await PickExcelFileAsync();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var result = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = false
                }
            });

            List<Estudiante> estudiantes = new List<Estudiante>();

            foreach (DataTable table in result.Tables)
            {
                var dataStartRow = DetectDataStartRow(table);

                for (var i = dataStartRow + 1; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];

                    // Verificar si la fila está completamente vacía
                    var filaVacia = row.ItemArray.All(cell => string.IsNullOrWhiteSpace(cell?.ToString()));
                    if (filaVacia)
                    {
                        continue; // Saltar filas vacías
                    }

                    // Leer y limpiar datos
                    var identificacion = row[1]?.ToString().Trim() ?? "";  //  validar primero que el índice exista
                    var nombre = row[2]?.ToString().Trim() ?? "";
                    var nivel = row[3]?.ToString().Trim() ?? "";
                    var seccion = row[4]?.ToString().Trim() ?? "";
                    var grupo = row[5]?.ToString().Trim() ?? "";

                    // Si identificación y nombre están vacíos, descartar la fila
                    if (string.IsNullOrWhiteSpace(identificacion) || string.IsNullOrWhiteSpace(nombre))
                    {
                        continue;
                    }

                    var estudiante = new Estudiante
                    {
                        Identificacion = identificacion,
                        Nombre = nombre,
                        Nivel = nivel,
                        Seccion = seccion,
                        Grupo = grupo,
                        Especialidad = (nivel == "10" || nivel == "11" || nivel == "12") ? (row[6]?.ToString().Trim() ?? "") : null,
                        Encargado_Legal = (nivel == "10" || nivel == "11" || nivel == "12") ? row[7]?.ToString().Trim() ?? "" : row[6]?.ToString().Trim() ?? "",
                        Telefono_Encargado = (nivel == "10" || nivel == "11" || nivel == "12") ? row[8]?.ToString().Trim() ?? "" : row[7]?.ToString().Trim() ?? ""
                    };

                    estudiantes.Add(estudiante);
                }
            }

            await InsertStudentsToDatabase(estudiantes);

            // Actualizar la colección observable para reflejar cambios en tiempo real
            EstudiantesImportados.Clear();
            estudiantes.ForEach(est => EstudiantesImportados.Add(est));

            await _dialogService.ShowDialogAsync("Atención", $"Importación completada. {estudiantes.Count} estudiantes agregados.");
        }
        catch (Exception)
        {
            
        }
    }

    private int DetectDataStartRow(DataTable table)
    {
        for (var i = 0; i < table.Rows.Count; i++)
        {
            var row = table.Rows[i];
            if (row.ItemArray.All(cell => cell != null && !string.IsNullOrWhiteSpace(cell.ToString())))
            {
                return i;
            }
        }
        return 0;
    }

    private async Task<string?> PickExcelFileAsync()
    {
        var picker = new FileOpenPicker();
        picker.ViewMode = PickerViewMode.Thumbnail;
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        picker.FileTypeFilter.Add(".xlsx");
        picker.FileTypeFilter.Add(".xls");

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hwnd);

        StorageFile file = await picker.PickSingleFileAsync();
        return file?.Path;
    }

    private async Task InsertStudentsToDatabase(List<Estudiante> estudiantes)
    {
        using var connection = new SqliteConnection("Data Source=C:\\Users\\mateo\\Desktop\\Assistance\\Assistance\\DB_ASSISTANCE.db");
        {
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();
            {
                foreach (var estudiante in estudiantes)
                {
                    var command = connection.CreateCommand();
                    command.CommandText = @"
                    INSERT INTO Estudiante 
                    (Identificacion, Nombre, Nivel, Seccion, Grupo, Especialidad, Encargado_Legal, Telefono_Encargado) 
                    VALUES ($Identificacion, $Nombre, $Nivel, $Seccion, $Grupo, $Especialidad, $Encargado_Legal, $Telefono_Encargado)";

                    command.Parameters.AddWithValue("$Identificacion", estudiante.Identificacion);
                    command.Parameters.AddWithValue("$Nombre", estudiante.Nombre);
                    command.Parameters.AddWithValue("$Nivel", estudiante.Nivel);
                    command.Parameters.AddWithValue("$Seccion", estudiante.Seccion);
                    command.Parameters.AddWithValue("$Grupo", estudiante.Grupo);
                    command.Parameters.AddWithValue("$Especialidad", estudiante.Especialidad ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("$Encargado_Legal", estudiante.Encargado_Legal ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("$Telefono_Encargado", estudiante.Telefono_Encargado ?? (object)DBNull.Value);

                    await command.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
        }
    }
}
