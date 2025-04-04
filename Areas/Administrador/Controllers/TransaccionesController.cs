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
    public class TransaccionesController : Controller
    {
        private readonly HumanAidDbContext _context;

        public TransaccionesController(HumanAidDbContext context)
        {
            _context = context;
        }

        // GET: Administrador/Transacciones
        public async Task<IActionResult> Index()
        {
            var humanAidDbContext = _context.Transaccion.Include(t => t.Socio).Include(t => t.TipoCuota);
            return View(await humanAidDbContext.ToListAsync());
        }

        // GET: Administrador/Transacciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaccion = await _context.Transaccion
                .Include(t => t.Socio)
                .Include(t => t.TipoCuota)
                .FirstOrDefaultAsync(m => m.TransaccionId == id);
            if (transaccion == null)
            {
                return NotFound();
            }

            return View(transaccion);
        }

        // GET: Administrador/Transacciones/Create
        public IActionResult Create()
        {
            ViewData["SocioId"] = new SelectList(_context.Socio, "SocioId", "Nombre");
            ViewData["TipoCuotaId"] = new SelectList(_context.TipoCuota, "TipoCuotaId", "Nombre");
            return View();
        }

        // POST: Administrador/Transacciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransaccionId,Monto,Metodo,Referencia,FechaPago,Tipo,Descripcion,SocioId,TipoCuotaId")] Transaccion transaccion)
        {
            ViewData["SocioId"] = new SelectList(_context.Socio, "SocioId", "Nombre", transaccion.SocioId);
            ViewData["TipoCuotaId"] = new SelectList(_context.TipoCuota, "TipoCuotaId", "Nombre", transaccion.TipoCuotaId);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Add(transaccion);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error al crear la transacción: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar la transacción.");
                return View(transaccion);
            }
        }

        // GET: Administrador/Transacciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaccion = await _context.Transaccion.FindAsync(id);
            if (transaccion == null)
            {
                return NotFound();
            }
            ViewData["SocioId"] = new SelectList(_context.Socio, "SocioId", "Nombre", transaccion.SocioId);
            ViewData["TipoCuotaId"] = new SelectList(_context.TipoCuota, "TipoCuotaId", "Nombre", transaccion.TipoCuotaId);
            return View(transaccion);
        }

        // POST: Administrador/Transacciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransaccionId,Monto,Metodo,Referencia,FechaPago,Tipo,Descripcion,SocioId,TipoCuotaId")] Transaccion transaccion)
        {
            if (id != transaccion.TransaccionId)
            {
                return NotFound();
            }

            ViewData["SocioId"] = new SelectList(_context.Socio, "SocioId", "Nombre", transaccion.SocioId);
            ViewData["TipoCuotaId"] = new SelectList(_context.TipoCuota, "TipoCuotaId", "Nombre", transaccion.TipoCuotaId);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Update(transaccion);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error al editar la transacción: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los cambios.");
                return View(transaccion);
            }
        }

        // GET: Administrador/Transacciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaccion = await _context.Transaccion
                .Include(t => t.Socio)
                .Include(t => t.TipoCuota)
                .FirstOrDefaultAsync(m => m.TransaccionId == id);
            if (transaccion == null)
            {
                return NotFound();
            }

            return View(transaccion);
        }

        // POST: Administrador/Transacciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var transaccion = await _context.Transaccion.FindAsync(id);
                if (transaccion != null)
                {
                    _context.Transaccion.Remove(transaccion);
                    await _context.SaveChangesAsync();
                }
                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error al eliminar la transacción: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al eliminar la transacción.");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
