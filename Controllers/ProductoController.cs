using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class ProductoController : Controller
    {
        private readonly ContextoRestaurante _contexto;
        public ProductoController(ContextoRestaurante contexto) => _contexto = contexto;
        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var productos = await _contexto.Productos
                .Include(p => p.IdTipoProductoNavigation)
                .Include(p => p.IdUnidadNavigation)
                .Include(p => p.Stocks)
                .ToListAsync();
            return View(productos);
        }

        public async Task<IActionResult> Crear()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            ViewBag.TiposProducto = new SelectList(await _contexto.TipoProductos.Where(c => c.Estado == true).ToListAsync(), "IdTipoProducto", "Nombre");
            ViewBag.Unidades = new SelectList(await _contexto.UnidadDeMedida.ToListAsync(), "IdUnidad", "Nombre");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Nombre,CodigoBarras,IdTipoProducto,PrecioCompraActual,PrecioVenta,IdUnidad")] Producto producto)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            if (string.IsNullOrWhiteSpace(producto.Nombre))
            {
                TempData["Error"] = "El nombre del producto es obligatorio";
                ViewBag.TiposProducto = new SelectList(await _contexto.TipoProductos.Where(c => c.Estado == true).ToListAsync(), "IdTipoProducto", "Nombre", producto.IdTipoProducto);
                ViewBag.Unidades = new SelectList(await _contexto.UnidadDeMedida.ToListAsync(), "IdUnidad", "Nombre", producto.IdUnidad);
                return View(producto);
            }

            if (producto.PrecioVenta <= 0)
            {
                TempData["Error"] = "El precio de venta debe ser mayor a 0";
                ViewBag.TiposProducto = new SelectList(await _contexto.TipoProductos.Where(c => c.Estado == true).ToListAsync(), "IdTipoProducto", "Nombre", producto.IdTipoProducto);
                ViewBag.Unidades = new SelectList(await _contexto.UnidadDeMedida.ToListAsync(), "IdUnidad", "Nombre", producto.IdUnidad);
                return View(producto);
            }

            producto.Estado = true;

            _contexto.Add(producto);
            await _contexto.SaveChangesAsync();

            var stock = new Stock
            {
                IdProducto = producto.IdProducto,
                CantidadActual = 0,
                CantidadMinima = 0,
                UltimaActualizacion = DateTime.Now
            };
            _contexto.Stocks.Add(stock);
            await _contexto.SaveChangesAsync();

            TempData["Mensaje"] = "Producto registrado exitosamente";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var producto = await _contexto.Productos
                .Include(p => p.Stocks)
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null) return NotFound();

            ViewBag.TiposProducto = new SelectList(await _contexto.TipoProductos.Where(c => c.Estado == true).ToListAsync(), "IdTipoProducto", "Nombre", producto.IdTipoProducto);
            ViewBag.Unidades = new SelectList(await _contexto.UnidadDeMedida.ToListAsync(), "IdUnidad", "Nombre", producto.IdUnidad);
            return View(producto);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdProducto,Nombre,CodigoBarras,IdTipoProducto,PrecioCompraActual,PrecioVenta,Estado,IdUnidad")] Producto producto)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id != producto.IdProducto) return NotFound();

            if (string.IsNullOrWhiteSpace(producto.Nombre))
            {
                TempData["Error"] = "El nombre del producto es obligatorio";
                ViewBag.TiposProducto = new SelectList(await _contexto.TipoProductos.Where(c => c.Estado == true).ToListAsync(), "IdTipoProducto", "Nombre", producto.IdTipoProducto);
                ViewBag.Unidades = new SelectList(await _contexto.UnidadDeMedida.ToListAsync(), "IdUnidad", "Nombre", producto.IdUnidad);
                return View(producto);
            }

            if (producto.PrecioVenta <= 0)
            {
                TempData["Error"] = "El precio de venta debe ser mayor a 0";
                ViewBag.TiposProducto = new SelectList(await _contexto.TipoProductos.Where(c => c.Estado == true).ToListAsync(), "IdTipoProducto", "Nombre", producto.IdTipoProducto);
                ViewBag.Unidades = new SelectList(await _contexto.UnidadDeMedida.ToListAsync(), "IdUnidad", "Nombre", producto.IdUnidad);
                return View(producto);
            }

            _contexto.Update(producto);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Producto actualizado";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var producto = await _contexto.Productos
                .Include(p => p.IdTipoProductoNavigation)
                .Include(p => p.IdUnidadNavigation)
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            return producto == null ? NotFound() : View(producto);
        }

        [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var producto = await _contexto.Productos.FindAsync(id);
            if (producto != null)
            {
                try
                {
                    _contexto.Productos.Remove(producto);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Producto eliminado";
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "No se puede eliminar este producto porque tiene registros asociados (compras, ventas, etc.).";
                }
                catch (InvalidOperationException)
                {
                    TempData["Error"] = "No se puede eliminar este producto porque tiene registros asociados.";
                }
            }
            return RedirectToAction(nameof(Listar));
        }
    }
}
