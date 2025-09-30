using System.ComponentModel.DataAnnotations;

namespace TipoCambioMVC.Models.Auth
{
    public class RegisterVM
    {
        [Required, EmailAddress] public string Email { get; set; } = null!;
        [Required, MaxLength(100)] public string Nombre { get; set; } = null!;
        [Required, DataType(DataType.Password), MinLength(6)] public string Password { get; set; } = null!;
        [Required, DataType(DataType.Password), Compare(nameof(Password))] public string ConfirmPassword { get; set; } = null!;
    }
}
