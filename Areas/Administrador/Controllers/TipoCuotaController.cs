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
    public class TipoCuotaController : Controller
    {
        private readonly HumanAidDbContext _context;

        public TipoCuotaController(HumanAidDbContext context)
        {
            _context = context;
        }

        public IActionResult AsignarSocios(int id)
        {
            var cuota = _context.TipoCuota.Find(id);
            if (cuota == null)
            {
                return NotFound();
            }

            ViewBag.Socios = _context.Socio
                            .Where(s => s.TipoCuotaId != id) // Aqui, se buscan los socios que no tienen la cuota seleccionada
                            .ToList();
            return View(cuota);
        }

        [HttpPost]
        public async Task<IActionResult> AsignarSocios(int tipoCuotaId, List<int> socioIds)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Buscar el tipo de cuota con sus socios
                var tipoCuota = await _context.TipoCuota
                                              .Include(tc => tc.Socios)
                                              .FirstOrDefaultAsync(tc => tc.TipoCuotaId == tipoCuotaId);

                if (tipoCuota == null)
                {
                    TempData["danger"] = "No se encontró la cuota.";
                    return RedirectToAction("Index");
                }

                // Buscar los socios seleccionados
                var sociosSeleccionados = await _context.Socio
                                                         .Where(s => socioIds.Contains(s.SocioId))
                                                         .ToListAsync();

                // Asignar la cuota a los socios seleccionados
                foreach (var socio in sociosSeleccionados)
                {
                    socio.TipoCuotaId = tipoCuota.TipoCuotaId;
                }

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                TempData["success"] = "Cuota asignado a socios correctamente.";
            }
            catch (Exception)
            {
                TempData["danger"] = "Ocurrió un error al asignar los socios.";
            }

            // Redirigir de vuelta a la lista
            return RedirectToAction("Index");
        }


        // GET: Administrador/TipoCuota
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoCuota.ToListAsync());
        }

        // GET: Administrador/TipoCuota/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoCuota = await _context.TipoCuota
                .FirstOrDefaultAsync(m => m.TipoCuotaId == id);
            if (tipoCuota == null)
            {
                return NotFound();
            }
            ViewBag.TiposCuota = new SelectList(_context.TipoCuota, "TipoCuotaId", "Nombre");
            return View(tipoCuota);
        }

        // GET: Administrador/TipoCuota/Create
        public IActionResult Create()
        {
            ViewBag.Socios = _context.Socio.ToList();
            return View();
        }

        // POST: Administrador/TipoCuota/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TipoCuotaId,Nombre,Importe,Descripcion")] TipoCuota tipoCuota)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Add(tipoCuota);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["success"] = "Cuota creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error al crear la cuota: {ex.Message}");
                TempData["danger"] = "Error al crear la cuota: " + ex.Message;
                return StatusCode(500, "Error al procesar la solicitud.");
            }
        }


        // GET: Administrador/TipoCuota/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoCuota = await _context.TipoCuota.FindAsync(id);
            if (tipoCuota == null)
            {
                return NotFound();
            }
            ViewBag.Socios = _context.Socio.ToList();
            return View(tipoCuota);
        }

        // POST: Administrador/TipoCuota/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TipoCuotaId,Nombre,Importe,Descripcion")] TipoCuota tipoCuota)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            if (id != tipoCuota.TipoCuotaId)
            {
                return NotFound();
            }

            try
            {
                _context.Update(tipoCuota);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["success"] = "Cuota actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine($"Error de concurrencia: {ex.Message}");
                if (!TipoCuotaExists(tipoCuota.TipoCuotaId))
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode(500, "Error al actualizar la cuota.");
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = "Error al actualizar la cuota: " + ex.Message;
                Console.WriteLine($"Error inesperado al actualizar la cuota: {ex.Message}");
                return StatusCode(500, "Error inesperado al actualizar la cuota.");
            }
        }


        // GET: Administrador/TipoCuota/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoCuota = await _context.TipoCuota
                .FirstOrDefaultAsync(m => m.TipoCuotaId == id);
            if (tipoCuota == null)
            {
                return NotFound();
            }

            return View(tipoCuota);
        }

        // POST: Administrador/TipoCuota/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var tipoCuota = await _context.TipoCuota.FindAsync(id);
                if (tipoCuota == null)
                {
                    return NotFound();
                }

                _context.TipoCuota.Remove(tipoCuota);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["success"] = "Cuota eliminada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la cuota: {ex.Message}");
                return StatusCode(500, "Error al eliminar la cuota.");
                TempData["danger"] = "Error al eliminar la cuota: " + ex.Message;
            }
        }

        private bool TipoCuotaExists(int id)
        {
            return _context.TipoCuota.Any(e => e.TipoCuotaId == id);
        }
    }
}
