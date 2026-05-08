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
        public async Task<IActionResult> Crear([Bind("Nombre,CodigoBarras,IdTipoProducto,IdUnidad")] Producto producto)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            if (string.IsNullOrWhiteSpace(producto.Nombre))
            {
                TempData["Error"] = "El nombre del producto es obligatorio";
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
        public async Task<IActionResult> Editar(int id, [Bind("IdProducto,Nombre,CodigoBarras,IdTipoProducto,Estado,IdUnidad")] Producto producto)
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
            if (producto == null) return RedirectToAction(nameof(Listar));

            var tieneAbastecimientos = await _contexto.DetalleAbastecimientos.AnyAsync(da => da.IdProducto == id);
            var tieneVentas = await _contexto.DetalleVentas.AnyAsync(dv => dv.IdProducto == id);

            if (tieneAbastecimientos || tieneVentas)
            {
                TempData["Error"] = "No se puede eliminar el producto porque tiene movimientos registrados (abastecimientos o ventas). Puede desactivarlo en su lugar.";
                return RedirectToAction(nameof(Listar));
            }

            using var transaccion = await _contexto.Database.BeginTransactionAsync();
            try
            {
                var stockAsociado = await _contexto.Stocks.FirstOrDefaultAsync(s => s.IdProducto == id);
                if (stockAsociado != null)
                {
                    _contexto.Stocks.Remove(stockAsociado);
                }

                _contexto.Productos.Remove(producto);
                await _contexto.SaveChangesAsync();
                await transaccion.CommitAsync();

                TempData["Mensaje"] = "Producto eliminado exitosamente junto con su registro de stock.";
            }
            catch (Exception)
            {
                await transaccion.RollbackAsync();
                TempData["Error"] = "Error al eliminar el producto. Asegúrese de que no tenga otros registros vinculados.";
            }

            return RedirectToAction(nameof(Listar));
        }
    }
}
