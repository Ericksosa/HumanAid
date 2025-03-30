using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanAid.Models
{
    public class Alimento
    {
        [Key]
        public int AlimentoId { get; set; }

        //Relacion con Envio
        public int EnvioId { get; set; }
        [ForeignKey("EnvioId")]
        public Envio? Envio { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "El maximo de caracteres es 100")]
        public string? Tipo { get; set; }
        [Required]
        public decimal Peso {  get; set; }
    }
}
