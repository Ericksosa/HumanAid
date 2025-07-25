﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HumanAid.Data;
using HumanAid.Models;
using Microsoft.AspNetCore.Authorization;

namespace HumanAid.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Area("Administrador")]
    public class VoluntarioSanitariosController : Controller
    {
        private readonly HumanAidDbContext _context;

        public VoluntarioSanitariosController(HumanAidDbContext context)
        {
            _context = context;
        }

        // GET: VoluntarioSanitarios
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            int pageSize = 7;

            var voluntariosSanitarios = from v in _context.VoluntarioSanitario.Include(v => v.Voluntario)
                                        select v;

            if (!string.IsNullOrEmpty(searchString))
            {
                voluntariosSanitarios = voluntariosSanitarios.Where(s => s.Voluntario.Nombre.Contains(searchString));
            }

            int totalItems = await voluntariosSanitarios.CountAsync();
            var items = await voluntariosSanitarios
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewData["SearchString"] = searchString;

            return View(items);
        }


        // GET: VoluntarioSanitarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voluntarioSanitario = await _context.VoluntarioSanitario
                .Include(v => v.Voluntario)
                .FirstOrDefaultAsync(m => m.VoluntarioSanitarioId == id);
            if (voluntarioSanitario == null)
            {
                return NotFound();
            }

            return View(voluntarioSanitario);
        }

        // GET: VoluntarioSanitarios/Create
        public IActionResult Create()
        {
            var voluntariosNoAdministrativosNiSanitarios = _context.Voluntario
                .Where(v => v.VoluntarioAdministrativo == null && v.VoluntarioSanitario == null)
                .Select(v => new { v.VoluntarioId, v.Nombre })
                .ToList();

            ViewData["VoluntarioId"] = new SelectList(voluntariosNoAdministrativosNiSanitarios, "VoluntarioId", "Nombre");
            return View();
        }


        // POST: VoluntarioSanitarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Profesion,Disponibilidad,NumeroTrabajosRealizados,VoluntarioId")] VoluntarioSanitario voluntarioSanitario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Add(voluntarioSanitario);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["success"] = "Voluntario Sanitario creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error: {ex.Message} - {ex.InnerException?.Message}";
            }

            ViewData["VoluntarioId"] = new SelectList(_context.Voluntario, "VoluntarioId", "Nombre", voluntarioSanitario.VoluntarioId);
            return View(voluntarioSanitario);
        }



        // GET: VoluntarioSanitarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voluntarioSanitario = await _context.VoluntarioSanitario.FindAsync(id);
            if (voluntarioSanitario == null)
            {
                return NotFound();
            }

            var voluntariosNoAdministrativosNiSanitarios = _context.Voluntario
                .Where(v => v.VoluntarioAdministrativo == null && v.VoluntarioSanitario == null)
                .Select(v => new { v.VoluntarioId, v.Nombre })
                .ToList();

            ViewData["VoluntarioId"] = new SelectList(voluntariosNoAdministrativosNiSanitarios, "VoluntarioId", "Nombre", voluntarioSanitario.VoluntarioId);
            return View(voluntarioSanitario);
        }

        // POST: VoluntarioSanitarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VoluntarioSanitarioId,Profesion,Disponibilidad,NumeroTrabajosRealizados,VoluntarioId")] VoluntarioSanitario voluntarioSanitario)
        {
            if (id != voluntarioSanitario.VoluntarioSanitarioId)
            {
                return NotFound();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Update(voluntarioSanitario);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["success"] = "Voluntario Sanitario editado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                if (!VoluntarioSanitarioExists(voluntarioSanitario.VoluntarioSanitarioId))
                {
                    TempData["danger"] = "Voluntario Sanitario no encontrado.";
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

            ViewData["VoluntarioId"] = new SelectList(_context.Voluntario, "VoluntarioId", "Nombre", voluntarioSanitario.VoluntarioId);
            return View(voluntarioSanitario);
        }





        // GET: VoluntarioSanitarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voluntarioSanitario = await _context.VoluntarioSanitario
                .Include(v => v.Voluntario)
                .FirstOrDefaultAsync(m => m.VoluntarioSanitarioId == id);
            if (voluntarioSanitario == null)
            {
                return NotFound();
            }

            return View(voluntarioSanitario);
        }

        // POST: VoluntarioSanitarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var voluntarioSanitario = await _context.VoluntarioSanitario.FindAsync(id);
                if (voluntarioSanitario != null)
                {
                    _context.VoluntarioSanitario.Remove(voluntarioSanitario);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["success"] = "Voluntario Sanitario eliminado exitosamente.";
                }
                else
                {
                    TempData["danger"] = "Voluntario Sanitario no encontrado.";
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error: {ex.Message} - {ex.InnerException?.Message}";
            }

            return RedirectToAction(nameof(Index));
        }



        private bool VoluntarioSanitarioExists(int id)
        {
            return _context.VoluntarioSanitario.Any(e => e.VoluntarioSanitarioId == id);
        }
    }
}
