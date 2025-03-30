using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HumanAid.Data;
using HumanAid.Models;

namespace HumanAid.Controllers
{
    public class VoluntarioSanitariosController : Controller
    {
        private readonly HumanAidDbContext _context;

        public VoluntarioSanitariosController(HumanAidDbContext context)
        {
            _context = context;
        }

        // GET: VoluntarioSanitarios
        public async Task<IActionResult> Index()
        {
            var humanAidDbContext = _context.VoluntarioSanitario.Include(v => v.Voluntario);
            return View(await humanAidDbContext.ToListAsync());
        }

        // GET: VoluntarioSanitarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voluntarioSanitario = await _context.VoluntarioSanitario
                .Include(v => v.Voluntario)
                .FirstOrDefaultAsync(m => m.VoluntarioSanitarioId == id);
            if (voluntarioSanitario == null)
            {
                return NotFound();
            }

            return View(voluntarioSanitario);
        }

        // GET: VoluntarioSanitarios/Create
        public IActionResult Create()
        {
            ViewData["VoluntarioId"] = new SelectList(_context.Voluntario, "VoluntarioId", "Direccion");
            return View();
        }

        // POST: VoluntarioSanitarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Profesion,Disponibilidad,NumeroTrabajosRealizados,VoluntarioId")] VoluntarioSanitario voluntarioSanitario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Add(voluntarioSanitario);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message} - {ex.InnerException?.Message}");
            }

            ViewData["VoluntarioId"] = new SelectList(_context.Voluntario, "VoluntarioId", "Direccion", voluntarioSanitario.VoluntarioId);
            return View(voluntarioSanitario);
        }



        // GET: VoluntarioSanitarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voluntarioSanitario = await _context.VoluntarioSanitario.FindAsync(id);
            if (voluntarioSanitario == null)
            {
                return NotFound();
            }
            ViewData["VoluntarioId"] = new SelectList(_context.Voluntario, "VoluntarioId", "Direccion", voluntarioSanitario.VoluntarioId);
            return View(voluntarioSanitario);
        }

        // POST: VoluntarioSanitarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VoluntarioSanitarioId,Profesion,Disponibilidad,NumeroTrabajosRealizados,VoluntarioId")] VoluntarioSanitario voluntarioSanitario)
        {
            if (id != voluntarioSanitario.VoluntarioSanitarioId)
            {
                return NotFound();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Update(voluntarioSanitario);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                if (!VoluntarioSanitarioExists(voluntarioSanitario.VoluntarioSanitarioId))
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

            ViewData["VoluntarioId"] = new SelectList(_context.Voluntario, "VoluntarioId", "Direccion", voluntarioSanitario.VoluntarioId);
            return View(voluntarioSanitario);
        }





        // GET: VoluntarioSanitarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voluntarioSanitario = await _context.VoluntarioSanitario
                .Include(v => v.Voluntario)
                .FirstOrDefaultAsync(m => m.VoluntarioSanitarioId == id);
            if (voluntarioSanitario == null)
            {
                return NotFound();
            }

            return View(voluntarioSanitario);
        }

        // POST: VoluntarioSanitarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var voluntarioSanitario = await _context.VoluntarioSanitario.FindAsync(id);
            if (voluntarioSanitario != null)
            {
                _context.VoluntarioSanitario.Remove(voluntarioSanitario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VoluntarioSanitarioExists(int id)
        {
            return _context.VoluntarioSanitario.Any(e => e.VoluntarioSanitarioId == id);
        }
    }
}
