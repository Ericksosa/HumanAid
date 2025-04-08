using ClosedXML.Excel;
using HumanAid.Data;
using HumanAid.Models;
using HumanAid.Services;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using NuGet.Protocol.Plugins;

namespace HumanAid.Areas.Administrador.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Area("Administrador")]
    public class AyudaHumanitariaController : Controller
    {
        private readonly HumanAidDbContext _context;

        public AyudaHumanitariaController(HumanAidDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string tipoDonacion, string destino, string fechaInicio, string fechaFin)
        {
            // Pasar los filtros a la vista
            ViewData["TipoDonacion"] = tipoDonacion;
            ViewData["Destino"] = destino;
            ViewData["FechaInicio"] = fechaInicio;
            ViewData["FechaFin"] = fechaFin;

            // Obtener las fechas y destinos únicos para los filtros
            var envios = await _context.Envio.ToListAsync();
            var fechas = envios.Select(e => e.Fecha).Distinct().OrderBy(f => f).ToList();
            var destinos = envios.Select(e => e.Destino).Distinct().ToList();

            ViewBag.Fechas = fechas;
            ViewBag.Destinos = destinos;


            if (!string.IsNullOrEmpty(tipoDonacion) ||
                   !string.IsNullOrEmpty(destino) ||
                   !string.IsNullOrEmpty(fechaInicio) ||
                   !string.IsNullOrEmpty(fechaFin))
            {
                await GetFilteredEnvios(tipoDonacion, destino, fechaInicio, fechaFin);
            }

            return View();
        }

        private async Task<List<Envio>> GetFilteredEnvios(string tipoDonacion, string destino, string fechaInicio, string fechaFin)
        {
            var envios = _context.Envio.AsQueryable();

            try
            {
                if (!string.IsNullOrEmpty(tipoDonacion))
                    envios = envios.Where(e => e.TipoEnvio == tipoDonacion);

                if (!string.IsNullOrEmpty(destino))
                    envios = envios.Where(e => e.Destino == destino);

                if (!string.IsNullOrEmpty(fechaInicio) && DateTime.TryParse(fechaInicio, out DateTime fechaInicioParsed))
                    envios = envios.Where(e => e.Fecha >= fechaInicioParsed);

                if (!string.IsNullOrEmpty(fechaFin) && DateTime.TryParse(fechaFin, out DateTime fechaFinParsed))
                    envios = envios.Where(e => e.Fecha <= fechaFinParsed);

                var lista = await envios.ToListAsync();

                if (lista == null || !lista.Any())
                {
                    TempData["danger"] = "No se encontraron datos en ese periodo.";
                }
                else
                {
                    TempData["success"] = "Datos filtrados correctamente.";
                }

                return lista;
            }
            catch (Exception)
            {
                TempData["danger"] = "Ocurrió un error al filtrar los datos.";
                return new List<Envio>();
            }
        }

        public async Task<IActionResult> ExportToPdf(string tipoDonacion, string destino, string fechaInicio, string fechaFin)
        {
            var envios = await GetFilteredEnvios(tipoDonacion, destino, fechaInicio, fechaFin);

            if (envios == null || !envios.Any())
            {
                TempData["danger"] = "No se encontraron datos en ese periodo para generar el PDF.";
                return RedirectToAction("Index", new { tipoDonacion, destino, fechaInicio, fechaFin });
            }

            using (var stream = new MemoryStream())
            {
                var document = new iTextSharp.text.Document(PageSize.A4);
                PdfWriter.GetInstance(document, stream);
                document.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                document.Add(new iTextSharp.text.Paragraph("Reporte de Ayuda Humanitaria", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER
                });

                document.Add(new iTextSharp.text.Paragraph(" ")); // Espacio

                PdfPTable table = new PdfPTable(4)
                {
                    WidthPercentage = 100
                };
                table.SetWidths(new float[] { 2, 2, 2, 2 }); // Column widths

                // Encabezados
                BaseColor headerBgColor = new BaseColor(0, 102, 204); // Azul
                AddCell(table, "Tipo de Ayuda", headerFont, headerBgColor, Element.ALIGN_CENTER);
                AddCell(table, "Destino", headerFont, headerBgColor, Element.ALIGN_CENTER);
                AddCell(table, "Fecha", headerFont, headerBgColor, Element.ALIGN_CENTER);
                AddCell(table, "Fecha de Salida", headerFont, headerBgColor, Element.ALIGN_CENTER);

                foreach (var envio in envios)
                {
                    AddCell(table, envio.TipoEnvio, cellFont);
                    AddCell(table, envio.Destino, cellFont);
                    AddCell(table, envio.Fecha.ToString("dd/MM/yyyy"), cellFont);
                    AddCell(table, envio.FechaSalida.ToString("dd/MM/yyyy"), cellFont);
                }

                document.Add(table);
                document.Close();

                return File(stream.ToArray(), "application/pdf", "Envios.pdf");
            }
        }

        public async Task<IActionResult> ExportToExcel(string tipoDonacion, string destino, string fechaInicio, string fechaFin)
        {
            var envios = await GetFilteredEnvios(tipoDonacion, destino, fechaInicio, fechaFin);

            if (envios == null || !envios.Any())
            {
                TempData["danger"] = "No se encontraron datos en ese periodo para generar el Excel.";
                return RedirectToAction("Index", new { tipoDonacion, destino, fechaInicio, fechaFin });
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Envios");
                var currentRow = 1;

                // Encabezados con formato
                worksheet.Cell(currentRow, 1).Value = "Tipo de Ayuda";
                worksheet.Cell(currentRow, 2).Value = "Destino";
                worksheet.Cell(currentRow, 3).Value = "Fecha";
                worksheet.Cell(currentRow, 4).Value = "Fecha de Salida";

                var headerRange = worksheet.Range(currentRow, 1, currentRow, 4);
                headerRange.Style.Fill.BackgroundColor = XLColor.CornflowerBlue;
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                foreach (var envio in envios)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = envio.TipoEnvio;
                    worksheet.Cell(currentRow, 2).Value = envio.Destino;
                    worksheet.Cell(currentRow, 3).Value = envio.Fecha.ToString("dd/MM/yyyy");
                    worksheet.Cell(currentRow, 4).Value = envio.FechaSalida.ToString("dd/MM/yyyy");
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Envios.xlsx");
                }
            }
        }

        private void AddCell(PdfPTable table, string text, Font font, BaseColor bgColor = null, int align = Element.ALIGN_LEFT)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font))
            {
                HorizontalAlignment = align,
                Padding = 5
            };

            if (bgColor != null)
            {
                cell.BackgroundColor = bgColor;
            }

            table.AddCell(cell);
        }
    }
}