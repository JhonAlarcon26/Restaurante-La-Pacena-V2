using la_paceña_v2.Datos;
using la_paceña_v2.Models.ViewModels;
using la_paceña_v2.Services;
using la_paceña_v2.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class ReporteVentaController : Controller
    {
        private readonly IReporteService _reporteService;
        private readonly ContextoRestaurante _contexto;

        public ReporteVentaController(IReporteService reporteService, ContextoRestaurante contexto)
        {
            _reporteService = reporteService;
            _contexto = contexto;
        }

        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin, int? idSucursal, int? idUsuario)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var inicio = fechaInicio ?? DateTime.Today;
            var fin = fechaFin ?? DateTime.Today.AddDays(1).AddSeconds(-1);

            if (fechaInicio.HasValue && !fechaFin.HasValue)
                fin = inicio.AddDays(1).AddSeconds(-1);
            else if (fechaFin.HasValue)
                fin = fechaFin.Value.Date.AddDays(1).AddSeconds(-1);

            var reporte = await _reporteService.ObtenerReporteVentas(inicio, fin, idSucursal, idUsuario);

            ViewBag.Sucursales = new SelectList(await _contexto.Sucursals.ToListAsync(), "IdSucursal", "Nombre", idSucursal);
            ViewBag.Usuarios = new SelectList(await _contexto.Usuarios.ToListAsync(), "IdUsuario", "Nombre", idUsuario);

            var model = new FiltroReporteVentasViewModel
            {
                FechaInicio = inicio,
                FechaFin = fechaFin ?? DateTime.Today,
                IdSucursal = idSucursal,
                IdUsuario = idUsuario,
                Reporte = reporte
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ExportarExcel(DateTime fechaInicio, DateTime fechaFin, int? idSucursal, int? idUsuario)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var fin = fechaFin.Date.AddDays(1).AddSeconds(-1);
            var reporte = await _reporteService.ObtenerReporteVentas(fechaInicio, fin, idSucursal, idUsuario);
            var content = ExportHelper.ExportarVentasExcel(reporte);
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Ventas_{DateTime.Now:yyyyMMddHHmm}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> ExportarPDF(DateTime fechaInicio, DateTime fechaFin, int? idSucursal, int? idUsuario)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var fin = fechaFin.Date.AddDays(1).AddSeconds(-1);
            var reporte = await _reporteService.ObtenerReporteVentas(fechaInicio, fin, idSucursal, idUsuario);
            var content = ExportHelper.ExportarVentasPDF(reporte);
            return File(content, "application/pdf", $"Ventas_{DateTime.Now:yyyyMMddHHmm}.pdf");
        }
        public async Task<IActionResult> Detalle(int id)
        {
            if (!VerificarAuth()) return Unauthorized();

            var venta = await _contexto.Ventas
                .Include(v => v.IdSucursalNavigation)
                .Include(v => v.IdUsuarioNavigation)
                .Include(v => v.DetalleVentas)
                    .ThenInclude(d => d.IdProductoNavigation)
                .Include(v => v.DetalleVentas)
                    .ThenInclude(d => d.IdPlatoNavigation)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null) return NotFound();

            return PartialView("_DetalleVenta", venta);
        }
    }
}
