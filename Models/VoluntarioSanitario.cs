using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanAid.Models
{
    public class VoluntarioSanitario
    {
        [Key]
        public int VoluntarioSanitarioId { get; set; }
        [MaxLength(100, ErrorMessage = "El maximo de caracteres es 100")]
        public string? Profesion { get; set; }
        public bool? Disponibilidad { get; set; }
        public int? NumeroTrabajosRealizados { get; set; }

        // Relación
        public int? VoluntarioId { get; set; }
        [ForeignKey("VoluntarioId")]
        public Voluntario? Voluntario { get; set; }
        public ICollection<VoluntarioMision>? VoluntarioMisiones { get; set; }
    }
}

