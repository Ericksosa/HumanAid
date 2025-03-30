using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanAid.Models
{
    public class Usuario
    {
        [Key]
        public int UsuarioId { get; set; }
        [Required]
        [EmailAddress]
        public string Correo { get; set; }
        [Required]
        public string Clave { get; set; }

        public int RolId { get; set; }
        [ForeignKey("RolId")]
        public Rol Rol { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public Socio Socio { get; set; }
        public Voluntario Voluntario { get; set; }

    }
}
