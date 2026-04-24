using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using la_paceña_v2.Services.DTOs; // Added
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json; // Added

namespace la_paceña_v2.Controllers
{
    public class RecetaController : Controller
    {
        private readonly ContextoRestaurante _contexto;
        public RecetaController(ContextoRestaurante contexto) => _contexto = contexto;
        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            // Agrupar por plato: obtenemos los platos que tienen recetas
            var platosConRecetas = await _contexto.Platos
                .Include(p => p.IdCategoriaNavigation)
                .Include(p => p.Receta)
                    .ThenInclude(r => r.IdInsumoNavigation)
                        .ThenInclude(i => i.IdUnidadNavigation)
                .Where(p => p.Receta.Any())
                .ToListAsync();

            return View(platosConRecetas);
        }

        public async Task<IActionResult> Crear()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            ViewBag.Platos = new SelectList(await _contexto.Platos.Where(p => p.Estado == true).ToListAsync(), "IdPlato", "Nombre");

            var insumos = await _contexto.Insumos
                .Include(i => i.IdUnidadNavigation)
                .Select(i => new
                {
                    i.IdInsumo,
                    i.Nombre,
                    Unidad = i.IdUnidadNavigation.Abreviatura
                })
                .ToListAsync();

            ViewBag.Insumos = insumos;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(RecetaDTO recetaDto)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            // Deserialize details
            List<DetalleRecetaDTO> detalles = new List<DetalleRecetaDTO>();
            try
            {
                if (!string.IsNullOrEmpty(recetaDto.DetallesJson))
                {
                    // Use specific PropertyNameCaseInsensitive options
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    detalles = JsonSerializer.Deserialize<List<DetalleRecetaDTO>>(recetaDto.DetallesJson, options);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al procesar los detalles de la receta: " + ex.Message;
                ViewBag.Platos = new SelectList(await _contexto.Platos.Where(p => p.Estado == true).ToListAsync(), "IdPlato", "Nombre", recetaDto.IdPlato);
                ViewBag.Insumos = new SelectList(await _contexto.Insumos.ToListAsync(), "IdInsumo", "Nombre");
                return View();
            }

            if (detalles == null || detalles.Count == 0)
            {
                TempData["Error"] = "Debe agregar al menos un ingrediente";
                ViewBag.Platos = new SelectList(await _contexto.Platos.Where(p => p.Estado == true).ToListAsync(), "IdPlato", "Nombre", recetaDto.IdPlato);
                ViewBag.Insumos = new SelectList(await _contexto.Insumos.ToListAsync(), "IdInsumo", "Nombre");
                return View();
            }

            var plato = await _contexto.Platos.FindAsync(recetaDto.IdPlato);
            if (plato == null || plato.Estado == false)
            {
                TempData["Error"] = "El plato seleccionado no existe o está inactivo";
                ViewBag.Platos = new SelectList(await _contexto.Platos.Where(p => p.Estado == true).ToListAsync(), "IdPlato", "Nombre", recetaDto.IdPlato);
                ViewBag.Insumos = new SelectList(await _contexto.Insumos.ToListAsync(), "IdInsumo", "Nombre");
                return View();
            }

            using (var transaction = await _contexto.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var det in detalles)
                    {
                        var insumo = await _contexto.Insumos.FindAsync(det.IdInsumo);
                        if (insumo == null) continue;

                        var existente = await _contexto.Receta
                            .FirstOrDefaultAsync(r => r.IdPlato == recetaDto.IdPlato && r.IdInsumo == det.IdInsumo);

                        if (existente != null)
                        {
                            // Update quantity if exists
                            existente.Cantidad = det.Cantidad;
                            _contexto.Receta.Update(existente);
                        }
                        else
                        {
                            // Add new
                            var nuevo = new Recetum
                            {
                                IdPlato = recetaDto.IdPlato,
                                IdInsumo = det.IdInsumo,
                                Cantidad = det.Cantidad
                            };
                            await _contexto.Receta.AddAsync(nuevo);
                        }
                    }

                    await _contexto.SaveChangesAsync();
                    await transaction.CommitAsync();
                    TempData["Mensaje"] = "Ingredientes agregados/actualizados correctamente";
                    return RedirectToAction(nameof(Listar));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = "Error al guardar la receta: " + ex.Message;
                    ViewBag.Platos = new SelectList(await _contexto.Platos.Where(p => p.Estado == true).ToListAsync(), "IdPlato", "Nombre", recetaDto.IdPlato);
                    var insumos = await _contexto.Insumos
                        .Include(i => i.IdUnidadNavigation)
                        .Select(i => new { i.IdInsumo, i.Nombre, Unidad = i.IdUnidadNavigation.Abreviatura })
                        .ToListAsync();
                    ViewBag.Insumos = insumos;
                    return View();
                }
            }
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var receta = await _contexto.Receta
                .Include(r => r.IdInsumoNavigation)
                    .ThenInclude(i => i.IdUnidadNavigation)
                .FirstOrDefaultAsync(r => r.IdReceta == id);
            if (receta == null) return NotFound();

            // Cargar todos los ingredientes del mismo plato para mostrarlos
            var ingredientesDelPlato = await _contexto.Receta
                .Where(r => r.IdPlato == receta.IdPlato)
                .Include(r => r.IdInsumoNavigation)
                    .ThenInclude(i => i.IdUnidadNavigation)
                .ToListAsync();

            ViewBag.NombrePlato = (await _contexto.Platos.FindAsync(receta.IdPlato))?.Nombre ?? "";
            ViewBag.IngredientesDelPlato = ingredientesDelPlato;
            ViewBag.Platos = new SelectList(await _contexto.Platos.Where(p => p.Estado == true).ToListAsync(), "IdPlato", "Nombre", receta.IdPlato);
            var insumos = await _contexto.Insumos
                .Include(i => i.IdUnidadNavigation)
                .Select(i => new { i.IdInsumo, i.Nombre, Unidad = i.IdUnidadNavigation.Abreviatura })
                .ToListAsync();

            ViewBag.Insumos = insumos;
            ViewBag.IdInsumoActual = receta.IdInsumo;
            return View(receta);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdReceta,IdPlato,IdInsumo,Cantidad")] Recetum receta)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id != receta.IdReceta) return NotFound();

            if (receta.Cantidad <= 0)
            {
                TempData["Error"] = "La cantidad requerida debe ser mayor a 0";
                ViewBag.IngredientesDelPlato = await _contexto.Receta
                    .Where(r => r.IdPlato == receta.IdPlato)
                    .Include(r => r.IdInsumoNavigation)
                        .ThenInclude(i => i.IdUnidadNavigation)
                    .ToListAsync();
                ViewBag.NombrePlato = (await _contexto.Platos.FindAsync(receta.IdPlato))?.Nombre ?? "";
                ViewBag.Platos = new SelectList(await _contexto.Platos.Where(p => p.Estado == true).ToListAsync(), "IdPlato", "Nombre", receta.IdPlato);
                ViewBag.Insumos = new SelectList(await _contexto.Insumos.ToListAsync(), "IdInsumo", "Nombre", receta.IdInsumo);
                return View(receta);
            }

            var recetaDuplicada = await _contexto.Receta
                .FirstOrDefaultAsync(r => r.IdPlato == receta.IdPlato && r.IdInsumo == receta.IdInsumo && r.IdReceta != receta.IdReceta);
            if (recetaDuplicada != null)
            {
                TempData["Error"] = "Este insumo ya está en la receta del plato seleccionado";
                ViewBag.IngredientesDelPlato = await _contexto.Receta
                    .Where(r => r.IdPlato == receta.IdPlato)
                    .Include(r => r.IdInsumoNavigation)
                        .ThenInclude(i => i.IdUnidadNavigation)
                    .ToListAsync();
                ViewBag.NombrePlato = (await _contexto.Platos.FindAsync(receta.IdPlato))?.Nombre ?? "";
                ViewBag.Platos = new SelectList(await _contexto.Platos.Where(p => p.Estado == true).ToListAsync(), "IdPlato", "Nombre", receta.IdPlato);
                ViewBag.Insumos = new SelectList(await _contexto.Insumos.ToListAsync(), "IdInsumo", "Nombre", receta.IdInsumo);
                return View(receta);
            }

            _contexto.Update(receta);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Receta actualizada";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var receta = await _contexto.Receta
                .Include(r => r.IdPlatoNavigation)
                .Include(r => r.IdInsumoNavigation)
                    .ThenInclude(i => i.IdUnidadNavigation)
                .FirstOrDefaultAsync(m => m.IdReceta == id);
            return receta == null ? NotFound() : View(receta);
        }

        [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var receta = await _contexto.Receta.FindAsync(id);
            if (receta != null)
            {
                try
                {
                    _contexto.Receta.Remove(receta);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Ingrediente eliminado de la receta";
                }
                catch (Exception)
                {
                    TempData["Error"] = "No se puede eliminar este ingrediente porque está en uso.";
                }
            }
            return RedirectToAction(nameof(Listar));
        }
    }
}
