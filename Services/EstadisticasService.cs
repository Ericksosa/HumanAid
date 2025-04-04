using Microsoft.Extensions.Caching.Memory;
using HumanAid.Models;
using HumanAid.Data;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.IO;



namespace HumanAid.Services
{
    public class EstadisticasService
    {
        private readonly HumanAidDbContext _context;
        private readonly IMemoryCache _cache;

        public EstadisticasService(HumanAidDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public List<EstadisticaDto> ObtenerEstadisticas(int? sedeId = null, int? tipoCuotaId = null, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            string cacheKey = $"estadisticas_{sedeId}_{tipoCuotaId}_{fechaInicio}_{fechaFin}";

            if (!_cache.TryGetValue(cacheKey, out List<EstadisticaDto> estadisticas))
            {
                var query = _context.Socio
                    .AsNoTracking()
                    .Include(s => s.Sede)
                    .Include(s => s.TipoCuota)
                    .Where(s =>
                        (!sedeId.HasValue || s.SedeId == sedeId.Value) &&
                        (!tipoCuotaId.HasValue || s.TipoCuotaId == tipoCuotaId.Value) &&
                        (!fechaInicio.HasValue || s.FechaRegistro >= fechaInicio.Value) &&
                        (!fechaFin.HasValue || s.FechaRegistro <= fechaFin.Value));

                estadisticas = query
                    .GroupBy(s => new { s.SedeId, s.TipoCuotaId })
                    .Select(g => new EstadisticaDto
                    {
                        Sede = g.First().Sede.Nombre,
                        TipoCuota = g.First().TipoCuota.Nombre,
                        TotalSocios = g.Count()
                    })
                    .ToList();

                _cache.Set(cacheKey, estadisticas, TimeSpan.FromMinutes(10));
            }

            return estadisticas;
        }

        public byte[] GenerarReporteExcel()
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Estadísticas");

            worksheet.Cell(1, 1).Value = "Sede";
            worksheet.Cell(1, 2).Value = "Tipo Cuota";
            worksheet.Cell(1, 3).Value = "Total Socios";

            var estadisticas = _context.Socio.AsNoTracking()
                .GroupBy(s => new { s.SedeId, s.TipoCuotaId })
                .Select(g => new EstadisticaDto
                {
                    Sede = g.First().Sede.Nombre,
                    TipoCuota = g.First().TipoCuota.Nombre,
                    TotalSocios = g.Count()
                }).ToList();

            int row = 2;
            foreach (var item in estadisticas)
            {
                worksheet.Cell(row, 1).Value = item.Sede;
                worksheet.Cell(row, 2).Value = item.TipoCuota;
                worksheet.Cell(row, 3).Value = item.TotalSocios;
                row++;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }
}