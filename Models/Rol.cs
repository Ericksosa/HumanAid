using System.ComponentModel.DataAnnotations;

namespace HumanAid.Models
{
    public class Rol
    {
        [Key]
        public int RolId { get; set; }
        [Required]
        public string? Nombre { get; set; }

        public ICollection<Usuario>? Usuarios { get; set; }
        public ICollection<RolPermiso>? RolPermisos { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

    }
}
