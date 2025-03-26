using Microsoft.AspNetCore.Antiforgery;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanAid.Models
{
    public class Socio
    {
        [Key]
        public int SocioId { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "El maximo de caracteres es 100")]
        public string Nombre { get; set; }
        [Required]
        public DateTime FechaNacimiento { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "El maximo de caracteres es 50")]
        public string CuentaBancaria { get; set; }
        [Required]
        public DateTime FechaPago { get; set; }

        //Relacion con TipoCuota
        public int TipoCuotaId {  get; set; }
        [ForeignKey("TipoCuotaId")]
        public TipoCuota TipoCuota { get; set; }

        //Relacion con Sede
        public int SedeId {  get; set; }
        [ForeignKey("SedeId")]
        public Sede Sede { get; set; }

        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
