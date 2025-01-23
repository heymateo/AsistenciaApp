using System.ComponentModel.DataAnnotations;

namespace AsistenciaApp.Core.Models;

#nullable enable
public class Estudiante
{
    [Key]
    public int Id_Estudiante { get; set; }
    [Required(ErrorMessage = "Identificación")]
    public string Identificacion { get; set; } = string.Empty;
    [Required(ErrorMessage = "Nombre")]
    public string Nombre { get; set; } = string.Empty;
    [Required(ErrorMessage = "Nivel")]
    public string Nivel { get; set; } = string.Empty;
    [Required(ErrorMessage = "Sección")]
    public string Seccion { get; set; } = string.Empty;
    [Required(ErrorMessage = "Grupo")]
    public string Grupo { get; set; } = string.Empty;
    public string? Especialidad { get; set; }
    public string? Encargado_Legal { get; set; }
    public string? Telefono_Encargado { get; set; }
    public ICollection<Registro_Asistencia> Registro_Asistencia { get; set; } = new List<Registro_Asistencia>();

}
