using HumanAid.Services;
using Microsoft.AspNetCore.Mvc;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace HumanAid.Areas.Administrador.Controllers
{
    [Route("Estadisticas")]
    public class EstadisticaController : Controller
    {
        private readonly EstadisticasService _estadisticasService;

        [HttpGet("ExportarExcel")]
        public IActionResult ExportarExcel()
        {
            var contenidoExcel = _estadisticasService.GenerarReporteExcel();
            return File(contenidoExcel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Estadisticas.xlsx");
        }

        [HttpGet("ExportarPDF")]
        public IActionResult ExportarPDF()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document();
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                doc.Add(new Paragraph("Estadísticas de Socios"));
                doc.Add(new Paragraph(" "));

                PdfPTable table = new PdfPTable(3);
                table.AddCell("Sede");
                table.AddCell("Tipo Cuota");
                table.AddCell("Total Socios");

                var estadisticas = _estadisticasService.ObtenerEstadisticas();
                foreach (var item in estadisticas)
                {
                    table.AddCell(item.Sede);
                    table.AddCell(item.TipoCuota);
                    table.AddCell(item.TotalSocios.ToString());
                }

                doc.Add(table);
                doc.Close();

                return File(ms.ToArray(), "application/pdf", "Estadisticas.pdf");
            }
        }
        public EstadisticaController(EstadisticasService estadisticasService)
        {
            _estadisticasService = estadisticasService;
        }

        public IActionResult Index()
        {
            var datos = _estadisticasService.ObtenerEstadisticas();
            return View(datos);
        }
    }
}
