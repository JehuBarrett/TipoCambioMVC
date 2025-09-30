using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TipoCambioMVC.Models
{
    [Table("DivisaUsuario")]
    public class DivisaUsuario
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string IdUsuario { get; set; } = null!;  // email

        [Required, MaxLength(5)]
        public string Codigo { get; set; } = null!;

        [Required]
        public bool IsPrincipal { get; set; } = false;

        public Usuario Usuario { get; set; } = null!;
    }
}
