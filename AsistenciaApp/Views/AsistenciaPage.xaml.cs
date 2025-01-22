using AsistenciaApp.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace AsistenciaApp.Views;
public sealed partial class AsistenciaPage : Page
{
    public AsistenciaPage()
    {
        InitializeComponent();
    }

    public AsistenciaViewModel ViewModel { get; } = new();

    private void CalendarView_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
    {
        ViewModel.TargetDate = args.AddedDates.Count > 0
            ? args.AddedDates[0].Date
            : null;
    }
}
