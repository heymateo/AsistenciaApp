namespace AsistenciaApp.ViewModels;
public class RegistroAsistenciaViewModel
{
    public int Id_Registro
    {
        get; set;
    }
    public string Nombre_Estudiante
    {
        get; set;
    }
    public DateTime Fecha
    {
        get; set;
    }

    public TimeOnly Hora_Entrada
    {
        get; set;
    }
    public bool Asistio
    {
        get; set;
    }
}
