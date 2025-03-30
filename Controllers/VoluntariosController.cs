using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HumanAid.Data;
using HumanAid.Models;

namespace HumanAid.Controllers
{
    public class VoluntariosController : Controller
    {
        private readonly HumanAidDbContext _context;

        public VoluntariosController(HumanAidDbContext context)
        {
            _context = context;
        }

        // GET: Voluntarios
        public async Task<IActionResult> Index()
        {
            var humanAidDbContext = _context.Voluntario.Include(v => v.Sede).Include(v => v.Usuario);
            return View(await humanAidDbContext.ToListAsync());
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
            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Ciudad");
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "UsuarioId", "Clave");
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
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message} - {ex.InnerException?.Message}");
            }

            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Ciudad", voluntario.SedeId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "UsuarioId", "Clave", voluntario.UsuarioId);
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
                return NotFound();
            }
            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Ciudad", voluntario.SedeId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "UsuarioId", "Clave", voluntario.UsuarioId);
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
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                if (!VoluntarioExists(voluntario.VoluntarioId))
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
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            }

            ViewData["SedeId"] = new SelectList(_context.Sede, "SedeId", "Ciudad", voluntario.SedeId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "UsuarioId", "Clave", voluntario.UsuarioId);
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
                return NotFound();
            }

            return View(voluntario);
        }

        // POST: Voluntarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var voluntario = await _context.Voluntario.FindAsync(id);
            if (voluntario != null)
            {
                _context.Voluntario.Remove(voluntario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VoluntarioExists(int id)
        {
            return _context.Voluntario.Any(e => e.VoluntarioId == id);
        }
    }
}
