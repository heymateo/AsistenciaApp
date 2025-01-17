using AsistenciaApp.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AsistenciaApp.Views;

// TODO: Change the grid as appropriate for your app. Adjust the column definitions on DataGridPage.xaml.
// For more details, see the documentation at https://docs.microsoft.com/windows/communitytoolkit/controls/datagrid.
public sealed partial class EstudiantesDataGrid : Page
{
    public EstudiantesDataGridViewModel ViewModel
    {
        get;
    }

    public EstudiantesDataGrid()
    {
        ViewModel = App.GetService<EstudiantesDataGridViewModel>();
        DataContext = ViewModel;
        this.InitializeComponent();
    }
    
}
        
    

