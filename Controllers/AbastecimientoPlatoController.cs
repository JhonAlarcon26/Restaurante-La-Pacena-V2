using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace la_paceña_v2.Controllers
{
    public class AbastecimientoPlatoController : Controller
    {
        private readonly ContextoRestaurante _contexto;

        public AbastecimientoPlatoController(ContextoRestaurante contexto)
        {
            _contexto = contexto;
        }

        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var abastecimientos = _contexto.AbastecimientoPlatos
                .Include(a => a.IdUsuarioNavigation)
                .Include(a => a.DetalleAbastecimientoPlatos)
                    .ThenInclude(d => d.IdPlatoNavigation)
                .OrderByDescending(a => a.FechaAbastecimiento);
            return View(await abastecimientos.ToListAsync());
        }

        private async Task CargarListasParaCrearAsync()
        {
            var jsonOpts = new JsonSerializerOptions { PropertyNamingPolicy = null };

            var platos = await _contexto.Platos
                .Where(p => p.Estado != false)
                .Select(p => new { 
                    p.IdPlato, 
                    p.Nombre, 
                    PrecioCompra = _contexto.DetalleAbastecimientoPlatos
                                    .Where(d => d.IdPlato == p.IdPlato)
                                    .OrderByDescending(d => d.IdDetalleAbastecimientoPlato)
                                    .Select(d => d.PrecioUnitario)
                                    .FirstOrDefault() 
                })
                .ToListAsync();
            ViewBag.PlatosJson = JsonSerializer.Serialize(platos, jsonOpts);
            ViewBag.Platos = platos;
        }

        public async Task<IActionResult> Crear()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            await CargarListasParaCrearAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("TotalAbastecimiento")] AbastecimientoPlato CompraProducto, string detallesJson)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Autenticacion");

            CompraProducto.IdUsuario = idUsuario.Value;
            CompraProducto.FechaAbastecimiento = DateTime.Now;

            List<DetalleAbastecimientoPlato> detalles = null;
            if (!string.IsNullOrEmpty(detallesJson))
            {
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    detalles = JsonSerializer.Deserialize<List<DetalleAbastecimientoPlato>>(detallesJson, options);
                }
                catch { }
            }

            if (detalles == null || !detalles.Any())
                ModelState.AddModelError("", "Debe agregar al menos un plato al CompraProducto.");
            else
            {
                CompraProducto.TotalAbastecimiento = detalles.Sum(d => d.Subtotal);
            }

            ModelState.Remove("IdUsuarioNavigation");
            ModelState.Remove("DetalleAbastecimientoPlatos");

            if (ModelState.IsValid)
            {
                using var transaction = _contexto.Database.BeginTransaction();
                try
                {
                    _contexto.Add(CompraProducto);
                    await _contexto.SaveChangesAsync();

                    foreach (var det in detalles)
                    {
                        var detalleReal = new DetalleAbastecimientoPlato
                        {
                            IdAbastecimientoPlato = CompraProducto.IdAbastecimientoPlato,
                            IdPlato = det.IdPlato,
                            Cantidad = det.Cantidad,
                            PrecioUnitario = det.PrecioUnitario,
                            Subtotal = det.Subtotal
                        };
                        _contexto.Add(detalleReal);

                        var stockRecord = await _contexto.StockPlatos.FirstOrDefaultAsync(s => s.IdPlato == det.IdPlato);
                        if (stockRecord == null)
                        {
                            stockRecord = new StockPlato
                            {
                                IdPlato = det.IdPlato,
                                IdRestaurante = 1,
                                CantidadActual = det.Cantidad,
                                CantidadMinima = 5,
                                UltimaActualizacion = DateTime.Now
                            };
                            _contexto.StockPlatos.Add(stockRecord);
                        }
                        else
                        {
                            stockRecord.CantidadActual += det.Cantidad;
                            stockRecord.UltimaActualizacion = DateTime.Now;
                            _contexto.StockPlatos.Update(stockRecord);
                        }

                        _contexto.HistorialDeMovimientos.Add(new HistorialDeMovimiento
                        {
                            Fecha = DateTime.Now,
                            TipoMovimiento = "Abastecimiento Platos",
                            IdPlato = det.IdPlato,
                            Cantidad = det.Cantidad,
                            CostoUnitario = det.PrecioUnitario,
                            IdReferencia = CompraProducto.IdAbastecimientoPlato
                        });
                    }

                    await _contexto.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Mensaje"] = "CompraProducto de platos registrado exitosamente";
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
            return View(CompraProducto);
        }

        public async Task<IActionResult> Ver(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();

            var CompraProducto = await _contexto.AbastecimientoPlatos
                .Include(a => a.IdUsuarioNavigation)
                .Include(a => a.DetalleAbastecimientoPlatos).ThenInclude(d => d.IdPlatoNavigation)
                .FirstOrDefaultAsync(m => m.IdAbastecimientoPlato == id);

            if (CompraProducto == null) return NotFound();

            return View(CompraProducto);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var CompraProducto = await _contexto.AbastecimientoPlatos
                .Include(a => a.DetalleAbastecimientoPlatos)
                .FirstOrDefaultAsync(a => a.IdAbastecimientoPlato == id);

            if (CompraProducto != null)
            {
                try
                {
                    foreach (var det in CompraProducto.DetalleAbastecimientoPlatos)
                    {
                        var stockRecord = await _contexto.StockPlatos.FirstOrDefaultAsync(s => s.IdPlato == det.IdPlato);
                        if (stockRecord != null)
                        {
                            stockRecord.CantidadActual -= det.Cantidad;
                            _contexto.StockPlatos.Update(stockRecord);
                        }
                    }

                    _contexto.DetalleAbastecimientoPlatos.RemoveRange(CompraProducto.DetalleAbastecimientoPlatos);
                    _contexto.AbastecimientoPlatos.Remove(CompraProducto);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "CompraProducto de platos eliminado exitosamente";
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
