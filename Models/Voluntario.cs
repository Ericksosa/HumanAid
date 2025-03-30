using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanAid.Models
{
    public class Voluntario
    {
        [Key]
        public int VoluntarioId { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "El maximo de caracteres es 100")]
        public string Nombre { get; set; }
        [Required]
        [MaxLength(255, ErrorMessage = "El maximo de caracteres es 255")]
        public string Direccion { get; set; }
        [Required]
        public DateTime FechaNacimiento { get; set; }

        //Relacion con Sede
        public int SedeId { get; set; }
        [ForeignKey("SedeId")]
        public Sede Sede { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "El maximo de caracteres es 100")]
        public string Email { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "El maximo de caracteres es 20")]
        public string Telefono { get; set; }

        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        //Relacion con voluntarioAdministrativo
        public VoluntarioAdministrativo VoluntarioAdministrativo { get; set; }

        //Relacion con voluntarioSanitario
        public VoluntarioSanitario VoluntarioSanitario { get; set; }

    }
}
