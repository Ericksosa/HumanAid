using HumanAid.Data;
using HumanAid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HumanAid.Areas.VoluntariosAdministrativos.Controllers
{
    [Area("VoluntariosAdministrativos")]
    [Authorize]
    public class LaboresController : Controller
    {
        private readonly HumanAidDbContext _context;

        public LaboresController(HumanAidDbContext context)
        {
            _context = context;
        }

        // GET: VoluntariosAdministrativos/Labores
        public async Task<IActionResult> Index(string searchString, string sortOrder, int page = 1)
        {
            int pageSize = 10;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Buscar el voluntario que corresponde a este usuario
            var voluntario = await _context.Voluntario
                .FirstOrDefaultAsync(v => v.UsuarioId.ToString() == userId);

            if (voluntario == null)
            {
                return NotFound("No se encontró un voluntario con este UsuarioId.");
            }

            // Ahora sí puedes buscar las labores con ese VoluntarioId
            var laboresQuery = _context.Labores
                .Where(l => l.VoluntarioId == voluntario.VoluntarioId);

            if (!string.IsNullOrEmpty(searchString))
            {
                laboresQuery = laboresQuery.Where(l => l.Descripcion.Contains(searchString));
            }

            // Ordenar según el sortOrder
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

