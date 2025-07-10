using System;
using System.Collections.Generic;
using System.Linq;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using AsistenciaApp.Core.Models;
using AsistenciaApp.ViewModels;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Forms.Fields;
using iText.Kernel.Geom;
using iText.Forms;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;

namespace AsistenciaApp.Views;
public sealed partial class CalendarioPage : Page
{
    public CalendarioPage()
    {
        InitializeComponent();
    }

    public AsistenciaViewModel ViewModel { get; } = new();

    private void CalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
    {
        ExportButton.IsEnabled = CalendarDatePicker1.Date != null && CalendarDatePicker2.Date != null;
    }

    private async void ExportToPDF(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        try
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            var records = GetAttendanceRecords(startDate.DateTime, endDate.DateTime);

            if (records.Count == 0)
            {
                await ShowDialog("Exportación", "No se encontraron registros en el rango de fechas seleccionado.");
                return;
            }

            var savePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            savePicker.FileTypeChoices.Add("PDF File", new List<string>() { ".pdf" });
            savePicker.SuggestedFileName = $"Asistencia_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf";
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                using (PdfWriter writer = new PdfWriter(file.Path))
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document document = new Document(pdf);

                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    document.Add(new Paragraph("Registro de Asistencia")
                        .SetFont(boldFont)
                        .SetFontSize(16)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                    // Agrupar los registros por fecha
                    var groupedRecords = records
                        .GroupBy(r => r.Fecha.Date)
                        .OrderBy(g => g.Key);

                    foreach (var group in groupedRecords)
                    {
                        // Título por fecha
                        document.Add(new Paragraph($"\nFecha: {group.Key:yyyy-MM-dd}")
                            .SetFont(boldFont)
                            .SetFontSize(12)
                            .SetMarginTop(10));

                        Table table = new Table(3);
                        table.AddHeaderCell("Cédula");
                        table.AddHeaderCell("Nombre");
                        table.AddHeaderCell("Marca");

                        foreach (var record in group)
                        {
                            table.AddCell(record.Estudiante?.Identificacion ?? "");
                            table.AddCell(record.NombreEstudiante);
                            table.AddCell(record.Hora_Entrada.ToString());
                        }

                        document.Add(table);
                    }

                    // Firma
                    PdfAcroForm form = PdfAcroForm.GetAcroForm(pdf, true);
                    PdfWidgetAnnotation widget = new(new Rectangle(70, 20, 150, 25));
                    PdfSignatureFormField signature = new PdfFormFactory().CreateSignatureFormField(widget, pdf);
                    widget.SetPage(pdf.GetLastPage());
                    widget.SetVisibility(4);
                    signature.SetFieldName("Signature");
                    form.AddField(signature);

                    Paragraph signatureLabel = new Paragraph("Firma del Responsable:")
                        .SetFixedPosition(pdf.GetNumberOfPages(), 70, 70, 200)
                        .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA))
                        .SetFontSize(10);
                    document.Add(signatureLabel);

                    PdfCanvas canvas = new PdfCanvas(pdf.GetLastPage());
                    canvas
                        .SetLineWidth(0.5f)
                        .MoveTo(70, 50)
                        .LineTo(220, 50)
                        .Stroke();

                    document.Close();
                }

                await ShowDialog("Exportación exitosa", "El archivo PDF ha sido generado correctamente.");
            }
        }
        catch (Exception ex)
        {
            await ShowDialog("Error", $"Ocurrió un error al exportar: {ex.Message}");
        }
    }


    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        if (CalendarDatePicker1.Date != null && CalendarDatePicker2.Date != null)
        {
            ExportToPDF(CalendarDatePicker1.Date.Value, CalendarDatePicker2.Date.Value);
        }
    }

    private List<Registro_Asistencia> GetAttendanceRecords(DateTime startDate, DateTime endDate)
    {
        using var context = new AssistanceDbContext();
        {
            return context.Registro_Asistencia
                .Where(r => r.Fecha >= startDate && r.Fecha <= endDate)
                .Select(r => new Registro_Asistencia
                {
                    Id_Registro = r.Id_Registro,
                    Id_Estudiante = r.Id_Estudiante,
                    Fecha = r.Fecha,
                    Hora_Entrada = r.Hora_Entrada,
                    Asistio = r.Asistio,
                    NombreEstudiante = r.Estudiante.Nombre,
                    Estudiante = new Estudiante
                    {
                        Identificacion = r.Estudiante.Identificacion
                    }
                })
                .ToList();
        }
    }

    private async System.Threading.Tasks.Task ShowDialog(string title, string content)
    {
        ContentDialog dialog = new ContentDialog
        {
            Title = title,
            Content = content,
            CloseButtonText = "OK",
            XamlRoot = App.MainWindow.Content.XamlRoot
        };
        await dialog.ShowAsync();
    }

    private void CalendarView_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
    {
        if (args.AddedDates.Count > 0)
        {
            ViewModel.TargetDate = args.AddedDates[0].Date;
        }
        else
        {
            ViewModel.TargetDate = null;
        }
    }

}
