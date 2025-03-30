using System.ComponentModel.DataAnnotations;

namespace HumanAid.Models
{
    public class TipoCuota
    {
        [Key]
        public int TipoCuotaId { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "El maximo de caracteres es 50")]
        public string? Nombre { get; set; }
        [Required]
        public decimal Importe { get; set; }
        [Required]
        public string? Descripcion {  get; set; }

        //Metodos de relación
        public ICollection<Socio>? Socios { get; set; }
    }
}
