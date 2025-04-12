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
    public class MedicamentoesController : Controller
    {
        private readonly HumanAidDbContext _context;

        public MedicamentoesController(HumanAidDbContext context)
        {
            _context = context;
        }

        // GET: Medicamentoes
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 7;

            var medicamentos = from m in _context.Medicamento.Include(m => m.Envio)
                               select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                medicamentos = medicamentos.Where(s => s.Nombre.Contains(searchString));
            }

            int totalItems = await medicamentos.CountAsync();
            var items = await medicamentos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewData["SearchString"] = searchString;

            return View(items);
        }


        // GET: Medicamentoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicamento = await _context.Medicamento
                .Include(m => m.Envio)
                .FirstOrDefaultAsync(m => m.MedicamentoId == id);
            if (medicamento == null)
            {
                return NotFound();
            }

            return View(medicamento);
        }

        // GET: Medicamentoes/Create
        public IActionResult Create()
        {
            ViewData["EnvioId"] = new SelectList(_context.Envio, "EnvioId", "Destino");
            return View();
        }

        // POST: Medicamentoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MedicamentoId,EnvioId,Nombre,Dosis,cantidad")] Medicamento medicamento)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Add(medicamento);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["success"] = "Donación de medicamentos creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error: {ex.Message} - {ex.InnerException?.Message}";
            }

            ViewData["EnvioId"] = new SelectList(_context.Envio, "EnvioId", "Destino", medicamento.EnvioId);
            return View(medicamento);
        }

        // GET: Medicamentoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicamento = await _context.Medicamento.FindAsync(id);
            if (medicamento == null)
            {
                return NotFound();
            }
            ViewData["EnvioId"] = new SelectList(_context.Envio, "EnvioId", "Destino", medicamento.EnvioId);
            return View(medicamento);
        }

        // POST: Medicamentoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MedicamentoId,EnvioId,Nombre,Dosis,cantidad")] Medicamento medicamento)
        {
            if (id != medicamento.MedicamentoId)
            {
                return NotFound();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Update(medicamento);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["success"] = "Donación de medicamentos editada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                if (!MedicamentoExists(medicamento.MedicamentoId))
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

            ViewData["EnvioId"] = new SelectList(_context.Envio, "EnvioId", "Destino", medicamento.EnvioId);
            return View(medicamento);
        }

        // GET: Medicamentoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicamento = await _context.Medicamento
                .Include(m => m.Envio)
                .FirstOrDefaultAsync(m => m.MedicamentoId == id);
            if (medicamento == null)
            {
                return NotFound();
            }

            return View(medicamento);
        }

        // POST: Medicamentoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var medicamento = await _context.Medicamento.FindAsync(id);
                if (medicamento != null)
                {
                    _context.Medicamento.Remove(medicamento);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["success"] = "Donación de medicamentos eliminada exitosamente.";
                }
                else
                {
                    TempData["danger"] = "Donación de medicamentos no encontrada.";
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error: {ex.Message} - {ex.InnerException?.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
        private bool MedicamentoExists(int id)
        {
            return _context.Medicamento.Any(e => e.MedicamentoId == id);
        }
    }
}
