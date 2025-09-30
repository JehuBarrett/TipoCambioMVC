using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TipoCambioMVC.Models
{
    [Table("Rol")]
    public class Rol
    {
        [Key]
        public int IdRol { get; set; }

        [Required, MaxLength(50)]
        public string TipoRol { get; set; } = null!;

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
