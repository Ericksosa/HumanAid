using HumanAid.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Pago
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int SocioId { get; set; }

    [ForeignKey("SocioId")]
    public Socio Socio { get; set; }

    [Required]
    public int TipoCuotaId { get; set; } // Cambiado de string a int

    [ForeignKey("TipoCuotaId")]
    public TipoCuota TipoCuota { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto de la cuota debe ser mayor que 0.")]
    public decimal MontoCuota { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad de cuotas debe ser al menos 1.")]
    public int CantidadCuotas { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Pago")]
    public DateTime FechaPago { get; set; }
}