using HumanAid.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace HumanAid.Models
{
    public enum MetodoPago
    {
        Tarjeta,
        Transferencia,
        Efectivo
    }

    public enum TipoTransaccion
    {
        Ingreso,
        Gasto
    }

    public class Transaccion : IValidatableObject
    {
        [Key]
        public int TransaccionId { get; set; }

        [Required]
        public decimal Monto { get; set; }

        [Required]
        public MetodoPago Metodo { get; set; }

        [MaxLength(100)]
        public string? Referencia { get; set; }

        [Required]
        public DateTime FechaPago { get; set; }

        [Required]
        public TipoTransaccion Tipo { get; set; }

        [MaxLength(255)]
        public string? Descripcion { get; set; }

        // Relación con Socio
        public int SocioId { get; set; }
        [ForeignKey("SocioId")]
        public Socio? Socio { get; set; }

        // Relación con TipoCuota
        public int? TipoCuotaId { get; set; }
        [ForeignKey("TipoCuotaId")]
        public TipoCuota? TipoCuota { get; set; }

        // Validación personalizada
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errores = new List<ValidationResult>();

            if (Tipo == TipoTransaccion.Ingreso)
            {
                if (TipoCuotaId == null)
                {
                    errores.Add(new ValidationResult("Debe seleccionar un Tipo de Cuota para los ingresos.", new[] { nameof(TipoCuotaId) }));
                }
                else
                {
                    var dbContext = (HumanAidDbContext)validationContext.GetService(typeof(HumanAidDbContext))!;
                    var tipoCuota = dbContext.TipoCuota.Find(TipoCuotaId);

                    if (tipoCuota != null && Monto != tipoCuota.Importe)
                    {
                        errores.Add(new ValidationResult($"El monto debe ser {tipoCuota.Importe} según el Tipo de Cuota seleccionado.", new[] { nameof(Monto) }));
                    }
                }
            }

            return errores;
        }
    }
}
