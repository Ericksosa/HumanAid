using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HumanAid.Data;
using HumanAid.Models;
using NuGet.Packaging;
using X.PagedList.Mvc;
using X.PagedList.EF;
using Microsoft.AspNetCore.Authorization;

namespace HumanAid.Areas.Administrador.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Area("Administrador")]
    public class EnvioController : Controller
    {
        private readonly HumanAidDbContext _context;

        public EnvioController(HumanAidDbContext context)
        {
            _context = context;
        }


        // GET: Envio
        public async Task<IActionResult> Index(int? sedeId, int? page)
        {
            int pageSize = 7; // Tamaño de página
            int pageNumber = page ?? 1; // Número de página

            // Cargar los envíos e incluir la relación con Sede
            var envios = _context.Envio
                .Include(e => e.EnvioSedes)
                .ThenInclude(es => es.Sede)
                .AsQueryable();

            // Filtrar por sede si se ha seleccionado una
            if (sedeId.HasValue)
            {
                envios = envios.Where(e => e.EnvioSedes.Any(es => es.SedeId == sedeId.Value));
            }

            // Obtener lista de sedes disponibles para el filtro
            ViewBag.Sedes = _context.Sede
                .Select(s => new { s.SedeId, s.Nombre })
                .ToList();

            // Establecer el filtro actual para la paginación
            ViewBag.CurrentSedeFilter = sedeId;

            // Aplicar paginación a la consulta
            var enviosPaged = await envios.ToPagedListAsync(pageNumber, pageSize);

            return View(enviosPaged);
        }


        // GET: Envio/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var envio = await _context.Envio
                .Include(e => e.EnvioSedes) // Incluir las relaciones de EnvioSedes
                .ThenInclude(es => es.Sede) // Incluir las sedes organizadoras
                .FirstOrDefaultAsync(m => m.EnvioId == id);
            if (envio == null)
            {
                return NotFound();
            }
            ViewBag.Sedes = envio.EnvioSedes.Select(es => es.Sede).ToList(); // Pasar solo las sedes del envío
            return View(envio);
        }

        // GET: Envio/Create
        public IActionResult Create()
        {
            RecargarSede(); // Llamar a la funcion recargar sede
            var envio = new Envio
            {
                Estado = "Pendiente" // Valor inicial
            };
            return View();
        }



        // POST: Envio/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnvioId,Fecha,Destino,TipoEnvio,Estado,CodigoEnvio,FechaSalida")] Envio envio, int[] sedeIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validar si el CódigoEnvio ya existe
                var codigoExistente = await _context.Envio.AnyAsync(e => e.CodigoEnvio == envio.CodigoEnvio);
                if (codigoExistente)
                {
                    throw new Exception("El código de envío es existente. Ingrese un nuevo código.");
                }

                // Guardar el envío
                _context.Add(envio);
                await _context.SaveChangesAsync();

                // Relaciones con las sedes organizadoras 
                if (sedeIds != null && sedeIds.Any())
                {
                    var envioSedes = sedeIds.Select(sedeId => new EnvioSede
                    {
                        EnvioId = envio.EnvioId,
                        SedeId = sedeId
                    }).ToList();

                    _context.EnvioSede.AddRange(envioSedes);
                    await _context.SaveChangesAsync();
                }

                // Confirmar la transacción
                await transaction.CommitAsync();
                TempData["success"] = "Envio creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // Mostrar mensaje detallado en la consola
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                // Agregar el error al ModelState para mostrarlo en la vista
                ModelState.AddModelError("", ex.Message);

                RecargarSede(); // Llamar a la funcion recargar sede
                await transaction.RollbackAsync();
                TempData["danger"] = "Ocurrió un error al crear el envio.";
                return View(envio);
            }
        }


        // GET: Envio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var envio = await _context.Envio
            .Include(e => e.EnvioSedes) // Incluir la relación con las sedes
            .ThenInclude(es => es.Sede)
            .FirstOrDefaultAsync(e => e.EnvioId == id);

            if (envio == null)
            {
                return NotFound();
            }
            RecargarSede(); // Llamar a la funcion recargar sede

            // Obtener la sede organizadora actual del envío
            int? sedeActualId = envio.EnvioSedes?.FirstOrDefault()?.SedeId;

            // Filtrar sedes excluyendo la sede organizadora actual
            ViewBag.Sedes = await _context.Sede
                .Where(s => s.SedeId != sedeActualId) // Excluir la sede actual
                .Select(s => new { s.SedeId, s.Nombre })
                .ToListAsync();

            return View(envio);
        }

        // POST: Envio/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EnvioId,Fecha,Destino,TipoEnvio,Estado,CodigoEnvio,FechaSalida")] Envio envio, int[] sedeIds)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            if (id != envio.EnvioId)
            {
                return NotFound();
            }
            try
            {
                var envioToUpdate = await _context.Envio
                    .Include(e => e.EnvioSedes) // Incluir la relación
                     .ThenInclude(es => es.Sede)
                    .FirstOrDefaultAsync(e => e.EnvioId == id);
                if (envioToUpdate == null)
                {
                    return NotFound();
                }

                // Validar si el Código de Envío ya existe en otro registro
                var envioExistente = await _context.Envio
                    .FirstOrDefaultAsync(e => e.CodigoEnvio == envio.CodigoEnvio && e.EnvioId != id);

                if (envioExistente != null)
                {
                    throw new Exception("El código de envío ya existe. Por favor, ingrese un código único.");
                }

                // **Actualizar los datos del Envío**
                envioToUpdate.Fecha = envio.Fecha;
                envioToUpdate.Destino = envio.Destino;
                envioToUpdate.TipoEnvio = envio.TipoEnvio;
                envioToUpdate.Estado = envio.Estado;
                envioToUpdate.CodigoEnvio = envio.CodigoEnvio;
                envioToUpdate.FechaSalida = envio.FechaSalida;

                // **Actualizar la relación con las Sedes**
                envioToUpdate.EnvioSedes.Clear(); // Eliminar las relaciones anteriores

                if (sedeIds != null && sedeIds.Length > 0)
                {
                    var nuevasSedes = sedeIds.Select(sedeId => new EnvioSede
                    {
                        EnvioId = envio.EnvioId,
                        SedeId = sedeId
                    }).ToList();

                    envioToUpdate.EnvioSedes.AddRange(nuevasSedes);
                }

                _context.Update(envioToUpdate);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["success"] = "Envio editado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Envio.Any(e => e.EnvioId == envio.EnvioId))
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
                ViewBag.ErrorMessage = ex.Message;
                TempData["danger"] = "Ocurrió un error al editar el envio.";
                RecargarSede(); // Recargar la lista de sedes
                return View(envio);
            }
        }

        private bool EnvioExists(int envioId)
        {
            throw new NotImplementedException();
        }

        // GET: Envio/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            if (id == null)
            {
                return NotFound();
            }
            var envio = await _context.Envio
                .Include(e => e.EnvioSedes) 
                .ThenInclude(es => es.Sede)
                .FirstOrDefaultAsync(m => m.EnvioId == id);

            if (envio == null)
            {
                return NotFound();
            }
            RecargarSede(); // Llamar a la funcion recargar sede
            // Obtener la sede organizadora actual del envío
             int? sedeActualId = envio.EnvioSedes?.FirstOrDefault()?.SedeId;

       
            ViewBag.Sedes = await _context.Sede
                .Where(s => s.SedeId == sedeActualId) 
                .Select(s => new { s.SedeId, s.Nombre })
                .ToListAsync();

            return View(envio);
        }

        // POST: Envio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var envio = await _context.Envio
                    .Include(e => e.EnvioSedes)
                    .Include(e => e.Alimentos)
                    .Include(e => e.Medicamentos)
                    .Include(e => e.MisionesHumanitarias)
                    .FirstOrDefaultAsync(m => m.EnvioId == id);

                if (envio == null)
                {
                    TempData["ErrorMessage"] = "El envío no existe o ya fue eliminado.";
                    return RedirectToAction(nameof(Index));
                }

                // Eliminar registros relacionados antes de eliminar el Envio
                _context.EnvioSede.RemoveRange(envio.EnvioSedes);
                _context.Alimento.RemoveRange(envio.Alimentos);
                _context.Medicamento.RemoveRange(envio.Medicamentos);
                _context.MisionHumanitaria.RemoveRange(envio.MisionesHumanitarias);
                _context.Envio.Remove(envio);            
                await transaction.CommitAsync();
                await _context.SaveChangesAsync();
                TempData["success"] = "Envío eliminado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["danger"] = $"Ocurrió un error al intentar eliminar el envío: {ex.Message}";
                return RedirectToAction("Delete", new { id });
            }

        }

        private void RecargarSede()  // Recargar la Sede
        {
            ViewBag.Sedes = _context.Sede
                .Select(s => new { s.SedeId, s.Nombre })
                .ToList();
        }

    }
}
