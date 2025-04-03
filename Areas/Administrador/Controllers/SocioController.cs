using HumanAid.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HumanAid.Areas.Administrador.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class SocioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly EstadisticasService _estadisticasService;

        public SocioController(EstadisticasService estadisticasService)
        {
            _estadisticasService = estadisticasService;
        }

        public IActionResult Estadisticas()
        {
            var datos = _estadisticasService.ObtenerEstadisticas();
            return View(datos);
        }

    }
}
