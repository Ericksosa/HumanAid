using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HumanAid.Data;
using HumanAid.Models;

namespace HumanAid.Areas.Administrador.Controllers
{
    [Area("Administrador")]
    public class LaboresController : Controller
    {
        private readonly HumanAidDbContext _context;

        public LaboresController(HumanAidDbContext context)
        {
            _context = context;
        }

        // GET: Administrador/Labores
        public async Task<IActionResult> Index(string searchString, int page = 1)
{
    int pageSize = 7;

    var labores = from l in _context.Labores.Include(l => l.Voluntario)
                  select l;

    if (!string.IsNullOrEmpty(searchString))
    {
        labores = labores.Where(s => s.Descripcion.Contains(searchString));
    }

    int totalItems = await labores.CountAsync();
    var items = await labores
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    ViewData["CurrentPage"] = page;
    ViewData["TotalPages"] = (int)Math.Ceiling((double)totalItems / pageSize);
    ViewData["SearchString"] = searchString;

    return View(items);
}


        // GET: Administrador/Labores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labores = await _context.Labores
                .Include(l => l.Voluntario)
                .FirstOrDefaultAsync(m => m.LaborId == id);
            if (labores == null)
            {
                return NotFound();
            }

            return View(labores);
        }

        // GET: Administrador/Labores/Create
        public IActionResult Create()
        {
            ViewData["VoluntarioId"] = new SelectList(_context.Voluntario, "VoluntarioId", "Nombre");
            return View();
        }

        // POST: Administrador/Labores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LaborId,Descripcion,Fecha,Tipo,Estado,VoluntarioId")] Labores labores)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Add(labores);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["success"] = "Labor creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error: {ex.Message} - {ex.InnerException?.Message}";
            }

            ViewData["VoluntarioId"] = new SelectList(_context.Voluntario, "VoluntarioId", "Nombre", labores.VoluntarioId);
            return View(labores);
        }



        // GET: Administrador/Labores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labores = await _context.Labores.FindAsync(id);
            if (labores == null)
            {
                return NotFound();
            }
            ViewData["VoluntarioId"] = new SelectList(_context.Voluntario, "VoluntarioId", "Nombre", labores.VoluntarioId);
            return View(labores);
        }

        // POST: Administrador/Labores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LaborId,Descripcion,Fecha,Tipo,Estado,VoluntarioId")] Labores labores)
        {
            if (id != labores.LaborId)
            {
                return NotFound();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Update(labores);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["success"] = "Labor editada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                if (!LaboresExists(labores.LaborId))
                {
                    return NotFound();
                }
                else
                {
                    TempData["danger"] = "Ocurrió un error de concurrencia al editar la labor.";
                    throw;
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error: {ex.Message} - {ex.InnerException?.Message}";
            }

            ViewData["VoluntarioId"] = new SelectList(_context.Voluntario, "VoluntarioId", "Nombre", labores.VoluntarioId);
            return View(labores);
        }


        // GET: Administrador/Labores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labores = await _context.Labores
                .Include(l => l.Voluntario)
                .FirstOrDefaultAsync(m => m.LaborId == id);
            if (labores == null)
            {
                return NotFound();
            }

            return View(labores);
        }

        // POST: Administrador/Labores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var labores = await _context.Labores.FindAsync(id);
                if (labores != null)
                {
                    _context.Labores.Remove(labores);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["success"] = "Labor eliminada exitosamente.";
                }
                else
                {
                    TempData["danger"] = "Labor no encontrada.";
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error: {ex.Message} - {ex.InnerException?.Message}";
            }

            return RedirectToAction(nameof(Index));
        }


        private bool LaboresExists(int id)
        {
            return _context.Labores.Any(e => e.LaborId == id);
        }
    }
}
