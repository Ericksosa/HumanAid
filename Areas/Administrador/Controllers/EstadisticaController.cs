using HumanAid.Services;
using Microsoft.AspNetCore.Mvc;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using HumanAid.Data;
using ClosedXML.Excel;
using HumanAid.Models;

namespace HumanAid.Areas.Administrador.Controllers
{
    [Route("Estadistica")]
    public class EstadisticaController : Controller
    {
        private readonly EstadisticasService _estadisticasService;
        private readonly HumanAidDbContext _context;

        public EstadisticaController(EstadisticasService estadisticasService, HumanAidDbContext context)
        {
            _estadisticasService = estadisticasService; // Inyectado correctamente
            _context = context;
        }

        [HttpGet("ObtenerEstadisticas")]
        public JsonResult ObtenerEstadisticas() // Metodo agregado para hacer graifcos en vivo
        {
            var datos = _context.Socio
                .GroupBy(s => new { SedeNombre = s.Sede.Nombre, TipoCuotaNombre = s.TipoCuota.Nombre, s.TipoCuota.Importe })
                .Select(g => new {
                    Sede = g.Key.SedeNombre,
                    TipoCuota = g.Key.TipoCuotaNombre,
                    TotalSocios = g.Count(),
                    ImporteCuota = g.Key.Importe
                })
                .ToList();

            return Json(datos);
        }

        public byte[] GenerarReporteExcel(List<EstadisticaDto> estadisticas)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Estadísticas");

                // Ajustar ancho de columnas
                worksheet.Column(1).Width = 25; // Sede
                worksheet.Column(2).Width = 35; // Tipo Cuota
                worksheet.Column(3).Width = 35; // Total Socios

                int fila = 1;
                string sedeActual = "";

                // Agregar encabezados de la tabla
                worksheet.Cell(fila, 1).Value = "Sede";
                worksheet.Cell(fila, 2).Value = "Tipo Cuota";
                worksheet.Cell(fila, 3).Value = "Total Socios";

                worksheet.Range(fila, 1, fila, 3).Style.Font.Bold = true;
                worksheet.Range(fila, 1, fila, 3).Style.Fill.BackgroundColor = XLColor.Blue;
                worksheet.Range(fila, 1, fila, 3).Style.Font.FontColor = XLColor.White;
                worksheet.Range(fila, 1, fila, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(fila, 1, fila, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                fila++; // Avanzamos a la siguiente fila

                foreach (var item in estadisticas.OrderBy(e => e.Sede).ThenBy(e => e.TipoCuota))
                {
                    // Si la sede cambia, agregamos una fila con su nombre en fondo gris
                    if (sedeActual != item.Sede)
                    {
                        sedeActual = item.Sede;

                        worksheet.Cell(fila, 1).Value = sedeActual;
                        worksheet.Cell(fila, 1).Style.Font.Bold = true;
                        worksheet.Cell(fila, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
                        worksheet.Cell(fila, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(fila, 1).Style.Font.FontColor = XLColor.Black;

                        worksheet.Range(fila, 1, fila, 3).Merge(); // Unir celdas para claridad
                        worksheet.Range(fila, 1, fila, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        fila++; // Avanzamos a la siguiente fila
                    }

                    // Rellenar la columna A con la sede actual
                    worksheet.Cell(fila, 1).Value = sedeActual;
                    worksheet.Cell(fila, 2).Value = item.TipoCuota;
                    worksheet.Cell(fila, 3).Value = item.TotalSocios;

                    worksheet.Range(fila, 1, fila, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Row(fila).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    fila++; // Avanzamos a la siguiente fila de datos
                }

                using (var ms = new MemoryStream())
                {
                    workbook.SaveAs(ms);
                    return ms.ToArray();
                }
            }
        }

        [HttpGet("ExportarExcel")]
        public IActionResult ExportarExcel()
        {
            var estadisticas = _estadisticasService.ObtenerEstadisticas()
                                       .OrderBy(e => e.Sede)
                                       .ThenBy(e => e.TipoCuota) // Opcional: ordena por tipo de cuota dentro de cada sede
                                       .ToList();
            var contenidoExcel = GenerarReporteExcel(estadisticas);
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

                var estadisticas = _estadisticasService.ObtenerEstadisticas()
                                                       .OrderBy(e => e.Sede)
                                                       .ThenBy(e => e.TipoCuota)
                                                       .ToList();

                string sedeActual = "";

                foreach (var item in estadisticas)
                {
                    // Si la sede cambia, agregamos un título nuevo
                    if (sedeActual != item.Sede)
                    {
                        sedeActual = item.Sede;
                        doc.Add(new Paragraph($" Sede: {sedeActual}")
                        {
                            Alignment = Element.ALIGN_CENTER,
                            Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK)
                        });
                        doc.Add(new Paragraph(" ")); // Espacio para separar
                    }

                    // Crear tabla con formato
                    PdfPTable table = new PdfPTable(2);
                    table.WidthPercentage = 100;

                    // Encabezado con fondo de color
                    PdfPCell cellTipoCuota = new PdfPCell(new Phrase("Tipo Cuota", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE)));
                    cellTipoCuota.BackgroundColor = new BaseColor(0, 102, 204); // Color Azul
                    cellTipoCuota.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cellTipoCuota);

                    PdfPCell cellTotalSocios = new PdfPCell(new Phrase("Total Socios", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE)));
                    cellTotalSocios.BackgroundColor = new BaseColor(0, 102, 204); // Color Azul
                    cellTotalSocios.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cellTotalSocios);

                    // Celdas con datos, alineación centrada
                    PdfPCell tipoCuotaCell = new PdfPCell(new Phrase(item.TipoCuota));
                    tipoCuotaCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(tipoCuotaCell);

                    PdfPCell totalSociosCell = new PdfPCell(new Phrase(item.TotalSocios.ToString()));
                    totalSociosCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(totalSociosCell);

                    doc.Add(table);
                }

                doc.Close();
                return File(ms.ToArray(), "application/pdf", "Estadisticas.pdf");
            }
        }

        public IActionResult Index()
        {
            var datos = _estadisticasService.ObtenerEstadisticas();
            return View(datos);
        }
    }
}
