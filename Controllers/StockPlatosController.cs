using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class StockPlatoController : Controller
    {
        private readonly ContextoRestaurante _contexto;

        public StockPlatoController(ContextoRestaurante contexto)
        {
            _contexto = contexto;
        }

        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var stockList = await _contexto.StockPlatos
                .Include(s => s.IdPlatoNavigation)
                .OrderBy(s => s.IdPlatoNavigation.Nombre)
                .ToListAsync();

            return View(stockList);
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();

            var stock = await _contexto.StockPlatos
                .Include(s => s.IdPlatoNavigation)
                .FirstOrDefaultAsync(m => m.IdStockPlato == id);

            if (stock == null) return NotFound();

            return View(stock);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdStockPlato,IdPlato,CantidadActual,CantidadMinima")] StockPlato stock)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id != stock.IdStockPlato) return NotFound();

            try
            {
                var stockExistente = await _contexto.StockPlatos.FindAsync(id);
                if (stockExistente == null) return NotFound();

                stockExistente.CantidadActual = stock.CantidadActual;
                stockExistente.CantidadMinima = stock.CantidadMinima;
                stockExistente.UltimaActualizacion = DateTime.Now;

                await _contexto.SaveChangesAsync();
                TempData["Mensaje"] = "Configuración de stock actualizada correctamente";
                return RedirectToAction(nameof(Listar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar: " + ex.Message;
                return View(stock);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            try
            {
                var stock = await _contexto.StockPlatos.FindAsync(id);
                if (stock != null)
                {
                    _contexto.StockPlatos.Remove(stock);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Registro de stock eliminado correctamente";
                }
                return RedirectToAction(nameof(Listar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo eliminar el stock: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LimpiarStock()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            try
            {
                var stocks = await _contexto.StockPlatos.ToListAsync();
                foreach (var stock in stocks)
                {
                    stock.CantidadActual = 0;
                    stock.UltimaActualizacion = DateTime.Now;
                }
                await _contexto.SaveChangesAsync();
                return RedirectToAction(nameof(Listar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al limpiar el stock: " + ex.Message;
                return RedirectToAction(nameof(Listar));
            }
        }
    }
}
