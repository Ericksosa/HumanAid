using System.Diagnostics;
using System.Security.Claims;
using HumanAid.Data;
using HumanAid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanAid.Areas.Socio.Controllers
{
    [Authorize(Roles = "Socio")]
    [Area("Socio")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HumanAidDbContext _context;

        public HomeController(ILogger<HomeController> logger, HumanAidDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var socio = await _context.Socio
                .Include(s => s.Usuario)
                .FirstOrDefaultAsync(s => s.UsuarioId.ToString() == userId);

            if (socio == null)
            {
                return NotFound("Socio no encontrado.");
            }

            ViewData["NombreSocio"] = socio.Nombre;
            return View(socio);
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
