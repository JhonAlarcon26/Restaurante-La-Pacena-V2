using la_paceña_v2.Datos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class KardexController : Controller
    {
        private readonly ContextoRestaurante _contexto;

        public KardexController(ContextoRestaurante contexto)
        {
            _contexto = contexto;
        }

        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Index()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            // Mostrar el historial completo unificado
            // Podemos usar la VistaHistorialCompleto si existe y funciona, o query directo a KardexGeneral

            // Opcion 1: Usar VistaHistorialCompleto (si SQL view esta actualizada)
            // Opcion 2: Query a KardexGeneral + Includes

            // Vamos con Opcion 2 para asegurar, ya que KardexGeneral es la tabla real ahora.
            var kardex = _contexto.KardexGenerals
                .Include(k => k.IdInsumoNavigation).ThenInclude(i => i.IdUnidadNavigation)
                .Include(k => k.IdProductoNavigation).ThenInclude(p => p.IdUnidadNavigation)
                .OrderByDescending(k => k.Fecha);

            return View(await kardex.ToListAsync());
        }
    }
}
