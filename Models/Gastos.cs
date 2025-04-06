using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanAid.Models
{
    public class Gastos
    {
        [Key]
        public int IdGastos { get; set; }
        [Required]
        public string? Descripcion { get; set; }
        [Required]
        public decimal Importe { get; set; }
        [Required]
        public DateTime FechaGasto { get; set; }

        //Relacion con Sede
        public int SedeId { get; set; }
        [ForeignKey("SedeId")]
        public Sede? Sede { get; set; }
    }
}
