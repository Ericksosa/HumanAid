using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HumanAid.Data;
using HumanAid.Models;

namespace HumanAid.Areas.Administrador.Controllers
{
    [Area("Administrador")]
    public class GastosController : Controller
    {
        private readonly HumanAidDbContext _context;

        public GastosController(HumanAidDbContext context)
        {
            _context = context;
        }

        // GET: Administrador/Gastos
        public async Task<IActionResult> Index()
        {
            var humanAidDbContext = _context.Gastos.Include(g => g.Sede);
            return View(await humanAidDbContext.ToListAsync());
        }

        // GET: Administrador/Gastos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gastos = await _context.Gastos
                .Include(g => g.Sede)
                .FirstOrDefaultAsync(m => m.IdGastos == id);
            if (gastos == null)
            {
                return NotFound();
            }

            return View(gastos);
        }

        // GET: Administrador/Gastos/Create
        public IActionResult Create()
        {
            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Ciudad");
            return View();
        }

        // POST: Administrador/Gastos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdGastos,Descripcion,Importe,FechaGasto,SedeId")] Gastos gastos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gastos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Ciudad", gastos.SedeId);
            return View(gastos);
        }

        // GET: Administrador/Gastos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gastos = await _context.Gastos.FindAsync(id);
            if (gastos == null)
            {
                return NotFound();
            }
            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Ciudad", gastos.SedeId);
            return View(gastos);
        }

        // POST: Administrador/Gastos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdGastos,Descripcion,Importe,FechaGasto,SedeId")] Gastos gastos)
        {
            if (id != gastos.IdGastos)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gastos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GastosExists(gastos.IdGastos))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Ciudad", gastos.SedeId);
            return View(gastos);
        }

        // GET: Administrador/Gastos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gastos = await _context.Gastos
                .Include(g => g.Sede)
                .FirstOrDefaultAsync(m => m.IdGastos == id);
            if (gastos == null)
            {
                return NotFound();
            }

            return View(gastos);
        }

        // POST: Administrador/Gastos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gastos = await _context.Gastos.FindAsync(id);
            if (gastos != null)
            {
                _context.Gastos.Remove(gastos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GastosExists(int id)
        {
            return _context.Gastos.Any(e => e.IdGastos == id);
        }
    }
}
