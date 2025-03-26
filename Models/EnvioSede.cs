using System.ComponentModel.DataAnnotations.Schema;

namespace HumanAid.Models
{
    public class EnvioSede
    {
        /* Esto es una tabla intermediaria
         * , se realiza porque la relacion es de 
         * mucho a mucho */

        //Relacion Con Envio
        public int EnvioId { get; set; }
        [ForeignKey("EnvioId")]
        public Envio Envio { get; set; }

        //Relacion con Sede
        public int SedeId {  get; set; }
        [ForeignKey("SedeId")]
        public Sede Sede { get; set; }
    }
}
