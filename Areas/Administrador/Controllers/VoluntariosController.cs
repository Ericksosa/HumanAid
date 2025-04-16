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
    public class VoluntariosController : Controller
    {
        private readonly HumanAidDbContext _context;

        public VoluntariosController(HumanAidDbContext context)
        {
            _context = context;
        }

        // GET: Voluntarios
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 7;

            var voluntarios = from v in _context.Voluntario.Include(v => v.Sede).Include(v => v.Usuario)
                              select v;

            if (!string.IsNullOrEmpty(searchString))
            {
                voluntarios = voluntarios.Where(s => s.Nombre.Contains(searchString));
            }

            int totalItems = await voluntarios.CountAsync();
            var items = await voluntarios
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewData["SearchString"] = searchString;

            return View(items);
        }


        // GET: Voluntarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voluntario = await _context.Voluntario
                .Include(v => v.Sede)
                .Include(v => v.Usuario)
                .FirstOrDefaultAsync(m => m.VoluntarioId == id);
            if (voluntario == null)
            {
                return NotFound();
            }

            return View(voluntario);
        }

        // GET: Voluntarios/Create
        public IActionResult Create()
        {
            var usuariosNoVoluntarios = _context.Usuario
                .Where(u => !_context.Voluntario.Any(v => v.UsuarioId == u.UsuarioId))
                .Select(u => new { u.UsuarioId, u.Correo })
                .ToList();

            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Nombre");
            ViewData["UsuarioId"] = new SelectList(usuariosNoVoluntarios, "UsuarioId", "Correo");
            return View();
        }


        // POST: Voluntarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VoluntarioId,Nombre,Direccion,FechaNacimiento,SedeId,FechaRegistro,Email,Telefono,UsuarioId")] Voluntario voluntario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Add(voluntario);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["success"] = "Voluntario creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error: {ex.Message} - {ex.InnerException?.Message}";
            }

            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Nombre", voluntario.SedeId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "UsuarioId", "Correo", voluntario.UsuarioId);
            return View(voluntario);
        }





        // GET: Voluntarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voluntario = await _context.Voluntario.FindAsync(id);
            if (voluntario == null)
            {
                TempData["danger"] = "Voluntario no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var usuariosNoVoluntarios = _context.Usuario
                .Where(u => !_context.Voluntario.Any(v => v.UsuarioId == u.UsuarioId))
                .Select(u => new { u.UsuarioId, u.Correo })
                .ToList();

            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Nombre", voluntario.SedeId);
            ViewData["UsuarioId"] = new SelectList(usuariosNoVoluntarios, "UsuarioId", "Correo", voluntario.UsuarioId);
            return View(voluntario);
        }

        // POST: Voluntarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VoluntarioId,Nombre,Direccion,FechaNacimiento,SedeId,FechaRegistro,Email,Telefono,UsuarioId")] Voluntario voluntario)
        {
            if (id != voluntario.VoluntarioId)
            {
                return NotFound();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Update(voluntario);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["success"] = "Voluntario editado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                if (!VoluntarioExists(voluntario.VoluntarioId))
                {
                    TempData["danger"] = "Voluntario no encontrado.";
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

            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Nombre", voluntario.SedeId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "UsuarioId", "Correo", voluntario.UsuarioId);
            return View(voluntario);
        }


        // GET: Voluntarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voluntario = await _context.Voluntario
                .Include(v => v.Sede)
                .Include(v => v.Usuario)
                .FirstOrDefaultAsync(m => m.VoluntarioId == id);
            if (voluntario == null)
            {
                TempData["danger"] = "Voluntario no encontrado.";
            }

            return View(voluntario);
        }

        // POST: Voluntarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var voluntario = await _context.Voluntario.FindAsync(id);
                if (voluntario != null)
                {
                    _context.Voluntario.Remove(voluntario);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["success"] = "Voluntario eliminado exitosamente.";
                }
                else
                {
                    TempData["danger"] = "Voluntario no encontrado.";
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error: {ex.Message} - {ex.InnerException?.Message}";
            }

            return RedirectToAction(nameof(Index));
        }


        private bool VoluntarioExists(int id)
        {
            return _context.Voluntario.Any(e => e.VoluntarioId == id);
        }
    }
}
