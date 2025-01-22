using System.ComponentModel.DataAnnotations;

namespace AsistenciaApp.Core.Models;

public class Registro_Asistencia
{
    [Key]
    public int Id_Registro { get; set; }
    [Required]
    public int Id_Estudiante { get; set; }
    [Required(ErrorMessage = "Fecha")]
    public DateTime Fecha { get; set; }
    [Required(ErrorMessage = "Hora")]
    public TimeOnly Hora_Entrada { get; set; }
    [Required]
    public bool Asistio { get; set; }

    public Estudiante Estudiante { get; set; } = null!;
    public string NombreEstudiante { get; set; } = string.Empty;
}
