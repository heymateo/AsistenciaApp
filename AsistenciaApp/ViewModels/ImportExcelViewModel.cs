using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsistenciaApp.Core.Models;
using AsistenciaApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExcelDataReader;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Prism.Dialogs;
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

                // Limpiar encabezados (quitar tildes)
                for (int c = 0; c < table.Columns.Count; c++)
                {
                    string originalName = table.Rows[dataStartRow][c]?.ToString() ?? "";
                    table.Columns[c].ColumnName = QuitarTildes(originalName.Trim());
                }

                // Función para acceder de forma segura a celdas
                string GetCell(DataRow row, int index)
                {
                    return index < row.ItemArray.Length ? row[index]?.ToString().Trim() ?? "" : "";
                }

                for (var i = dataStartRow + 1; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];

                    var filaVacia = row.ItemArray.All(cell => string.IsNullOrWhiteSpace(cell?.ToString()));

                    string[] valoresInvalidos = { "SIN ESPECIALIDAD", "N/A", "NINGUNA", "NO APLICA" };
                    var sinEspecialidad = row.ItemArray.Any(cell =>
                    {
                        var valor = cell?.ToString().Trim().ToUpperInvariant();
                        return valoresInvalidos.Contains(valor);
                    });

                    if (filaVacia)
                        continue;

                    // Leer y limpiar datos
                    var identificacion = LimpiarIdentificacion(GetCell(row, 0));
                    var nombre = GetCell(row, 1);
                    var nivel = GetCell(row, 2);
                    var seccion = GetCell(row, 3);
                    var grupo = GetCell(row, 4);
                    var especialidad = GetCell(row, 5);

                    if (string.IsNullOrWhiteSpace(identificacion) || string.IsNullOrWhiteSpace(nombre))
                        continue;

                    var estudiante = new Estudiante
                    {
                        Identificacion = identificacion,
                        Nombre = nombre,
                        Nivel = nivel,
                        Seccion = seccion,
                        Grupo = grupo
                    };

                    // Limpiar tildes del nivel antes del switch
                    var nivelLimpio = QuitarTildes(nivel.Trim()).ToLowerInvariant();
                    var especialidadLimpia = QuitarTildes(especialidad.Trim()).ToUpperInvariant();

                    // Lista de valores inválidos para especialidad
                    string[] valoresInvalidos2 = { "NO APLICA", "NINGUNA", "SIN ESPECIALIDAD", "N/A", "" };

                    switch (nivelLimpio.Trim())
                    {
                        case "decimo":
                        case "undecimo":
                        case "duodecimo":
                        case "3 ciclo":
                        case "4 ciclo":
                            estudiante.Especialidad = valoresInvalidos2.Contains(especialidadLimpia) ? null : especialidad;
                            break;
                        default:
                            estudiante.Especialidad = null;
                            break;
                    }

                    estudiantes.Add(estudiante);
                }
            }

            await InsertStudentsToDatabase(estudiantes);

            // Actualizar colección en tiempo real
            EstudiantesImportados.Clear();
            estudiantes.ForEach(est => EstudiantesImportados.Add(est));

            await _dialogService.ShowDialogAsync("Atención", $"Importación completada. {estudiantes.Count} estudiantes agregados.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("An error occurred: " + ex.Message);
            System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);

            if (ex.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine("Inner Exception: " + ex.InnerException.Message);
                System.Diagnostics.Debug.WriteLine("Inner Stack Trace: " + ex.InnerException.StackTrace);
            }


        }
    }

    private string LimpiarIdentificacion(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return string.Empty;

        return new string(valor
            .Where(c => char.IsLetterOrDigit(c)) // Solo letras y números
            .ToArray());
    }

    public static string QuitarTildes(string texto)
    {
        if (string.IsNullOrEmpty(texto))
            return texto;

        var normalized = texto.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
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
        using var context = new AssistanceDbContext();

        // Log the database path or connection info
        System.Diagnostics.Debug.WriteLine("Database path: " + context.Database.GetDbConnection().ConnectionString);



        foreach (var estudiante in estudiantes)
        {
            // Puedes verificar si ya existe por Identificación si quieres evitar duplicados:
            var existe = await context.Estudiante
                .AnyAsync(e => e.Identificacion == estudiante.Identificacion);

            if (!existe)
            {
                context.Estudiante.Add(estudiante);
            }
        }

        // context.Estudiante.RemoveRange(context.Estudiante);
        context.Estudiante.AddRange(estudiantes);

        await context.SaveChangesAsync();
    }
}