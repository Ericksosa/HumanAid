using HumanAid.Data;
using HumanAid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HumanAid.Areas.VoluntariosSanitarios.Controllers
{
    [Area("VoluntariosSanitarios")]
    [Authorize]
    public class LaboresController : Controller
    {
        private readonly HumanAidDbContext _context;

        public LaboresController(HumanAidDbContext context)
        {
            _context = context;
        }

        // GET: VoluntariosSanitarios/Labores
        public async Task<IActionResult> Index(string searchString, string sortOrder, int page = 1)
        {
            int pageSize = 10;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Encontrar el registro de voluntario basado en el UsuarioId
            var voluntario = await _context.Voluntario
                .FirstOrDefaultAsync(v => v.UsuarioId.ToString() == userId);

            if (voluntario == null)
            {
                return NotFound("Voluntario no encontrado.");
            }

            // Filtrar las labores basadas en el VoluntarioId
            var laboresQuery = _context.Labores
                .Where(l => l.VoluntarioId == voluntario.VoluntarioId);

            if (!string.IsNullOrEmpty(searchString))
            {
                laboresQuery = laboresQuery.Where(l => l.Descripcion.Contains(searchString));
            }

            // Ordenar las labores según la fecha
            switch (sortOrder)
            {
                case "date_desc":
                    laboresQuery = laboresQuery.OrderByDescending(l => l.Fecha);
                    break;
                default:
                    laboresQuery = laboresQuery.OrderBy(l => l.Fecha);
                    break;
            }

            int totalItems = await laboresQuery.CountAsync();
            var labores = await laboresQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewData["SearchString"] = searchString;

            return View(labores);
        }



    }
}

