using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TipoCambioMVC.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        // Email como PK
        [Key]
        [Required, EmailAddress, MaxLength(255)]
        public string IdUsuario { get; set; } = null!; // email

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = null!;

        [Required, MaxLength(255)]
        public string Contrasena { get; set; } = null!;

        [Required]
        public int IdRol { get; set; }

        public Rol Rol { get; set; } = null!;

        public ICollection<DivisaUsuario> DivisaUsuarios { get; set; } = new List<DivisaUsuario>();
    }
}
