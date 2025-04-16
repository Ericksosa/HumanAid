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
    public class SedesController : Controller
    {
        private readonly HumanAidDbContext _context;

        public SedesController(HumanAidDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, string searchString = null)
        {
            var sedes = from s in _context.Sede
                        select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                sedes = sedes.Where(s => s.Nombre.Contains(searchString));
            }

            int pageSize = 7;
            int totalSedes = sedes.Count();
            int totalPages = (int)Math.Ceiling(totalSedes / (double)pageSize);

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["SearchString"] = searchString;

            var paginatedSedes = sedes.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View(paginatedSedes);

            //return View(await _context.Sede.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sede = await _context.Sede
                .Include(m => m.Socios)
                .FirstOrDefaultAsync(m => m.SedeId == id);
            if (sede == null)
            {
                return NotFound();
            }

            return View(sede);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SedeId,Nombre,Ciudad,Direccion,Director,FechaFundacion")] Sede sede)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (string.IsNullOrEmpty(sede.Nombre) || string.IsNullOrEmpty(sede.Ciudad) || string.IsNullOrEmpty(sede.Direccion) || string.IsNullOrEmpty(sede.Director) || sede.FechaFundacion == default)
                    {
                        throw new ArgumentException("Todos los campos son obligatorios.");
                    }

                    _context.Add(sede);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["success"] = "Sede creada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["danger"] = $"Ocurrió un error: {ex.Message} - {ex.InnerException?.Message}";
                }
            }
            return View(sede);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sede = await _context.Sede.FindAsync(id);
            if (sede == null)
            {
                return NotFound();
            }
            return View(sede);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SedeId,Nombre,Ciudad,Direccion,Director,FechaFundacion")] Sede sede)
        {
            if (id != sede.SedeId)
            {
                return NotFound();
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (string.IsNullOrEmpty(sede.Nombre) || string.IsNullOrEmpty(sede.Ciudad) || string.IsNullOrEmpty(sede.Direccion) || string.IsNullOrEmpty(sede.Director) || sede.FechaFundacion == default)
                    {
                        throw new ArgumentException("Todos los campos son obligatorios.");
                    }

                    _context.Update(sede);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["success"] = "Sede editada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    await transaction.RollbackAsync();
                    if (!SedeExists(sede.SedeId))
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
            }
            return View(sede);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sede = await _context.Sede
                .FirstOrDefaultAsync(m => m.SedeId == id);
            if (sede == null)
            {
                return NotFound();
            }

            return View(sede);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sede = await _context.Sede
                .Include(s => s.EnvioSedes)
                .FirstOrDefaultAsync(s => s.SedeId == id);

            if (sede == null)
            {
                TempData["danger"] = "Sede no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            if (sede.EnvioSedes != null && sede.EnvioSedes.Any())
            {
                TempData["danger"] = "No se puede eliminar la sede porque está asociada a uno o más envíos.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            try
            {
                _context.Sede.Remove(sede);
                await _context.SaveChangesAsync();
                TempData["success"] = "Sede eliminada exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["danger"] = $"Ocurrió un error: {ex.Message} - {ex.InnerException?.Message}";
            }

            return RedirectToAction(nameof(Index));
        }


        private bool SedeExists(int id)
        {
            return _context.Sede.Any(e => e.SedeId == id);
        }
    }
}