using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace la_paceña_v2.Controllers
{
    public class AbastecimientoProductoController : Controller
    {
        private readonly ContextoRestaurante _contexto;

        public AbastecimientoProductoController(ContextoRestaurante contexto)
        {
            _contexto = contexto;
        }

        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var abastecimientos = _contexto.Abastecimientos
                .Include(a => a.IdProveedorNavigation)
                .Include(a => a.IdUsuarioNavigation)
                .OrderByDescending(a => a.FechaAbastecimiento);
            return View(await abastecimientos.ToListAsync());
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

        public class DetalleAbastecimientoExtendido : DetalleAbastecimiento
        {
            public decimal? PrecioVentaNuevo { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("IdProveedor,NroFacturaProveedor,TotalAbastecimiento")] Abastecimiento abastecimiento, string detallesJson)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Autenticacion");

            abastecimiento.IdUsuario = idUsuario.Value;
            abastecimiento.FechaAbastecimiento = DateTime.Now;

            if (abastecimiento.IdProveedor <= 0)
                ModelState.AddModelError("IdProveedor", "Debe seleccionar un proveedor de la lista.");

            List<DetalleAbastecimientoExtendido> detalles = null;
            if (!string.IsNullOrEmpty(detallesJson))
            {
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    detalles = JsonSerializer.Deserialize<List<DetalleAbastecimientoExtendido>>(detallesJson, options);
                }
                catch { }
            }

            if (detalles == null || !detalles.Any())
                ModelState.AddModelError("", "Debe agregar al menos un producto al abastecimiento.");
            else
            {
                abastecimiento.TotalAbastecimiento = detalles.Sum(d => d.Subtotal);
            }

            ModelState.Remove("IdUsuarioNavigation");
            ModelState.Remove("IdProveedorNavigation");
            ModelState.Remove("DetalleAbastecimientos");

            if (ModelState.IsValid)
            {
                using var transaction = _contexto.Database.BeginTransaction();
                try
                {
                    _contexto.Add(abastecimiento);
                    await _contexto.SaveChangesAsync();

                    foreach (var det in detalles)
                    {
                        var detalleReal = new DetalleAbastecimiento
                        {
                            IdAbastecimiento = abastecimiento.IdAbastecimiento,
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
                                IdReferencia = abastecimiento.IdAbastecimiento
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
            return View(abastecimiento);
        }

        public async Task<IActionResult> Ver(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();

            var abastecimiento = await _contexto.Abastecimientos
                .Include(a => a.IdProveedorNavigation)
                .Include(a => a.IdUsuarioNavigation)
                .Include(a => a.DetalleAbastecimientos).ThenInclude(d => d.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdAbastecimiento == id);

            if (abastecimiento == null) return NotFound();

            return View(abastecimiento);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var abastecimiento = await _contexto.Abastecimientos
                .Include(a => a.DetalleAbastecimientos)
                .FirstOrDefaultAsync(a => a.IdAbastecimiento == id);

            if (abastecimiento != null)
            {
                try
                {
                    foreach (var det in abastecimiento.DetalleAbastecimientos)
                    {
                        var stockRecord = await _contexto.Stocks.FirstOrDefaultAsync(s => s.IdProducto == det.IdProducto);
                        if (stockRecord != null)
                        {
                            stockRecord.CantidadActual -= det.Cantidad;
                            _contexto.Stocks.Update(stockRecord);
                        }
                    }

                    _contexto.DetalleAbastecimientos.RemoveRange(abastecimiento.DetalleAbastecimientos);
                    _contexto.Abastecimientos.Remove(abastecimiento);
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
