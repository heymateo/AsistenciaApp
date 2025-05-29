using System.ComponentModel.DataAnnotations;

namespace AsistenciaApp.Core.Models;

#nullable enable
public class Estudiante
{
    [Key]
    public int Id_Estudiante { get; set; }
    [Required(ErrorMessage = "Cédula")]
    public string Identificacion { get; set; } = string.Empty;
    [Required(ErrorMessage = "Nombre")]
    public string Nombre { get; set; } = string.Empty;
    [Required(ErrorMessage = "Nivel")]
    public string Nivel { get; set; } = string.Empty;
    [Required(ErrorMessage = "Sección")]
    public string Seccion { get; set; } = string.Empty;
    [Required(ErrorMessage = "Grupo")]
    public string Grupo { get; set; } = string.Empty;
    public string? Especialidad { get; set; } = string.Empty;

    [Required(ErrorMessage = "Encargado")]
    public string? Encargado_Legal { get; set; } = string.Empty;

    [Required(ErrorMessage = "Teléfono")]
    public string? Telefono_Encargado { get; set; } = string.Empty;
    public ICollection<Registro_Asistencia> Registro_Asistencia { get; set; } = new List<Registro_Asistencia>();

}
