using la_paceña_v2.Datos;
using la_paceña_v2.Services;
using la_paceña_v2.Services.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace la_paceña_v2.Controllers
{
    public class ReporteStockController : Controller
    {
        private readonly IReporteService _reporteService;

        public ReporteStockController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Index(string? tipo, string? estado)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var reporte = await _reporteService.ObtenerReporteStock(tipo, estado);
            ViewBag.Tipo = tipo;
            ViewBag.Estado = estado;
            return View(reporte);
        }

        [HttpPost]
        public async Task<IActionResult> ExportarExcel(string? tipo, string? estado)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var reporte = await _reporteService.ObtenerReporteStock(tipo, estado);
            var content = ExportHelper.ExportarStockExcel(reporte);
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Stock_{DateTime.Now:yyyyMMddHHmm}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> ExportarPDF(string? tipo, string? estado)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var reporte = await _reporteService.ObtenerReporteStock(tipo, estado);
            var content = ExportHelper.ExportarStockPDF(reporte);
            return File(content, "application/pdf", $"Stock_{DateTime.Now:yyyyMMddHHmm}.pdf");
        }

        public async Task<IActionResult> Detalle(string tipo, string nombre)
        {
            if (!VerificarAuth()) return PartialView("_ErrorAuth");
            var detalle = await _reporteService.ObtenerDetalleStock(tipo, nombre);
            if (detalle == null) return NotFound();
            return PartialView("_DetalleStock", detalle);
        }
    }
}
