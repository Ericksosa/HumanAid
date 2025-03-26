using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanAid.Models
{
    public class Medicamento
    {
        [Key]
        public int MedicamentoId { get; set; }

        //Relacion con envio
        public int EnvioId { get; set; }
        [ForeignKey("EnvioId")]
        public Envio Envio { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "El maximo de caracteres es 100")]
        public string Nombre {  get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "El maximo de caracteres es 50")]
        public string Dosis { get; set; }
        [Required]
        public int cantidad { get; set; }
    }
}
