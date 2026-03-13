using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using la_paceña_v2.Datos;

namespace la_paceña_v2.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ContextoRestaurante _contexto;
        private readonly la_paceña_v2.Services.IReporteService _reporteService;

        public DashboardController(ContextoRestaurante contexto, la_paceña_v2.Services.IReporteService reporteService)
        {
            _contexto = contexto;
            _reporteService = reporteService;
        }

        public async Task<IActionResult> Index()
        {

            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
            {
                return RedirectToAction("Login", "Autenticacion");
            }

            ViewBag.NombreUsuario = HttpContext.Session.GetString("NombreCompleto");
            ViewBag.NombreRol = HttpContext.Session.GetString("NombreRol");
            ViewBag.NombreSucursal = HttpContext.Session.GetString("NombreSucursal");

            var kpis = await _reporteService.ObtenerKPIsDashboard();

            ViewBag.TotalCategorias = await _contexto.CategoriaPlatos.CountAsync();
            ViewBag.CategoriasActivas = await _contexto.CategoriaPlatos.CountAsync(c => c.Estado == true);
            ViewBag.TotalPlatos = await _contexto.Platos.CountAsync();
            ViewBag.TotalInsumos = await _contexto.Insumos.CountAsync();
            ViewBag.TotalUsuarios = await _contexto.Usuarios.CountAsync();
            ViewBag.TotalProveedores = await _contexto.Proveedors.CountAsync();

            return View(kpis);
        }
    }
}
