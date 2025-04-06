using System.ComponentModel.DataAnnotations;

namespace HumanAid.Models
{
    public class Sede
    {
        [Key]
        public int SedeId { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "El maximo de caracteres es 100")]
        public string? Nombre { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "El maximo de caracteres es 100")]
        public string? Ciudad { get; set; }
        [Required]
        [MaxLength(255, ErrorMessage = "El maximo de caracteres es 255")]
        public string? Direccion { get; set; }
        [Required]
        [MaxLength(255, ErrorMessage = "El maximo de caracteres es 100")]
        public string? Director { get; set; }
        [Required]
        public DateTime FechaFundacion { get; set; }

        //Estableciendo metodos de relacion
        public ICollection<EnvioSede>? EnvioSedes { get; set; }
        public ICollection<Voluntario>? Voluntarios { get; set; }
        public ICollection<Socio>? Socios { get; set; }
        public ICollection<Gastos>? Gastos { get; set; }
    }
}
