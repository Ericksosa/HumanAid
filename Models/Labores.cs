using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HumanAid.Models
{
    public enum TipoLabor
    {
        LaborAdministrativa,
        LaborSanitaria
    }

    public class Labores
    {
        [Key]
        public int LaborId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public TipoLabor Tipo { get; set; }
        public string Estado { get; set; } = "Pendiente"; // Estado por defecto
        public int VoluntarioId { get; set; }
        [ForeignKey("VoluntarioId")]
        public Voluntario Voluntario { get; set; }
    }
}
