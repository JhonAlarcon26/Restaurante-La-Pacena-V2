using la_paceña_v2.Datos;
using la_paceña_v2.Models.ViewModels;
using la_paceña_v2.Services;
using la_paceña_v2.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class ReporteCompraController : Controller
    {
        private readonly IReporteService _reporteService;
        private readonly ContextoRestaurante _contexto;

        public ReporteCompraController(IReporteService reporteService, ContextoRestaurante contexto)
        {
            _reporteService = reporteService;
            _contexto = contexto;
        }

        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public IActionResult Index()
        {
            return RedirectToAction("Index", "ReporteCompraInsumo");
        }
    }
}
