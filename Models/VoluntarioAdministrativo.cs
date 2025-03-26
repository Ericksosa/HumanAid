using System.ComponentModel.DataAnnotations;

namespace HumanAid.Models
{
    public class VoluntarioAdministrativo
    {
        [Key]
        public int VoluntarioAdministrativoId { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "El maximo de caracteres es 100")]
        public string Profesion { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "El maximo de caracteres es 100")]
        public string Departamento { get; set; }

        //Relacion
        public Voluntario voluntario { get; set; }
    }
}
