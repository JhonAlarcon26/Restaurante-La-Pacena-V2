using la_paceña_v2.Datos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class HistorialDeMovimientosController : Controller
    {
        private readonly ContextoRestaurante _contexto;

        public HistorialDeMovimientosController(ContextoRestaurante contexto)
        {
            _contexto = contexto;
        }

        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");


            var kardexLista = await _contexto.HistorialDeMovimientos
                .Include(k => k.IdInsumoNavigation).ThenInclude(i => i.IdUnidadNavigation)
                .Include(k => k.IdProductoNavigation).ThenInclude(p => p.IdUnidadNavigation)
                .Include(k => k.IdPlatoNavigation)
                .OrderByDescending(k => k.Fecha)
                .ToListAsync();

            return View(kardexLista);
        }
    }
}
