using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace la_paceña_v2.Controllers
{
    public class VentaController : Controller
    {
        private readonly ContextoRestaurante _contexto;

        public VentaController(ContextoRestaurante contexto)
        {
            _contexto = contexto;
        }

        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        // --- LISTADO ---
        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var ventas = _contexto.Ventas
                .Include(v => v.IdClienteNavigation)
                .Include(v => v.IdUsuarioNavigation)
                .Include(v => v.IdTipoPagoNavigation)
                .Include(v => v.IdSucursalNavigation)
                .OrderByDescending(v => v.FechaVenta);
            return View(await ventas.ToListAsync());
        }

        // --- REGISTRAR VENTA (UNIFICADA) ---
        public async Task<IActionResult> Crear()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            // Cargar Clientes con NIT
            var clientes = await _contexto.Clientes.Select(c => new { c.IdCliente, c.RazonSocial, c.NitCarnet }).ToListAsync();
            var jsonOpts = new JsonSerializerOptions { PropertyNamingPolicy = null };
            ViewBag.ClientesJson = JsonSerializer.Serialize(clientes, jsonOpts);
            ViewData["IdCliente"] = new SelectList(clientes, "IdCliente", "RazonSocial");

            ViewData["IdTipoPago"] = new SelectList(_contexto.TipoDePagos.Where(t => t.Estado == true), "IdTipoPago", "Nombre");
            ViewData["IdSucursal"] = new SelectList(_contexto.Sucursals.Where(s => s.Estado == true), "IdSucursal", "Nombre");

            // Cargar Platos para JS
            var platos = await _contexto.Platos
                .Where(p => p.Estado == true)
                .Select(p => new { p.IdPlato, p.Nombre, p.PrecioVenta })
                .ToListAsync();
            ViewBag.Platos = platos;
            ViewBag.PlatosJson = JsonSerializer.Serialize(platos, jsonOpts);

            // Cargar Productos para JS (Fetch stock from lookup)
            var productos = await _contexto.Productos
                .Include(p => p.IdUnidadNavigation)
                .Include(p => p.Stocks)
                .Where(p => p.Estado == true)
                .Select(p => new
                {
                    p.IdProducto,
                    p.Nombre,
                    Unidad = p.IdUnidadNavigation.Abreviatura,
                    p.PrecioVenta,
                    StockActual = p.Stocks.Select(s => s.CantidadActual).FirstOrDefault()
                })
                .ToListAsync();
            ViewBag.Productos = productos;
            ViewBag.ProductosJson = JsonSerializer.Serialize(productos, jsonOpts);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("IdCliente,IdSucursal,NroMesa,IdTipoPago")] Venta venta, string detallesJson)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Autenticacion");

            venta.IdUsuario = idUsuario.Value;
            venta.FechaVenta = DateTime.Now;
            venta.Estado = "PAGADA"; // Por defecto pagada

            List<DetalleVenta> detalles = null;
            if (!string.IsNullOrEmpty(detallesJson))
            {
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    detalles = JsonSerializer.Deserialize<List<DetalleVenta>>(detallesJson, options);
                }
                catch { }
            }

            if (detalles == null || !detalles.Any())
                ModelState.AddModelError("", "Debe agregar al menos un ítem (plato o producto) a la venta.");
            else
            {
                venta.TotalVenta = detalles.Sum(d => d.Subtotal);
            }

            ModelState.Remove("IdUsuarioNavigation");
            ModelState.Remove("IdSucursalNavigation");
            ModelState.Remove("IdClienteNavigation");
            ModelState.Remove("IdTipoPagoNavigation");
            ModelState.Remove("DetalleVentas");
            ModelState.Remove("IdUsuario");
            ModelState.Remove("FechaVenta");
            ModelState.Remove("Estado");
            ModelState.Remove("TotalVenta");

            if (ModelState.IsValid)
            {
                using var transaction = _contexto.Database.BeginTransaction();
                try
                {
                    _contexto.Add(venta);
                    await _contexto.SaveChangesAsync();

                    foreach (var det in detalles)
                    {
                        det.IdVenta = venta.IdVenta;
                        _contexto.Add(det);

                        // LÓGICA PARA PRODUCTO (Control de Stock)
                        if (det.IdProducto.HasValue)
                        {
                            var stockRecord = await _contexto.Stocks.FirstOrDefaultAsync(s => s.IdProducto == det.IdProducto);
                            if (stockRecord == null || stockRecord.CantidadActual < det.Cantidad)
                            {
                                var prod = await _contexto.Productos.FindAsync(det.IdProducto);
                                throw new Exception($"Stock insuficiente para el producto {prod?.Nombre}.");
                            }

                            stockRecord.CantidadActual -= det.Cantidad;
                            stockRecord.UltimaActualizacion = DateTime.Now;
                            _contexto.Stocks.Update(stockRecord);

                            // Kardex Salida
                            _contexto.KardexGenerals.Add(new KardexGeneral
                            {
                                Fecha = DateTime.Now,
                                TipoMovimiento = "VENTA",
                                IdProducto = det.IdProducto,
                                Cantidad = det.Cantidad,
                                CostoUnitario = det.PrecioUnitario,
                                IdReferencia = venta.IdVenta
                            });
                        }
                        // LÓGICA PARA PLATO (Control de Stock de Insumos vía Receta)
                        else if (det.IdPlato.HasValue)
                        {
                            var listaReceta = await _contexto.Receta
                                .Include(r => r.IdInsumoNavigation)
                                .Where(r => r.IdPlato == det.IdPlato)
                                .ToListAsync();

                            foreach (var ingrediente in listaReceta)
                            {
                                var stockInsumo = await _contexto.Stocks.FirstOrDefaultAsync(s => s.IdInsumo == ingrediente.IdInsumo);
                                decimal cantidadRequerida = ingrediente.Cantidad * det.Cantidad;

                                if (stockInsumo == null || stockInsumo.CantidadActual < cantidadRequerida)
                                {
                                    var plato = await _contexto.Platos.FindAsync(det.IdPlato);
                                    string nombreInsumo = ingrediente.IdInsumoNavigation?.Nombre ?? "Desconocido";
                                    decimal stockDisponible = stockInsumo?.CantidadActual ?? 0;

                                    throw new Exception($"Stock insuficiente del insumo '{nombreInsumo}' para preparar el plato '{plato?.Nombre}'. Se requieren {cantidadRequerida}, pero solo hay {stockDisponible} en stock.");
                                }

                                stockInsumo.CantidadActual -= cantidadRequerida;
                                stockInsumo.UltimaActualizacion = DateTime.Now;
                                _contexto.Stocks.Update(stockInsumo);

                                _contexto.KardexGenerals.Add(new KardexGeneral
                                {
                                    Fecha = DateTime.Now,
                                    TipoMovimiento = "VENTA PLATO (INSUMO)",
                                    IdInsumo = ingrediente.IdInsumo,
                                    Cantidad = cantidadRequerida,
                                    CostoUnitario = ingrediente.IdInsumoNavigation?.CostoPromedio,
                                    IdReferencia = venta.IdVenta
                                });
                            }
                        }
                    }

                    await _contexto.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Mensaje"] = "Venta registrada exitosamente";
                    return RedirectToAction(nameof(Listar));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Error al registrar la venta: " + ex.Message);
                }
            }

            ViewData["IdCliente"] = new SelectList(_contexto.Clientes, "IdCliente", "RazonSocial", venta.IdCliente);
            ViewData["IdTipoPago"] = new SelectList(_contexto.TipoDePagos.Where(t => t.Estado == true), "IdTipoPago", "Nombre", venta.IdTipoPago);
            ViewData["IdSucursal"] = new SelectList(_contexto.Sucursals.Where(s => s.Estado == true), "IdSucursal", "Nombre", venta.IdSucursal);

            var clientes = await _contexto.Clientes.Select(c => new { c.IdCliente, c.RazonSocial, c.NitCarnet }).ToListAsync();
            var platos = await _contexto.Platos.Where(p => p.Estado == true).Select(p => new { p.IdPlato, p.Nombre, p.PrecioVenta }).ToListAsync();
            var productos = await _contexto.Productos.Include(p => p.IdUnidadNavigation).Include(p => p.Stocks).Where(p => p.Estado == true).Select(p => new { p.IdProducto, p.Nombre, Unidad = p.IdUnidadNavigation.Abreviatura, p.PrecioVenta, StockActual = p.Stocks.Select(s => s.CantidadActual).FirstOrDefault() }).ToListAsync();

            var jsonOpts = new JsonSerializerOptions { PropertyNamingPolicy = null };
            ViewBag.ClientesJson = JsonSerializer.Serialize(clientes, jsonOpts);
            ViewBag.PlatosJson = JsonSerializer.Serialize(platos, jsonOpts);
            ViewBag.ProductosJson = JsonSerializer.Serialize(productos, jsonOpts);

            ViewBag.Platos = platos;
            ViewBag.Productos = productos;
            ViewBag.DetallesRestaurados = detallesJson;

            return View(venta);
        }

        // --- VER ---
        public async Task<IActionResult> Ver(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();

            var venta = await _contexto.Ventas
                .Include(v => v.IdClienteNavigation)
                .Include(v => v.IdUsuarioNavigation)
                .Include(v => v.IdTipoPagoNavigation)
                .Include(v => v.IdSucursalNavigation)
                .Include(v => v.DetalleVentas).ThenInclude(d => d.IdPlatoNavigation)
                .Include(v => v.DetalleVentas).ThenInclude(d => d.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdVenta == id);

            if (venta == null) return NotFound();

            return View(venta);
        }



        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var venta = await _contexto.Ventas
                .Include(v => v.DetalleVentas)
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta != null)
            {
                try
                {
                    // Restaurar stock si son productos
                    foreach (var det in venta.DetalleVentas)
                    {
                        if (det.IdProducto != null)
                        {
                            var stockRecord = await _contexto.Stocks.FirstOrDefaultAsync(s => s.IdProducto == det.IdProducto);
                            if (stockRecord != null)
                            {
                                stockRecord.CantidadActual += det.Cantidad;
                                _contexto.Stocks.Update(stockRecord);

                                _contexto.KardexGenerals.Add(new KardexGeneral
                                {
                                    Fecha = DateTime.Now,
                                    TipoMovimiento = "ANULACION",
                                    IdProducto = det.IdProducto,
                                    Cantidad = det.Cantidad,
                                    CostoUnitario = det.PrecioUnitario,
                                    IdReferencia = venta.IdVenta
                                });
                            }
                        }
                        else if (det.IdPlato.HasValue)
                        {
                            var listaReceta = await _contexto.Receta.Include(r => r.IdInsumoNavigation).Where(r => r.IdPlato == det.IdPlato).ToListAsync();
                            foreach (var ingrediente in listaReceta)
                            {
                                var stockInsumo = await _contexto.Stocks.FirstOrDefaultAsync(s => s.IdInsumo == ingrediente.IdInsumo);
                                if (stockInsumo != null)
                                {
                                    decimal cantidadADevolver = ingrediente.Cantidad * det.Cantidad;
                                    stockInsumo.CantidadActual += cantidadADevolver;
                                    stockInsumo.UltimaActualizacion = DateTime.Now;
                                    _contexto.Stocks.Update(stockInsumo);

                                    _contexto.KardexGenerals.Add(new KardexGeneral
                                    {
                                        Fecha = DateTime.Now,
                                        TipoMovimiento = "ANULACION PLATO (INSUMO)",
                                        IdInsumo = ingrediente.IdInsumo,
                                        Cantidad = cantidadADevolver,
                                        CostoUnitario = ingrediente.IdInsumoNavigation?.CostoPromedio,
                                        IdReferencia = venta.IdVenta
                                    });
                                }
                            }
                        }
                    }

                    _contexto.DetalleVentas.RemoveRange(venta.DetalleVentas);
                    _contexto.Ventas.Remove(venta);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Venta eliminada exitosamente";
                }
                catch (Exception)
                {
                    TempData["Error"] = "No se puede eliminar la venta.";
                }
            }
            return RedirectToAction(nameof(Listar));
        }
    }
}
