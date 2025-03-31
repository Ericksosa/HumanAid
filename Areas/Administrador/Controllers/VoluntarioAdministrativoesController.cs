using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HumanAid.Data;
using HumanAid.Models;
using Microsoft.AspNetCore.Authorization;

namespace HumanAid.Areas.Administrador.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Area("Administrador")]
    public class VoluntarioAdministrativoesController : Controller
    {
        private readonly HumanAidDbContext _context;

        public VoluntarioAdministrativoesController(HumanAidDbContext context)
        {
            _context = context;
        }

        // GET: VoluntarioAdministrativoes
        public async Task<IActionResult> Index()
        {
            var humanAidDbContext = _context.VoluntarioAdministrativo.Include(v => v.Voluntario);
            return View(await humanAidDbContext.ToListAsync());
        }

        // GET: VoluntarioAdministrativoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voluntarioAdministrativo = await _context.VoluntarioAdministrativo
                .Include(v => v.Voluntario)
                .FirstOrDefaultAsync(m => m.VoluntarioAdministrativoId == id);
            if (voluntarioAdministrativo == null)
            {
                return NotFound();
            }

            return View(voluntarioAdministrativo);
        }

        // GET: VoluntarioAdministrativoes/Create
        public IActionResult Create()
        {
            ViewData["VoluntarioId"] = new SelectList(_context.Voluntario, "VoluntarioId", "Direccion");
            return View();
        }

        // POST: VoluntarioAdministrativoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Profesion,Departamento,VoluntarioId")] VoluntarioAdministrativo voluntarioAdministrativo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Add(voluntarioAdministrativo);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message} - {ex.InnerException?.Message}");
            }

            ViewData["VoluntarioId"] = new SelectList(_context.Voluntario, "VoluntarioId", "Direccion", voluntarioAdministrativo.VoluntarioId);
            return View(voluntarioAdministrativo);
        }

        // GET: VoluntarioAdministrativoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voluntarioAdministrativo = await _context.VoluntarioAdministrativo.FindAsync(id);
            if (voluntarioAdministrativo == null)
            {
                return NotFound();
            }
            ViewData["VoluntarioId"] = new SelectList(_context.Voluntario, "VoluntarioId", "Direccion", voluntarioAdministrativo.VoluntarioId);
            return View(voluntarioAdministrativo);
        }

        // POST: VoluntarioAdministrativoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VoluntarioAdministrativoId,Profesion,Departamento,VoluntarioId")] VoluntarioAdministrativo voluntarioAdministrativo)
        {
            if (id != voluntarioAdministrativo.VoluntarioAdministrativoId)
            {
                return NotFound();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Update(voluntarioAdministrativo);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                if (!VoluntarioAdministrativoExists(voluntarioAdministrativo.VoluntarioAdministrativoId))
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
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message} - {ex.InnerException?.Message}");
            }

            ViewData["VoluntarioId"] = new SelectList(_context.Voluntario, "VoluntarioId", "Direccion", voluntarioAdministrativo.VoluntarioId);
            return View(voluntarioAdministrativo);
        }

        // GET: VoluntarioAdministrativoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voluntarioAdministrativo = await _context.VoluntarioAdministrativo
                .Include(v => v.Voluntario)
                .FirstOrDefaultAsync(m => m.VoluntarioAdministrativoId == id);
            if (voluntarioAdministrativo == null)
            {
                return NotFound();
            }

            return View(voluntarioAdministrativo);
        }

        // POST: VoluntarioAdministrativoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var voluntarioAdministrativo = await _context.VoluntarioAdministrativo.FindAsync(id);
            if (voluntarioAdministrativo != null)
            {
                _context.VoluntarioAdministrativo.Remove(voluntarioAdministrativo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VoluntarioAdministrativoExists(int id)
        {
            return _context.VoluntarioAdministrativo.Any(e => e.VoluntarioAdministrativoId == id);
        }
    }
}
