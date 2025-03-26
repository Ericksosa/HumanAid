using System.ComponentModel.DataAnnotations.Schema;

namespace HumanAid.Models
{
    public class VoluntarioMision
    {
        /* Tabla intermediaria
         * Relacion de mucho a mucho */

        //Relacion con VoluntarioSanitario
        public int VoluntarioSanitarioId { get; set; }
        [ForeignKey("VoluntarioSanitarioId")]
        public VoluntarioSanitario VoluntarioSanitario { get; set; }

        //Relacion con MisionHumanitaria
        public int MisionId { get; set; }
        [ForeignKey("MisionId")]
        public MisionHumanitaria MisionHumanitaria { get; set; }

        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }
    }
}
