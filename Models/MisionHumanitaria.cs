using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanAid.Models
{
    public class MisionHumanitaria
    {
        [Key]
        public int MisionId { get; set; }

        //Relacion con Envio
        public int EnvioId { get; set; }
        [ForeignKey("EnvioId")]
        public Envio? Envio { get; set; }

        [Required]
        public DateTime Fecha { get; set; }
        [Required]
        public string? Descripcion { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "El maximo de caracteres es 50")]
        public string? Estado { get; set; }

        //Estableciendo metodos de relación
        public ICollection<VoluntarioMision>? VoluntarioMisiones { get; set; }
    }
}
