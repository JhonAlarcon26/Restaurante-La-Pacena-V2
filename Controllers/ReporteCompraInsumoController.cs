using la_paceña_v2.Datos;
using la_paceña_v2.Models.ViewModels;
using la_paceña_v2.Services;
using la_paceña_v2.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class ReporteCompraInsumoController : Controller
    {
        private readonly IReporteService _reporteService;
        private readonly ContextoRestaurante _contexto;

        public ReporteCompraInsumoController(IReporteService reporteService, ContextoRestaurante contexto)
        {
            _reporteService = reporteService;
            _contexto = contexto;
        }

        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin, int? idProveedor)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var hoy = DateTime.Today;
            var inicio = fechaInicio ?? new DateTime(hoy.Year, hoy.Month, 1);
            var fin = fechaFin ?? new DateTime(hoy.Year, hoy.Month, DateTime.DaysInMonth(hoy.Year, hoy.Month));
            var finConsulta = fin.Date.AddDays(1).AddSeconds(-1);

            var reporte = await _reporteService.ObtenerReporteCompraInsumos(inicio, finConsulta, idProveedor);

            ViewBag.Proveedores = new SelectList(await _contexto.Proveedors.ToListAsync(), "IdProveedor", "NombreEmpresa", idProveedor);

            var model = new FiltroReporteComprasViewModel
            {
                FechaInicio = inicio,
                FechaFin = fin,
                IdProveedor = idProveedor,
                Reporte = reporte
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ExportarExcel(DateTime fechaInicio, DateTime fechaFin, int? idProveedor)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var fin = fechaFin.Date.AddDays(1).AddSeconds(-1);
            var reporte = await _reporteService.ObtenerReporteCompraInsumos(fechaInicio, fin, idProveedor);
            var content = ExportHelper.ExportarComprasExcel(reporte, "REPORTE COMPRA INSUMO");
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ComprasInsumos_{DateTime.Now:yyyyMMddHHmm}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> ExportarPDF(DateTime fechaInicio, DateTime fechaFin, int? idProveedor)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var fin = fechaFin.Date.AddDays(1).AddSeconds(-1);
            var reporte = await _reporteService.ObtenerReporteCompraInsumos(fechaInicio, fin, idProveedor);
            var content = ExportHelper.ExportarComprasPDF(reporte, "Reporte Compra Insumo");
            return File(content, "application/pdf", $"ComprasInsumos_{DateTime.Now:yyyyMMddHHmm}.pdf");
        }

        public async Task<IActionResult> Detalle(int id)
        {
            if (!VerificarAuth()) return Unauthorized();

            var CompraInsumo = await _contexto.Compras
                .Include(c => c.IdProveedorNavigation)
                .Include(c => c.IdUsuarioNavigation)
                .Include(c => c.DetalleCompras)
                    .ThenInclude(d => d.IdInsumoNavigation)
                .FirstOrDefaultAsync(c => c.IdCompra == id);

            if (CompraInsumo == null) return NotFound();

            return PartialView("_DetalleCompraInsumo", CompraInsumo);
        }
    }
}
