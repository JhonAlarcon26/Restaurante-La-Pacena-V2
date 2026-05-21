using la_paceña_v2.Datos;
using la_paceña_v2.Models.ViewModels;
using la_paceña_v2.Services;
using la_paceña_v2.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class ReporteCompraProductoController : Controller
    {
        private readonly IReporteService _reporteService;
        private readonly ContextoRestaurante _contexto;

        public ReporteCompraProductoController(IReporteService reporteService, ContextoRestaurante contexto)
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

            var reporte = await _reporteService.ObtenerReporteCompraProductos(inicio, finConsulta, idProveedor);

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
            var reporte = await _reporteService.ObtenerReporteCompraProductos(fechaInicio, fin, idProveedor);
            var content = ExportHelper.ExportarComprasExcel(reporte, "REPORTE COMPRA PRODUCTO");
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ComprasProductos_{DateTime.Now:yyyyMMddHHmm}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> ExportarPDF(DateTime fechaInicio, DateTime fechaFin, int? idProveedor)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var fin = fechaFin.Date.AddDays(1).AddSeconds(-1);
            var reporte = await _reporteService.ObtenerReporteCompraProductos(fechaInicio, fin, idProveedor);
            var content = ExportHelper.ExportarComprasPDF(reporte, "Reporte Compra Producto");
            return File(content, "application/pdf", $"ComprasProductos_{DateTime.Now:yyyyMMddHHmm}.pdf");
        }

        public async Task<IActionResult> Detalle(int id)
        {
            if (!VerificarAuth()) return Unauthorized();

            var CompraProducto = await _contexto.Abastecimientos
                .Include(c => c.IdProveedorNavigation)
                .Include(c => c.IdUsuarioNavigation)
                .Include(c => c.DetalleAbastecimientos)
                    .ThenInclude(d => d.IdProductoNavigation)
                .FirstOrDefaultAsync(c => c.IdAbastecimiento == id);

            if (CompraProducto == null) return NotFound();

            return PartialView("_DetalleCompraProducto", CompraProducto);
        }
    }
}
