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

        public async Task<IActionResult> Index()
        {
            return View(await _context.Sede.ToListAsync());
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
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(string.Empty, $"Error al crear la sede: {ex.Message}");
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
                    ModelState.AddModelError(string.Empty, $"Error al editar la sede: {ex.Message}");
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
            var sede = await _context.Sede.FindAsync(id);
            if (sede != null)
            {
                _context.Sede.Remove(sede);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SedeExists(int id)
        {
            return _context.Sede.Any(e => e.SedeId == id);
        }
    }
}