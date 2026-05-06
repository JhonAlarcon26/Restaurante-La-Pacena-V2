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
                .OrderByDescending(a => a.FechaAbastecimiento);
            return View(await abastecimientos.ToListAsync());
        }

        private async Task CargarListasParaCrearAsync()
        {
            var jsonOpts = new JsonSerializerOptions { PropertyNamingPolicy = null };

            var platos = await _contexto.Platos
                .Where(p => p.Estado != false)
                .Select(p => new { p.IdPlato, p.Nombre, p.PrecioVenta })
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
        public async Task<IActionResult> Crear([Bind("NroFacturaProveedor,TotalAbastecimiento")] AbastecimientoPlato abastecimiento, string detallesJson)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null) return RedirectToAction("Login", "Autenticacion");

            abastecimiento.IdUsuario = idUsuario.Value;
            abastecimiento.FechaAbastecimiento = DateTime.Now;

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
                ModelState.AddModelError("", "Debe agregar al menos un plato al abastecimiento.");
            else
            {
                abastecimiento.TotalAbastecimiento = detalles.Sum(d => d.Subtotal);
            }

            ModelState.Remove("IdUsuarioNavigation");
            ModelState.Remove("DetalleAbastecimientoPlatos");

            if (ModelState.IsValid)
            {
                using var transaction = _contexto.Database.BeginTransaction();
                try
                {
                    _contexto.Add(abastecimiento);
                    await _contexto.SaveChangesAsync();

                    foreach (var det in detalles)
                    {
                        var detalleReal = new DetalleAbastecimientoPlato
                        {
                            IdAbastecimientoPlato = abastecimiento.IdAbastecimientoPlato,
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

                        _contexto.KardexGenerals.Add(new KardexGeneral
                        {
                            Fecha = DateTime.Now,
                            TipoMovimiento = "ENTRADA",
                            IdPlato = det.IdPlato,
                            Cantidad = det.Cantidad,
                            CostoUnitario = det.PrecioUnitario,
                            IdReferencia = abastecimiento.IdAbastecimientoPlato
                        });
                    }

                    await _contexto.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Mensaje"] = "Abastecimiento de platos registrado exitosamente";
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

            var abastecimiento = await _contexto.AbastecimientoPlatos
                .Include(a => a.IdUsuarioNavigation)
                .Include(a => a.DetalleAbastecimientoPlatos).ThenInclude(d => d.IdPlatoNavigation)
                .FirstOrDefaultAsync(m => m.IdAbastecimientoPlato == id);

            if (abastecimiento == null) return NotFound();

            return View(abastecimiento);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var abastecimiento = await _contexto.AbastecimientoPlatos
                .Include(a => a.DetalleAbastecimientoPlatos)
                .FirstOrDefaultAsync(a => a.IdAbastecimientoPlato == id);

            if (abastecimiento != null)
            {
                try
                {
                    foreach (var det in abastecimiento.DetalleAbastecimientoPlatos)
                    {
                        var stockRecord = await _contexto.StockPlatos.FirstOrDefaultAsync(s => s.IdPlato == det.IdPlato);
                        if (stockRecord != null)
                        {
                            stockRecord.CantidadActual -= det.Cantidad;
                            _contexto.StockPlatos.Update(stockRecord);
                        }
                    }

                    _contexto.DetalleAbastecimientoPlatos.RemoveRange(abastecimiento.DetalleAbastecimientoPlatos);
                    _contexto.AbastecimientoPlatos.Remove(abastecimiento);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Abastecimiento de platos eliminado exitosamente";
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
