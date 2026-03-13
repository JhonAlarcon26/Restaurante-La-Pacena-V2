using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore; // Added for ToListAsync
using la_paceña_v2.Datos;
using la_paceña_v2.Services;
using la_paceña_v2.Models.ViewModels;
using la_paceña_v2.Services.Helpers;
using la_paceña_v2.Models; // For User/Sucursal access if needed directly or via service
using System.Linq; // Added for Select

namespace la_paceña_v2.Controllers
{
    public class ReporteController : Controller
    {
        private readonly IReporteService _reporteService;
        private readonly ContextoRestaurante _contexto; // To load dropdowns

        public ReporteController(IReporteService reporteService, ContextoRestaurante contexto)
        {
            _reporteService = reporteService;
            _contexto = contexto;
        }

        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Ventas(DateTime? fechaInicio, DateTime? fechaFin, int? idSucursal, int? idUsuario)
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
            ViewBag.Usuarios = new SelectList(await _contexto.Usuarios.ToListAsync(), "IdUsuario", "NombreCompleto", idUsuario);

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
        public async Task<IActionResult> ExportarVentasExcel(DateTime fechaInicio, DateTime fechaFin, int? idSucursal, int? idUsuario)
        {
             if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
             var fin = fechaFin.Date.AddDays(1).AddSeconds(-1);
             var reporte = await _reporteService.ObtenerReporteVentas(fechaInicio, fin, idSucursal, idUsuario);
             var content = ExportHelper.ExportarVentasExcel(reporte);
             return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Ventas_{DateTime.Now:yyyyMMddHHmm}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> ExportarVentasPDF(DateTime fechaInicio, DateTime fechaFin, int? idSucursal, int? idUsuario)
        {
             if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
             var fin = fechaFin.Date.AddDays(1).AddSeconds(-1);
             var reporte = await _reporteService.ObtenerReporteVentas(fechaInicio, fin, idSucursal, idUsuario);
             var content = ExportHelper.ExportarVentasPDF(reporte);
             return File(content, "application/pdf", $"Ventas_{DateTime.Now:yyyyMMddHHmm}.pdf");
        }

        public async Task<IActionResult> Compras(DateTime? fechaInicio, DateTime? fechaFin, int? idProveedor)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var inicio = fechaInicio ?? DateTime.Today;
            var fin = fechaFin ?? DateTime.Today.AddDays(1).AddSeconds(-1);
             if (fechaFin.HasValue) fin = fechaFin.Value.Date.AddDays(1).AddSeconds(-1);

            var reporte = await _reporteService.ObtenerReporteCompras(inicio, fin, idProveedor);

            ViewBag.Proveedores = new SelectList(await _contexto.Proveedors.ToListAsync(), "IdProveedor", "Empresa", idProveedor);

            var model = new FiltroReporteComprasViewModel
            {
                FechaInicio = inicio,
                FechaFin = fechaFin ?? DateTime.Today,
                IdProveedor = idProveedor,
                Reporte = reporte
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ExportarComprasExcel(DateTime fechaInicio, DateTime fechaFin, int? idProveedor)
        {
             if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
             var fin = fechaFin.Date.AddDays(1).AddSeconds(-1);
             var reporte = await _reporteService.ObtenerReporteCompras(fechaInicio, fin, idProveedor);
             var content = ExportHelper.ExportarComprasExcel(reporte);
             return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Compras_{DateTime.Now:yyyyMMddHHmm}.xlsx");
        }

        [HttpPost]
        public async Task<IActionResult> ExportarComprasPDF(DateTime fechaInicio, DateTime fechaFin, int? idProveedor)
        {
             if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
             var fin = fechaFin.Date.AddDays(1).AddSeconds(-1);
             var reporte = await _reporteService.ObtenerReporteCompras(fechaInicio, fin, idProveedor);
             var content = ExportHelper.ExportarComprasPDF(reporte);
             return File(content, "application/pdf", $"Compras_{DateTime.Now:yyyyMMddHHmm}.pdf");
        }

        public async Task<IActionResult> Stock()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var reporte = await _reporteService.ObtenerReporteStock();
            return View(reporte);
        }
    }
}
