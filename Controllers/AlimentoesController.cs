using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HumanAid.Data;
using HumanAid.Models;

namespace HumanAid.Controllers
{
    public class AlimentoesController : Controller
    {
        private readonly HumanAidDbContext _context;

        public AlimentoesController(HumanAidDbContext context)
        {
            _context = context;
        }

        // GET: Alimentoes
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 7;

            var alimentos = from a in _context.Alimento.Include(a => a.Envio)
                            select a;

            if (!string.IsNullOrEmpty(searchString))
            {
                alimentos = alimentos.Where(s => s.Tipo.Contains(searchString));
            }

            int totalItems = await alimentos.CountAsync();
            var items = await alimentos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewData["SearchString"] = searchString;

            return View(items);
        }


        // GET: Alimentoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alimento = await _context.Alimento
                .Include(a => a.Envio)
                .FirstOrDefaultAsync(m => m.AlimentoId == id);
            if (alimento == null)
            {
                return NotFound();
            }

            return View(alimento);
        }

        // GET: Alimentoes/Create
        public IActionResult Create()
        {
            ViewData["EnvioId"] = new SelectList(_context.Envio, "EnvioId", "Destino");
            return View();
        }

        // POST: Alimentoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlimentoId,EnvioId,Tipo,Peso")] Alimento alimento)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Add(alimento);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["success"] = "Donacion de alimentos creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error: {ex.Message} - {ex.InnerException?.Message}";
            }

            ViewData["EnvioId"] = new SelectList(_context.Envio, "EnvioId", "Destino", alimento.EnvioId);
            return View(alimento);
        }

        // GET: Alimentoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alimento = await _context.Alimento.FindAsync(id);
            if (alimento == null)
            {
                return NotFound();
            }
            ViewData["EnvioId"] = new SelectList(_context.Envio, "EnvioId", "Destino", alimento.EnvioId);
            return View(alimento);
        }

        // POST: Alimentoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlimentoId,EnvioId,Tipo,Peso")] Alimento alimento)
        {
            if (id != alimento.AlimentoId)
            {
                return NotFound();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Update(alimento);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["success"] = "Donacion de alimentos editada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                if (!AlimentoExists(alimento.AlimentoId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error: {ex.Message} - {ex.InnerException?.Message}";
            }

            ViewData["EnvioId"] = new SelectList(_context.Envio, "EnvioId", "Destino", alimento.EnvioId);
            return View(alimento);
        }

        // GET: Alimentoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alimento = await _context.Alimento
                .Include(a => a.Envio)
                .FirstOrDefaultAsync(m => m.AlimentoId == id);
            if (alimento == null)
            {
                return NotFound();
            }

            return View(alimento);
        }

        // POST: Alimentoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var alimento = await _context.Alimento.FindAsync(id);
                if (alimento != null)
                {
                    _context.Alimento.Remove(alimento);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["success"] = "Donación de alimentos eliminada exitosamente.";
                }
                else
                {
                    TempData["danger"] = "Donación de alimentos no encontrada.";
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error: {ex.Message} - {ex.InnerException?.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AlimentoExists(int id)
        {
            return _context.Alimento.Any(e => e.AlimentoId == id);
        }
    }
}
