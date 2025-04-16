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
    public class UsuariosController : Controller
    {
        private readonly HumanAidDbContext _context;

        public UsuariosController(HumanAidDbContext context)
        {
            _context = context;
        }

        // GET: Administrador/Usuarios
        public async Task<IActionResult> Index(string searchCorreo, int? RolId, int page = 1)
        {
            ViewBag.Roles = await _context.Rol.ToListAsync();

            var usuariosQuery = _context.Usuario.Include(u => u.Rol).AsQueryable();

            if (!string.IsNullOrEmpty(searchCorreo))
            {
                usuariosQuery = usuariosQuery.Where(u => u.Correo.Contains(searchCorreo));
            }

            if (RolId.HasValue)
            {
                usuariosQuery = usuariosQuery.Where(u => u.RolId == RolId.Value);
            }

            int pageSize = 7;
            int totalUsuarios = await usuariosQuery.CountAsync();
            int totalPages = (int)Math.Ceiling(totalUsuarios / (double)pageSize);

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["SearchCorreo"] = searchCorreo;
            ViewData["RolId"] = RolId;

            var paginatedUsuarios = await usuariosQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return View(paginatedUsuarios);
        }

        // GET: Administrador/Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Administrador/Usuarios/Create
        public IActionResult Create()
        {
            ViewData["RolId"] = new SelectList(_context.Rol, "RolId", "Nombre");
            return View();
        }

        // POST: Administrador/Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioId,Correo,Clave,RolId,FechaRegistro")] Usuario usuario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["success"] = "El usuario fue creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error al guardar el usuario: {ex.Message}";
                ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar el usuario.");
            }

            ViewData["RolId"] = new SelectList(_context.Rol, "RolId", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // GET: Administrador/Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["RolId"] = new SelectList(_context.Rol, "RolId", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // POST: Administrador/Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioId,Correo,Clave,RolId,FechaRegistro")] Usuario usuario)
        {
            if (id != usuario.UsuarioId)
            {
                TempData["danger"] = "El ID del usuario no coincide.";
                return NotFound();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var usuarioExistente = await _context.Usuario.FindAsync(id);

                if (usuarioExistente == null)
                {
                    TempData["danger"] = "El usuario no fue encontrado.";
                    return NotFound();
                }

                usuarioExistente.Correo = usuario.Correo;
                usuarioExistente.Clave = usuario.Clave;
                usuarioExistente.RolId = usuario.RolId;
                usuarioExistente.FechaRegistro = usuario.FechaRegistro;

                _context.Update(usuarioExistente);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["success"] = "El usuario fue actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error al actualizar el usuario: {ex.Message}";
                ModelState.AddModelError(string.Empty, "Ocurrió un error al actualizar el usuario.");
            }

            ViewData["RolId"] = new SelectList(_context.Rol, "RolId", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // GET: Administrador/Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Administrador/Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var usuario = await _context.Usuario.FindAsync(id);
                if (usuario == null)
                {
                    TempData["danger"] = "El usuario no fue encontrado.";
                    return NotFound();
                }

                _context.Usuario.Remove(usuario);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["success"] = "El usuario fue eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error al eliminar el usuario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.UsuarioId == id);
        }
    }
}
