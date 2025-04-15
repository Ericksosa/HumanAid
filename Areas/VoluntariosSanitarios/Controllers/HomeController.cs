using System.Diagnostics;
using HumanAid.Data;
using HumanAid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HumanAid.Areas.VoluntariosSanitarios.Controllers
{
    [Authorize(Roles = "VoluntarioSanitario")]
    [Area("VoluntariosSanitarios")]
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

            var voluntarioSanitario = await _context.VoluntarioSanitario
                .Include(vs => vs.Voluntario)
                .ThenInclude(v => v.Usuario)
                .FirstOrDefaultAsync(vs => vs.Voluntario.UsuarioId.ToString() == userId);

            if (voluntarioSanitario == null)
            {
                return NotFound("Voluntario sanitario no encontrado.");
            }

            return View(voluntarioSanitario);
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
