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
            _context = context;
        }

        public IActionResult Index()
        {
            var totalSocios = _context.Socio.Count();
            var totalEnvios = _context.Envio.Count();
            var totalSedes = _context.Sede.Count();
            var totalVoluntarios = _context.Voluntario.Count();

            ViewBag.TotalSocios = totalSocios;
            ViewBag.TotalEnvios = totalEnvios;
            ViewBag.TotalSedes = totalSedes;
            ViewBag.TotalVoluntarios = totalVoluntarios;

            return View();
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
