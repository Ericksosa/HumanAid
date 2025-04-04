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
    public class RolesController : Controller
    {
        private readonly HumanAidDbContext _context;

        public RolesController(HumanAidDbContext context)
        {
            _context = context;
        }

        // GET: Administrador/Roles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Rol.ToListAsync());
        }

        // GET: Administrador/Roles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rol = await _context.Rol
                .FirstOrDefaultAsync(m => m.RolId == id);
            if (rol == null)
            {
                return NotFound();
            }

            return View(rol);
        }

        // GET: Administrador/Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Administrador/Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RolId,Nombre,FechaRegistro")] Rol rol)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Add(rol);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error al crear el rol: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al crear el rol.");
                return View(rol);
            }
        }

        // GET: Administrador/Roles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rol = await _context.Rol.FindAsync(id);
            if (rol == null)
            {
                return NotFound();
            }
            return View(rol);
        }

        // POST: Administrador/Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RolId,Nombre,FechaRegistro")] Rol rol)
        {
            if (id != rol.RolId)
            {
                return NotFound();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Update(rol);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error al editar el rol: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al editar el rol.");
                return View(rol);
            }
        }

        // GET: Administrador/Roles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rol = await _context.Rol
                .FirstOrDefaultAsync(m => m.RolId == id);
            if (rol == null)
            {
                return NotFound();
            }

            return View(rol);
        }

        // POST: Administrador/Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var rol = await _context.Rol.FindAsync(id);
                if (rol != null)
                {
                    _context.Rol.Remove(rol);
                    await _context.SaveChangesAsync();
                }
                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error al eliminar el rol: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al eliminar el rol.");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
