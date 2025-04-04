using System.ComponentModel.DataAnnotations;

namespace HumanAid.Models
{
    public class Permiso
    {
        [Key]
        public int PermisoId { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public ICollection<RolPermiso>? RolPermisos { get; set; }

    }
}
