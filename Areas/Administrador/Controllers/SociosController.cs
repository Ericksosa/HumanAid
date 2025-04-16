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
    public class SociosController : Controller
    {
        private readonly HumanAidDbContext _context;

        public SociosController(HumanAidDbContext context)
        {
            _context = context;
        }

        // GET: Socio/Socios
        public async Task<IActionResult> Index(int page = 1, string searchNombre = null)
        {
            var sociosQuery = _context.Socio
                .Include(s => s.Sede)
                .Include(s => s.TipoCuota)
                .Include(s => s.Usuario)
                .AsQueryable();

            // Filtrar por nombre si el parámetro no está vacío
            if (!string.IsNullOrEmpty(searchNombre))
            {
                sociosQuery = sociosQuery.Where(s => s.Nombre.Contains(searchNombre));
            }

            int pageSize = 7;
            int totalSocios = await sociosQuery.CountAsync();
            int totalPages = (int)Math.Ceiling(totalSocios / (double)pageSize);

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["SearchNombre"] = searchNombre;

            var paginatedSocios = await sociosQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return View(paginatedSocios);
        }

        // GET: Socio/Socios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var socio = await _context.Socio
                .Include(s => s.Sede)
                .Include(s => s.TipoCuota)
                .Include(s => s.Usuario)
                .FirstOrDefaultAsync(m => m.SocioId == id);
            if (socio == null)
            {
                return NotFound();
            }

            return View(socio);
        }

        // GET: Socio/Socios/Create
        public IActionResult Create()
        {
            var usuariosSociosDisponibles = _context.Usuario
                .Include(u => u.Rol)
                .Where(u => u.Rol.Nombre == "Socio" && !_context.Socio.Any(s => s.UsuarioId == u.UsuarioId))
                .ToList();

            if (!usuariosSociosDisponibles.Any())
            {
                TempData["Alerta"] = "No hay usuarios disponibles para asignar como socios. Por favor, crea usuarios primero.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Nombre");
            ViewData["TipoCuotaId"] = new SelectList(_context.TipoCuota, "TipoCuotaId", "Nombre");
            ViewData["UsuarioId"] = new SelectList(usuariosSociosDisponibles, "UsuarioId", "Correo");

            return View();
        }

        // POST: Socio/Socios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SocioId,Nombre,FechaNacimiento,CuentaBancaria,FechaPago,TipoCuotaId,SedeId,UsuarioId,FechaRegistro")] Models.Socio socio)
        {
            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Nombre", socio.SedeId);
            ViewData["TipoCuotaId"] = new SelectList(_context.TipoCuota, "TipoCuotaId", "Nombre", socio.TipoCuotaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "UsuarioId", "Correo", socio.UsuarioId);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Add(socio);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["success"] = "El socio fue creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error al guardar el socio: {ex.Message}";
                ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar el socio.");
                return View(socio);
            }
        }

        // GET: Socio/Socios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var socio = await _context.Socio.FindAsync(id);
            if (socio == null)
            {
                return NotFound();
            }
            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Nombre", socio.SedeId);
            ViewData["TipoCuotaId"] = new SelectList(_context.TipoCuota, "TipoCuotaId", "Nombre", socio.TipoCuotaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "UsuarioId", "Correo", socio.UsuarioId);
            return View(socio);
        }

        // POST: Socio/Socios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SocioId,Nombre,FechaNacimiento,CuentaBancaria,FechaPago,TipoCuotaId,SedeId,UsuarioId,FechaRegistro")] Models.Socio socio)
        {
            if (id != socio.SocioId)
            {
                TempData["danger"] = "El ID del socio no coincide.";
                return NotFound();
            }

            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Nombre", socio.SedeId);
            ViewData["TipoCuotaId"] = new SelectList(_context.TipoCuota, "TipoCuotaId", "Nombre", socio.TipoCuotaId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "UsuarioId", "Correo", socio.UsuarioId);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Update(socio);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["success"] = "El socio fue actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error al actualizar el socio: {ex.Message}";
                ModelState.AddModelError(string.Empty, "Ocurrió un error al actualizar el socio.");
                return View(socio);
            }
        }

        // GET: Socio/Socios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var socio = await _context.Socio
                .Include(s => s.Sede)
                .Include(s => s.TipoCuota)
                .Include(s => s.Usuario)
                .FirstOrDefaultAsync(m => m.SocioId == id);
            if (socio == null)
            {
                return NotFound();
            }

            return View(socio);
        }

        // POST: Socio/Socios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var socio = await _context.Socio.FindAsync(id);
                if (socio == null)
                {
                    TempData["danger"] = "El socio no fue encontrado.";
                    return NotFound();
                }

                _context.Socio.Remove(socio);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["success"] = "El socio fue eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error al eliminar el socio: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool SocioExists(int id)
        {
            return _context.Socio.Any(e => e.SocioId == id);
        }
    }
}
