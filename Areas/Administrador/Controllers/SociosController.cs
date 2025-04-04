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
        public async Task<IActionResult> Index()
        {
            var humanAidDbContext = _context.Socio.Include(s => s.Sede).Include(s => s.TipoCuota).Include(s => s.Usuario);
            return View(await humanAidDbContext.ToListAsync());
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
            var usuariosDisponibles = _context.Usuario
                .Where(u =>
                    !_context.Socio.Any(s => s.UsuarioId == u.UsuarioId) &&
                    !_context.Voluntario.Any(v => v.UsuarioId == u.UsuarioId)
                )
                .ToList();

            if (!usuariosDisponibles.Any())
            {
                TempData["NoUsuariosDisponibles"] = "No hay usuarios disponibles para asignar como socios.";
            }

            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Nombre");
            ViewData["TipoCuotaId"] = new SelectList(_context.TipoCuota, "TipoCuotaId", "Nombre");
            ViewData["UsuarioId"] = new SelectList(usuariosDisponibles, "UsuarioId", "Correo");
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
            ViewData["UsuarioId"] = new SelectList(
                _context.Usuario
                    .Where(u =>
                        !_context.Socio.Any(s => s.UsuarioId == u.UsuarioId) &&
                        !_context.Voluntario.Any(v => v.UsuarioId == u.UsuarioId)),
                "UsuarioId", "Correo", socio.UsuarioId
            );

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (_context.Socio.Any(s => s.UsuarioId == socio.UsuarioId) ||
                    _context.Voluntario.Any(v => v.UsuarioId == socio.UsuarioId))
                {
                    ModelState.AddModelError("UsuarioId", "El usuario seleccionado ya está asignado a otro socio o voluntario.");
                    return View(socio);
                }

                _context.Add(socio);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error al guardar el socio: {ex.Message}");
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

            var usuariosDisponibles = _context.Usuario
                .Where(u =>
                    (!_context.Socio.Any(s => s.UsuarioId == u.UsuarioId) &&
                     !_context.Voluntario.Any(v => v.UsuarioId == u.UsuarioId)) ||
                     u.UsuarioId == socio.UsuarioId
                )
                .ToList();

            if (!usuariosDisponibles.Any())
            {
                ViewData["NoUsuariosDisponibles"] = "No hay usuarios disponibles para asignar como socios.";
            }

            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Nombre", socio.SedeId);
            ViewData["TipoCuotaId"] = new SelectList(_context.TipoCuota, "TipoCuotaId", "Nombre", socio.TipoCuotaId);
            ViewData["UsuarioId"] = new SelectList(usuariosDisponibles, "UsuarioId", "Correo", socio.UsuarioId);
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
                return NotFound();
            }

            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Nombre", socio.SedeId);
            ViewData["TipoCuotaId"] = new SelectList(_context.TipoCuota, "TipoCuotaId", "Nombre", socio.TipoCuotaId);
            ViewData["UsuarioId"] = new SelectList(
                _context.Usuario
                    .Where(u =>
                        (!_context.Socio.Any(s => s.UsuarioId == u.UsuarioId) &&
                         !_context.Voluntario.Any(v => v.UsuarioId == u.UsuarioId)) ||
                        u.UsuarioId == socio.UsuarioId), 
                "UsuarioId", "Correo", socio.UsuarioId
            );

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (_context.Socio.Any(s => s.UsuarioId == socio.UsuarioId && s.SocioId != socio.SocioId) ||
                    _context.Voluntario.Any(v => v.UsuarioId == socio.UsuarioId))
                {
                    ModelState.AddModelError("UsuarioId", "El usuario seleccionado ya está asignado a otro socio o voluntario.");
                    return View(socio);
                }

                _context.Update(socio);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error al actualizar el socio: {ex.Message}");
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
            var socio = await _context.Socio.FindAsync(id);
            if (socio != null)
            {
                _context.Socio.Remove(socio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SocioExists(int id)
        {
            return _context.Socio.Any(e => e.SocioId == id);
        }
    }
}
