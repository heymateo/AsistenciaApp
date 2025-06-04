using System.ComponentModel.DataAnnotations;

namespace AsistenciaApp.Core.Models;

#nullable enable
public class Admin
{
    [Key]
    public int Id_Admin { get; set; }
    [Required(ErrorMessage = "Escriba el nombre de usuario")]
    public string User { get; set; } = string.Empty;
    [Required(ErrorMessage = "Escriba su correo")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Escriba su contraseña")]
    public string Password { get; set; } = string.Empty;
    public string? Salt { get; set; }
    public string? Password_Reset_Token { get; set; }  
    public DateTime? Password_Reset_Token_Expiry { get; set; }
}
