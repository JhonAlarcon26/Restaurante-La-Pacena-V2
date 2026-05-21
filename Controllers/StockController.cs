using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class StockController : Controller
    {
        private readonly ContextoRestaurante _contexto;

        public StockController(ContextoRestaurante contexto)
        {
            _contexto = contexto;
        }

        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var stockList = await _contexto.Stocks
                .Include(s => s.IdInsumoNavigation).ThenInclude(i => i.IdUnidadNavigation)
                .Include(s => s.IdProductoNavigation).ThenInclude(p => p.IdUnidadNavigation)
                .OrderBy(s => s.IdInsumo != null ? s.IdInsumoNavigation.Nombre : s.IdProductoNavigation.Nombre)
                .ToListAsync();

            return View(stockList);
        }

        private async Task CargarListas()
        {
            var insumos = await _contexto.Insumos
                .Include(i => i.IdUnidadNavigation)
                .Where(i => i.Estado != false)
                .OrderBy(i => i.Nombre)
                .ToListAsync();

            var productos = await _contexto.Productos
                .Include(p => p.IdUnidadNavigation)
                .Where(p => p.Estado != false)
                .OrderBy(p => p.Nombre)
                .ToListAsync();

            ViewBag.InsumosLista = insumos.Select(i => new
            {
                Id = i.IdInsumo,
                Nombre = i.Nombre,
                Unidad = i.IdUnidadNavigation?.Abreviatura ?? "N/A"
            }).ToList();

            ViewBag.ProductosLista = productos.Select(p => new
            {
                Id = p.IdProducto,
                Nombre = p.Nombre,
                Unidad = p.IdUnidadNavigation?.Abreviatura ?? "N/A"
            }).ToList();
        }

        public async Task<IActionResult> Crear()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            await CargarListas();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(int? idInsumo, int? idProducto, decimal cantidadActual, decimal cantidadMinima)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            if (idInsumo == null && idProducto == null)
            {
                ModelState.AddModelError("", "Debe seleccionar un insumo o un producto");
                await CargarListas();
                return View();
            }

            if (idInsumo == 0) idInsumo = null;
            if (idProducto == 0) idProducto = null;

            if (idInsumo.HasValue)
            {
                idProducto = null;
                if (await _contexto.Stocks.AnyAsync(s => s.IdInsumo == idInsumo))
                {
                    ModelState.AddModelError("", "Este insumo ya tiene un registro de stock. Utilice la opción Editar.");
                    await CargarListas();
                    return View();
                }
            }
            else if (idProducto.HasValue)
            {
                idInsumo = null;
                if (await _contexto.Stocks.AnyAsync(s => s.IdProducto == idProducto))
                {
                    ModelState.AddModelError("", "Este producto ya tiene un registro de stock. Utilice la opción Editar.");
                    await CargarListas();
                    return View();
                }
            }

            try
            {
                var nuevoStock = new Stock
                {
                    IdInsumo = idInsumo,
                    IdProducto = idProducto,
                    IdRestaurante = 1,
                    CantidadActual = cantidadActual,
                    CantidadMinima = cantidadMinima,
                    UltimaActualizacion = DateTime.Now
                };

                _contexto.Add(nuevoStock);
                await _contexto.SaveChangesAsync();
                TempData["Mensaje"] = "Registro de stock creado correctamente";
                return RedirectToAction(nameof(Listar));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al crear el registro: " + ex.Message;
                await CargarListas();
                return View();
            }
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();

            var stock = await _contexto.Stocks
                .Include(s => s.IdInsumoNavigation).ThenInclude(i => i.IdUnidadNavigation)
                .Include(s => s.IdProductoNavigation).ThenInclude(p => p.IdUnidadNavigation)
                .FirstOrDefaultAsync(m => m.IdStock == id);

            if (stock == null) return NotFound();

            await CargarListas();

            return View(stock);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdStock,IdInsumo,IdProducto,CantidadActual,CantidadMinima")] Stock stock)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id != stock.IdStock) return NotFound();

            if (stock.IdInsumo == 0) stock.IdInsumo = null;
            if (stock.IdProducto == 0) stock.IdProducto = null;

            if (stock.IdInsumo.HasValue)
                stock.IdProducto = null;
            else if (stock.IdProducto.HasValue)
                stock.IdInsumo = null;

            if (stock.IdInsumo == null && stock.IdProducto == null)
            {
                ModelState.AddModelError("", "Debe seleccionar un insumo o un producto");
                await CargarListas();
                var stockOriginal = await _contexto.Stocks
                    .AsNoTracking()
                    .Include(s => s.IdInsumoNavigation).ThenInclude(i => i.IdUnidadNavigation)
                    .Include(s => s.IdProductoNavigation).ThenInclude(p => p.IdUnidadNavigation)
                    .FirstOrDefaultAsync(s => s.IdStock == id);
                return View(stockOriginal ?? stock);
            }

            try
            {
                var stockExistente = await _contexto.Stocks.FindAsync(id);
                if (stockExistente == null) return NotFound();

                stockExistente.IdInsumo = stock.IdInsumo;
                stockExistente.IdProducto = stock.IdProducto;
                stockExistente.CantidadActual = stock.CantidadActual;
                stockExistente.CantidadMinima = stock.CantidadMinima;
                stockExistente.UltimaActualizacion = DateTime.Now;

                await _contexto.SaveChangesAsync();
                TempData["Mensaje"] = "Configuración de stock actualizada correctamente";
                return RedirectToAction(nameof(Listar));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StockExists(stock.IdStock)) return NotFound();
                else throw;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar: " + ex.Message;
                await CargarListas();
                return View(stock);
            }
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();

            var stock = await _contexto.Stocks
                .Include(s => s.IdInsumoNavigation).ThenInclude(i => i.IdUnidadNavigation)
                .Include(s => s.IdProductoNavigation).ThenInclude(p => p.IdUnidadNavigation)
                .FirstOrDefaultAsync(m => m.IdStock == id);

            if (stock == null) return NotFound();

            return View(stock);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var stock = await _contexto.Stocks.FindAsync(id);
            if (stock != null)
            {
                _contexto.Stocks.Remove(stock);
                await _contexto.SaveChangesAsync();
                TempData["Mensaje"] = "Registro de stock eliminado correctamente";
            }

            return RedirectToAction(nameof(Listar));
        }

        private bool StockExists(int id)
        {
            return _contexto.Stocks.Any(e => e.IdStock == id);
        }
    }
}
