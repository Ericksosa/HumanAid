using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HumanAid.Data;

namespace HumanAid.Areas.Socio.Controllers
{
    [Area("Socio")]
    public class PagosController : Controller
    {
        private readonly HumanAidDbContext _context;

        public PagosController(HumanAidDbContext context)
        {
            _context = context;
        }

        // GET: Socio/Pagos
        public async Task<IActionResult> Index()
        {
            var humanAidDbContext = _context.Pago.Include(p => p.Socio).Include(p => p.TipoCuota);
            return View(await humanAidDbContext.ToListAsync());
        }

        // GET: Socio/Pagos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pago
                .Include(p => p.Socio)
                .Include(p => p.TipoCuota)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // GET: Socio/Pagos/Create
        public IActionResult Create()
        {
            var socios = _context.Socio.Include(s => s.TipoCuota).ToList();
            var socioSelectList = socios.Select(s => new SelectListItem
            {
                Value = s.SocioId.ToString(),
                Text = s.Nombre 
            }).ToList();

            ViewData["SocioId"] = socioSelectList;

            var pago = new Pago();

            if (socios.Any())
            {
                var socioInicial = socios.First();
                pago.SocioId = socioInicial.SocioId;
                pago.TipoCuotaId = socioInicial.TipoCuota.TipoCuotaId;
                pago.MontoCuota = socioInicial.TipoCuota.Importe;
                ViewData["TipoCuotaTexto"] = $"Cuota: {socioInicial.TipoCuota.Nombre}, Importe: {socioInicial.TipoCuota.Importe}";
            }

            return View(pago);
        }

        // POST: Socio/Pagos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SocioId,CantidadCuotas,FechaPago")] Pago pago)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var socio = _context.Socio.Include(s => s.TipoCuota).FirstOrDefault(s => s.SocioId == pago.SocioId);
                if (socio == null)
                {
                    ModelState.AddModelError("", "El socio seleccionado no existe.");
                    throw new Exception("El socio seleccionado no existe en la base de datos.");
                }

                pago.TipoCuotaId = socio.TipoCuota.TipoCuotaId;
                pago.MontoCuota = socio.TipoCuota.Importe;
                _context.Add(pago);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error al guardar el pago: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar el pago.");

                var socios = _context.Socio.Include(s => s.TipoCuota).ToList();
                ViewData["SocioId"] = socios.Select(s => new SelectListItem
                {
                    Value = s.SocioId.ToString(),
                    Text = s.Nombre
                }).ToList();

                return View(pago);
            }
        }

        // GET: Socio/Pagos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pago
                .Include(p => p.Socio)
                .Include(p => p.TipoCuota)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pago == null)
            {
                return NotFound();
            }

            ViewBag.SocioId = new SelectList(_context.Socio, "SocioId", "Nombre", pago.SocioId);
            ViewBag.TipoCuotaId = new SelectList(_context.TipoCuota, "TipoCuotaId", "Nombre", pago.TipoCuotaId);
            ViewData["TipoCuotaTexto"] = $"Cuota: {pago.TipoCuota.Nombre}, Importe: {pago.MontoCuota}";

            return View(pago);
        }

        // POST: Socio/Pagos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SocioId,TipoCuotaId,MontoCuota,CantidadCuotas,FechaPago")] Pago pago)
        {
            if (id != pago.Id)
            {
                return NotFound();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var pagoExistente = await _context.Pago
                    .Include(p => p.TipoCuota)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pagoExistente == null)
                {
                    return NotFound();
                }

                pagoExistente.SocioId = pago.SocioId;
                pagoExistente.TipoCuotaId = pago.TipoCuotaId;
                pagoExistente.MontoCuota = pago.MontoCuota;
                pagoExistente.CantidadCuotas = pago.CantidadCuotas;
                pagoExistente.FechaPago = pago.FechaPago;

                _context.Update(pagoExistente);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al guardar los cambios.");
            }

            ViewBag.SocioId = new SelectList(_context.Socio, "SocioId", "Nombre", pago.SocioId);
            ViewBag.TipoCuotaId = new SelectList(_context.TipoCuota, "TipoCuotaId", "Nombre", pago.TipoCuotaId);

            return View(pago);
        }

        // GET: Socio/Pagos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pago
                .Include(p => p.Socio)
                .Include(p => p.TipoCuota)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // POST: Socio/Pagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pago = await _context.Pago.FindAsync(id);
            if (pago != null)
            {
                _context.Pago.Remove(pago);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public JsonResult ObtenerDatosCuota(int socioId)
        {
            var socio = _context.Socio.Include(s => s.TipoCuota).FirstOrDefault(s => s.SocioId == socioId);

            if (socio == null || socio.TipoCuota == null)
            {
                return Json(new { success = false, message = "Socio o tipo de cuota no encontrado." });
            }

            return Json(new
            {
                success = true,
                tipoCuota = $"Cuota: {socio.TipoCuota.Nombre}, Importe: {socio.TipoCuota.Importe}",
                tipoCuotaId = socio.TipoCuota.TipoCuotaId, 
                montoCuota = socio.TipoCuota.Importe
            });
        }

        private bool PagoExists(int id)
        {
            return _context.Pago.Any(e => e.Id == id);
        }
    }
}