using System.Diagnostics;
using System.Security.Claims;
using HumanAid.Data;
using HumanAid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanAid.Areas.VoluntariosAdministrativos.Controllers
{
    [Authorize(Roles = "VoluntarioAdministrativo")]
    [Area("VoluntariosAdministrativos")]
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

            var voluntarioAdministrativo = await _context.VoluntarioAdministrativo
                .Include(va => va.Voluntario)
                .ThenInclude(v => v.Usuario)
                .FirstOrDefaultAsync(va => va.Voluntario.UsuarioId.ToString() == userId);

            if (voluntarioAdministrativo == null)
            {
                return NotFound("Voluntario administrativo no encontrado.");
            }

            return View(voluntarioAdministrativo);
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

