using System.ComponentModel.DataAnnotations;

namespace HumanAid.Models
{
    public class Envio
    {
        [Key]
        public int EnvioId {  get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        [MaxLength(255, ErrorMessage = "El maximo de caracteres es 255")]
        public string? Destino {  get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "El maximo de caracteres es 50")]
        public string? TipoEnvio {  get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "El maximo de caracteres es 50")]
        public string? Estado { get; set; }

        //Aplicacion de los metodos relacionales
        public ICollection<Alimento>? Alimentos { get; set; }
        public ICollection<EnvioSede>? EnvioSedes { get; set; }
        public ICollection<Medicamento>? Medicamentos { get; set; }
        public ICollection<MisionHumanitaria>? MisionesHumanitarias { get; set; }
    }
}
