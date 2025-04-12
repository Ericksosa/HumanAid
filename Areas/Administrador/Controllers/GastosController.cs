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
        public async Task<IActionResult> Index(int? SedeId)
        {
            ViewBag.Sedes = await _context.Sede.ToListAsync();

            var gastosQuery = _context.Gastos.Include(g => g.Sede).AsQueryable();

            if (SedeId.HasValue)
            {
                gastosQuery = gastosQuery.Where(g => g.SedeId == SedeId.Value);
            }

            var gastos = await gastosQuery.ToListAsync();
            return View(gastos);
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
            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Nombre");
            return View();
        }

        // POST: Administrador/Gastos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdGastos,Descripcion,Importe,FechaGasto,SedeId")] Gastos gastos)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var totalPagosPorSede = await _context.Pago
                    .Include(p => p.TipoCuota)
                    .Include(p => p.Socio)
                    .Where(p => p.Socio.SedeId == gastos.SedeId)
                    .SumAsync(p => p.TipoCuota.Importe * p.CantidadCuotas);

                var totalGastosPorSede = await _context.Gastos
                    .Where(g => g.SedeId == gastos.SedeId)
                    .SumAsync(g => g.Importe);

                var fondosDisponiblesPorSede = totalPagosPorSede - totalGastosPorSede;

                if (fondosDisponiblesPorSede < gastos.Importe)
                {
                    TempData["danger"] = "Fondos insuficientes para realizar este gasto en la sede seleccionada.";
                    ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Nombre", gastos.SedeId);
                    return View(gastos);
                }

                _context.Add(gastos);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["success"] = "El gasto se creó correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error al guardar el gasto: {ex.Message}";
                ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar el gasto.");
            }

            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Nombre", gastos.SedeId);
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
            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Nombre", gastos.SedeId);
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
                TempData["danger"] = "El ID del gasto no coincide.";
                return NotFound();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var gastoExistente = await _context.Gastos.FindAsync(id);

                if (gastoExistente == null)
                {
                    TempData["danger"] = "El gasto no fue encontrado.";
                    return NotFound();
                }

                var totalPagosPorSede = await _context.Pago
                    .Include(p => p.TipoCuota)
                    .Include(p => p.Socio)
                    .Where(p => p.Socio.SedeId == gastos.SedeId)
                    .SumAsync(p => p.TipoCuota.Importe * p.CantidadCuotas);

                var totalGastosPorSede = await _context.Gastos
                    .Where(g => g.SedeId == gastos.SedeId && g.IdGastos != gastos.IdGastos)
                    .SumAsync(g => g.Importe);

                var fondosDisponiblesPorSede = totalPagosPorSede - totalGastosPorSede;

                if (fondosDisponiblesPorSede < gastos.Importe)
                {
                    TempData["danger"] = "Fondos insuficientes para realizar este gasto en la sede seleccionada.";
                    ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Nombre", gastos.SedeId);
                    return View(gastos);
                }

                gastoExistente.Descripcion = gastos.Descripcion;
                gastoExistente.Importe = gastos.Importe;
                gastoExistente.FechaGasto = gastos.FechaGasto;
                gastoExistente.SedeId = gastos.SedeId;

                _context.Update(gastoExistente);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["success"] = "El gasto se actualizó correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error al actualizar el gasto: {ex.Message}";
                ModelState.AddModelError(string.Empty, "Ocurrió un error al actualizar el gasto.");
            }

            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Nombre", gastos.SedeId);
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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var gastos = await _context.Gastos.FindAsync(id);
                if (gastos == null)
                {
                    TempData["danger"] = "El gasto no fue encontrado.";
                    return NotFound();
                }

                _context.Gastos.Remove(gastos);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["success"] = "El gasto fue eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error al eliminar el gasto: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool GastosExists(int id)
        {
            return _context.Gastos.Any(e => e.IdGastos == id);
        }
    }
}
