using HumanAid.Data;
using HumanAid.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Mvc;
using X.PagedList.EF;


namespace HumanAid.Controllers
{
    public class SeguimientoController : Controller
    {

        private readonly HumanAidDbContext _context;
        public SeguimientoController(HumanAidDbContext context) {
            _context = context;
       
        }
        public async Task<IActionResult> Index(string tipoDonacion, string destino, int? envioId, int? page)
        {
            int pageSize = 7; // Tamaño de página
            int pageNumber = page ?? 1; // Número de página

            var envios = _context.Envio
                .Include(e => e.EnvioSedes)
                .ThenInclude(es => es.Sede)
                .AsQueryable();

            // Aplicar filtros solo si los valores están presentes en la URL
            if (!string.IsNullOrEmpty(tipoDonacion))
            {
                envios = envios.Where(e => e.TipoEnvio == tipoDonacion);
            }

            if (!string.IsNullOrEmpty(destino))
            {
                envios = envios.Where(e => e.CodigoEnvio == destino);
            }

            if (envioId.HasValue)
            {
                envios = envios.Where(e => e.EnvioSedes.Any(es => es.EnvioId == envioId));
            }

            // Enviar lista de envíos para el formulario de selección
            ViewBag.Envios = await _context.Envio
                .Select(s => new { s.TipoEnvio, s.CodigoEnvio })
                .Distinct()
                .ToListAsync();

            // Guardar los filtros actuales para mantenerlos seleccionados en la vista
            ViewBag.CurrentTipoDonacion = tipoDonacion;
            ViewBag.CurrentDestino = destino;

            // Aplicar paginación
            var enviosPaged = await envios.ToPagedListAsync(pageNumber, pageSize);

            return View(enviosPaged);
        }

    }
}

