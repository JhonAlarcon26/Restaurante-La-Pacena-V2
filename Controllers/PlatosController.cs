using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class PlatoController : Controller
    {
        private readonly ContextoRestaurante _contexto;
        public PlatoController(ContextoRestaurante contexto) => _contexto = contexto;
        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var platos = await _contexto.Platos
                .Include(p => p.IdCategoriaNavigation)
                .ToListAsync();
            return View(platos);
        }

        public IActionResult Crear()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            ViewData["Categorias"] = new SelectList(_contexto.CategoriaPlatos, "IdCategoria", "Nombre");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Nombre,DescripcionCarta,IdCategoria,Estado")] Plato plato)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            if (string.IsNullOrWhiteSpace(plato.Nombre))
            {
                TempData["Error"] = "El nombre del plato es obligatorio";
                ViewData["Categorias"] = new SelectList(_contexto.CategoriaPlatos, "IdCategoria", "Nombre", plato.IdCategoria);
                return View(plato);
            }

            var platoExistente = await _contexto.Platos
                .FirstOrDefaultAsync(p => p.Nombre.ToLower() == plato.Nombre.ToLower());
            if (platoExistente != null)
            {
                TempData["Error"] = $"Ya existe un plato con el nombre '{plato.Nombre}'";
                ViewData["Categorias"] = new SelectList(_contexto.CategoriaPlatos, "IdCategoria", "Nombre", plato.IdCategoria);
                return View(plato);
            }


            var categoria = await _contexto.CategoriaPlatos.FindAsync(plato.IdCategoria);
            if (categoria == null)
            {
                TempData["Error"] = "La categoría seleccionada no existe";
                ViewData["Categorias"] = new SelectList(_contexto.CategoriaPlatos, "IdCategoria", "Nombre", plato.IdCategoria);
                return View(plato);
            }

            if (categoria.Estado == false)
            {
                TempData["Error"] = "La categoría seleccionada está inactiva";
                ViewData["Categorias"] = new SelectList(_contexto.CategoriaPlatos.Where(c => c.Estado == true), "IdCategoria", "Nombre", plato.IdCategoria);
                return View(plato);
            }

            _contexto.Add(plato);
            await _contexto.SaveChangesAsync();

            var stockPlato = new StockPlato
            {
                IdPlato = plato.IdPlato,
                CantidadActual = 0,
                CantidadMinima = 5,
                UltimaActualizacion = DateTime.Now
            };
            _contexto.StockPlatos.Add(stockPlato);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Plato creado exitosamente";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var plato = await _contexto.Platos.FindAsync(id);
            if (plato == null) return NotFound();
            ViewData["Categorias"] = new SelectList(_contexto.CategoriaPlatos, "IdCategoria", "Nombre", plato.IdCategoria);
            return View(plato);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdPlato,Nombre,DescripcionCarta,IdCategoria,Estado")] Plato plato)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id != plato.IdPlato) return NotFound();

            if (string.IsNullOrWhiteSpace(plato.Nombre))
            {
                TempData["Error"] = "El nombre del plato es obligatorio";
                ViewData["Categorias"] = new SelectList(_contexto.CategoriaPlatos, "IdCategoria", "Nombre", plato.IdCategoria);
                return View(plato);
            }

            var platoExistente = await _contexto.Platos
                .FirstOrDefaultAsync(p => p.Nombre.ToLower() == plato.Nombre.ToLower() && p.IdPlato != plato.IdPlato);
            if (platoExistente != null)
            {
                TempData["Error"] = $"Ya existe otro plato con el nombre '{plato.Nombre}'";
                ViewData["Categorias"] = new SelectList(_contexto.CategoriaPlatos, "IdCategoria", "Nombre", plato.IdCategoria);
                return View(plato);
            }


            var categoria = await _contexto.CategoriaPlatos.FindAsync(plato.IdCategoria);
            if (categoria == null || categoria.Estado == false)
            {
                TempData["Error"] = "La categoría seleccionada no existe o está inactiva";
                ViewData["Categorias"] = new SelectList(_contexto.CategoriaPlatos.Where(c => c.Estado == true), "IdCategoria", "Nombre", plato.IdCategoria);
                return View(plato);
            }

            _contexto.Update(plato);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Plato actualizado";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var plato = await _contexto.Platos
                .Include(p => p.IdCategoriaNavigation)
                .FirstOrDefaultAsync(m => m.IdPlato == id);
            return plato == null ? NotFound() : View(plato);
        }

        [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var plato = await _contexto.Platos.FindAsync(id);
            if (plato == null) return RedirectToAction(nameof(Listar));

            var tieneVentas = await _contexto.DetalleVentas.AnyAsync(dv => dv.IdPlato == id);
            var tieneAbastecimiento = await _contexto.DetalleAbastecimientoPlatos.AnyAsync(r => r.IdPlato == id);

            if (tieneVentas || tieneAbastecimiento)
            {
                TempData["Error"] = "No se puede eliminar el plato porque tiene ventas o abastecimientos registrados. Puede desactivarlo en su lugar.";
                return RedirectToAction(nameof(Listar));
            }

            using var transaccion = await _contexto.Database.BeginTransactionAsync();
            try
            {
                var stockAsociado = await _contexto.StockPlatos.FirstOrDefaultAsync(s => s.IdPlato == id);
                if (stockAsociado != null)
                {
                    _contexto.StockPlatos.Remove(stockAsociado);
                }

                _contexto.Platos.Remove(plato);
                await _contexto.SaveChangesAsync();
                await transaccion.CommitAsync();

                TempData["Mensaje"] = "Plato eliminado exitosamente junto con su registro de stock.";
            }
            catch (Exception)
            {
                await transaccion.RollbackAsync();
                TempData["Error"] = "Error al intentar eliminar el plato. Es posible que tenga otros registros asociados.";
            }

            return RedirectToAction(nameof(Listar));
        }
    }
}
