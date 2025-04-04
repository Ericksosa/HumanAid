using System.Diagnostics;
using DocumentFormat.OpenXml.InkML;
using HumanAid.Data;
using HumanAid.Models;
using HumanAid.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace HumanAid.Areas.Administrador.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Area("Administrador")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EstadisticasService _estadisticasService; // Agregado
        private readonly HumanAidDbContext _context;

        public HomeController(ILogger<HomeController> logger, EstadisticasService estadisticasService, HumanAidDbContext context)
        {
            _logger = logger;
            _estadisticasService = estadisticasService; // Inyectado correctamente
            _context = context;
        }

        public IActionResult Index()
        {
            var datos = _estadisticasService.ObtenerEstadisticas();
            return View(datos);
        }

        public IActionResult Estadisticas()
        {
            var datos = _context.Socio
                .GroupBy(s => new { SedeNombre = s.Sede.Nombre, TipoCuotaNombre = s.TipoCuota.Nombre, s.TipoCuota.Importe }) // Renamed properties to avoid duplicate names
                .Select(g => new {
                    Sede = g.Key.SedeNombre,
                    TipoCuota = g.Key.TipoCuotaNombre,
                    TotalSocios = g.Count(),
                    ImporteCuota = g.Key.Importe // Usamos Importe como referencia en lugar de Serie
                })
                .ToList();

            return View(datos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
