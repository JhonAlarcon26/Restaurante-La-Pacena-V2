using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace la_paceña_v2.Controllers
{
    public class AvastecimientoController : Controller
    {
        private readonly ContextoRestaurante _contexto;

        public AvastecimientoController(ContextoRestaurante contexto)
        {
            _contexto = contexto;
        }

        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var avastecimientos = _contexto.Avastecimientos
                .Include(a => a.IdProveedorNavigation)
                .Include(a => a.IdUsuarioNavigation)
                .OrderByDescending(a => a.FechaAvastecimiento);
            return View(await avastecimientos.ToListAsync());
        }

        private async Task CargarListasParaCrearAsync()
        {
            var proveedores = await _contexto.Proveedors
                .Where(p => p.Estado != false)
                .Select(p => new { p.IdProveedor, Empresa = p.NombreEmpresa, p.NitCarnet })
                .ToListAsync();
            var jsonOpts = new JsonSerializerOptions { PropertyNamingPolicy = null };
            ViewBag.ProveedoresJson = JsonSerializer.Serialize(proveedores, jsonOpts);
            ViewData["IdProveedor"] = new SelectList(proveedores, "IdProveedor", "Empresa");

            var productos = await _contexto.Productos
                .Include(p => p.IdUnidadNavigation)
                .Where(p => p.Estado != false)
                .Select(p => new { p.IdProducto, p.Nombre, Unidad = p.IdUnidadNavigation.Abreviatura, p.PrecioCompraActual, p.PrecioVenta })
                .ToListAsync();
            ViewBag.ProductosJson = JsonSerializer.Serialize(productos, jsonOpts);
            ViewBag.Productos = productos;
        }

        public async Task<IActionResult> Crear()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            await CargarListasParaCrearAsync();

            return View();
        }

        public class DetalleAvastecimientoExtendido : DetalleAvastecimiento
        {
            public decimal? PrecioVentaNuevo { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("IdProveedor,NroReferencia,TotalAvastecimiento")] Avastecimiento avastecimiento, string detallesJson)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Autenticacion");

            avastecimiento.IdUsuario = idUsuario.Value;
            avastecimiento.FechaAvastecimiento = DateTime.Now;

            if (avastecimiento.IdProveedor <= 0)
                ModelState.AddModelError("IdProveedor", "Debe seleccionar un proveedor de la lista.");

            List<DetalleAvastecimientoExtendido> detalles = null;
            if (!string.IsNullOrEmpty(detallesJson))
            {
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    detalles = JsonSerializer.Deserialize<List<DetalleAvastecimientoExtendido>>(detallesJson, options);
                }
                catch { }
            }

            if (detalles == null || !detalles.Any())
                ModelState.AddModelError("", "Debe agregar al menos un producto al abastecimiento.");
            else
            {
                avastecimiento.TotalAvastecimiento = detalles.Sum(d => d.Subtotal);
            }

            ModelState.Remove("IdUsuarioNavigation");
            ModelState.Remove("IdProveedorNavigation");
            ModelState.Remove("DetalleAvastecimiento");

            if (ModelState.IsValid)
            {
                using var transaction = _contexto.Database.BeginTransaction();
                try
                {
                    _contexto.Add(avastecimiento);
                    await _contexto.SaveChangesAsync();

                    foreach (var det in detalles)
                    {
                        var detalleReal = new DetalleAvastecimiento
                        {
                            IdAvastecimiento = avastecimiento.IdAvastecimiento,
                            IdProducto = det.IdProducto,
                            Cantidad = det.Cantidad,
                            PrecioUnitario = det.PrecioUnitario,
                            Subtotal = det.Subtotal
                        };
                        _contexto.Add(detalleReal);

                        var producto = await _contexto.Productos.FindAsync(det.IdProducto);
                        if (producto != null)
                        {
                            producto.PrecioCompraActual = det.PrecioUnitario;
                            if (det.PrecioVentaNuevo.HasValue && det.PrecioVentaNuevo > 0)
                            {
                                producto.PrecioVenta = det.PrecioVentaNuevo.Value;
                            }
                            _contexto.Update(producto);

                            var stockRecord = await _contexto.Stocks.FirstOrDefaultAsync(s => s.IdProducto == det.IdProducto);
                            if (stockRecord == null)
                            {
                                stockRecord = new Stock
                                {
                                    IdProducto = det.IdProducto,
                                    CantidadActual = det.Cantidad,
                                    CantidadMinima = 5,
                                    UltimaActualizacion = DateTime.Now
                                };
                                _contexto.Stocks.Add(stockRecord);
                            }
                            else
                            {
                                stockRecord.CantidadActual += det.Cantidad;
                                stockRecord.UltimaActualizacion = DateTime.Now;
                                _contexto.Stocks.Update(stockRecord);
                            }

                            _contexto.KardexGenerals.Add(new KardexGeneral
                            {
                                Fecha = DateTime.Now,
                                TipoMovimiento = "ABASTECIMIENTO",
                                IdProducto = det.IdProducto,
                                Cantidad = det.Cantidad,
                                CostoUnitario = det.PrecioUnitario,
                                IdReferencia = avastecimiento.IdAvastecimiento
                            });
                        }
                    }

                    await _contexto.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Mensaje"] = "Abastecimiento registrado exitosamente";
                    return RedirectToAction(nameof(Listar));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Error al registrar: " + ex.Message);
                }
            }

            ViewBag.DetallesRestaurados = detallesJson;
            await CargarListasParaCrearAsync();
            return View(avastecimiento);
        }

        public async Task<IActionResult> Ver(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();

            var avastecimiento = await _contexto.Avastecimientos
                .Include(a => a.IdProveedorNavigation)
                .Include(a => a.IdUsuarioNavigation)
                .Include(a => a.DetalleAvastecimiento).ThenInclude(d => d.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdAvastecimiento == id);

            if (avastecimiento == null) return NotFound();

            return View(avastecimiento);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var avastecimiento = await _contexto.Avastecimientos
                .Include(a => a.DetalleAvastecimiento)
                .FirstOrDefaultAsync(a => a.IdAvastecimiento == id);

            if (avastecimiento != null)
            {
                try
                {
                    foreach (var det in avastecimiento.DetalleAvastecimiento)
                    {
                        var stockRecord = await _contexto.Stocks.FirstOrDefaultAsync(s => s.IdProducto == det.IdProducto);
                        if (stockRecord != null)
                        {
                            stockRecord.CantidadActual -= det.Cantidad;
                            _contexto.Stocks.Update(stockRecord);
                        }
                    }

                    _contexto.DetalleAvastecimientos.RemoveRange(avastecimiento.DetalleAvastecimiento);
                    _contexto.Avastecimientos.Remove(avastecimiento);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Abastecimiento eliminado exitosamente";
                }
                catch (Exception)
                {
                    TempData["Error"] = "No se puede eliminar.";
                }
            }
            return RedirectToAction(nameof(Listar));
        }
    }
}
