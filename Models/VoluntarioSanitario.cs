using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanAid.Models
{
    public class VoluntarioSanitario
    {
        [Key]
        public int VoluntarioSanitarioId { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "El maximo de caracteres es 100")]
        public string Profesion { get; set; }
        [Required]
        public bool Disponibilidad { get; set; }
        [Required]
        public int NumeroTrabajosRealizados { get; set; }

        //Relación
        public ICollection<VoluntarioMision> VoluntarioMisiones { get; set; }
        public int VoluntarioId { get; set; }
        [ForeignKey("VoluntarioId")]
        public Voluntario Voluntario { get; set; }
    }
}
