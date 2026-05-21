using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class InsumoController : Controller
    {
        private readonly ContextoRestaurante _contexto;
        public InsumoController(ContextoRestaurante contexto) => _contexto = contexto;
        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var insumos = await _contexto.Insumos
                .Include(i => i.IdUnidadNavigation)
                .Include(i => i.Stocks)
                .ToListAsync();
            return View(insumos);
        }

        public IActionResult Crear()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            ViewData["Unidades"] = new SelectList(_contexto.UnidadDeMedida, "IdUnidad", "Nombre");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Nombre,IdUnidad,Estado")] Insumo insumo)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            if (string.IsNullOrWhiteSpace(insumo.Nombre))
            {
                TempData["Error"] = "El nombre del insumo es obligatorio";
                ViewData["Unidades"] = new SelectList(_contexto.UnidadDeMedida, "IdUnidad", "Nombre", insumo.IdUnidad);
                return View(insumo);
            }

            var insumoExistente = await _contexto.Insumos
                .FirstOrDefaultAsync(i => i.Nombre.ToLower() == insumo.Nombre.ToLower());
            if (insumoExistente != null)
            {
                TempData["Error"] = $"Ya existe un insumo con el nombre '{insumo.Nombre}'";
                ViewData["Unidades"] = new SelectList(_contexto.UnidadDeMedida, "IdUnidad", "Nombre", insumo.IdUnidad);
                return View(insumo);
            }



            var unidad = await _contexto.UnidadDeMedida.FindAsync(insumo.IdUnidad);
            if (unidad == null)
            {
                TempData["Error"] = "La unidad de medida seleccionada no existe";
                ViewData["Unidades"] = new SelectList(_contexto.UnidadDeMedida, "IdUnidad", "Nombre", insumo.IdUnidad);
                return View(insumo);
            }

            _contexto.Add(insumo);
            await _contexto.SaveChangesAsync();


            TempData["Mensaje"] = "Insumo creado exitosamente";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var insumo = await _contexto.Insumos
                .Include(i => i.Stocks)
                .FirstOrDefaultAsync(i => i.IdInsumo == id);

            if (insumo == null) return NotFound();

            ViewData["Unidades"] = new SelectList(_contexto.UnidadDeMedida, "IdUnidad", "Nombre", insumo.IdUnidad);
            return View(insumo);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdInsumo,Nombre,IdUnidad,Estado")] Insumo insumo)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id != insumo.IdInsumo) return NotFound();

            if (string.IsNullOrWhiteSpace(insumo.Nombre))
            {
                TempData["Error"] = "El nombre del insumo es obligatorio";
                ViewData["Unidades"] = new SelectList(_contexto.UnidadDeMedida, "IdUnidad", "Nombre", insumo.IdUnidad);
                return View(insumo);
            }

            var insumoExistente = await _contexto.Insumos
                .FirstOrDefaultAsync(i => i.Nombre.ToLower() == insumo.Nombre.ToLower() && i.IdInsumo != insumo.IdInsumo);
            if (insumoExistente != null)
            {
                TempData["Error"] = $"Ya existe otro insumo con el nombre '{insumo.Nombre}'";
                ViewData["Unidades"] = new SelectList(_contexto.UnidadDeMedida, "IdUnidad", "Nombre", insumo.IdUnidad);
                return View(insumo);
            }



            _contexto.Update(insumo);

            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Insumo actualizado";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var insumo = await _contexto.Insumos
                .Include(i => i.IdUnidadNavigation)
                .FirstOrDefaultAsync(m => m.IdInsumo == id);
            return insumo == null ? NotFound() : View(insumo);
        }

        [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var insumo = await _contexto.Insumos.FindAsync(id);
            if (insumo == null) return RedirectToAction(nameof(Listar));

            var tieneCompras = await _contexto.DetalleCompras.AnyAsync(dc => dc.IdInsumo == id);
            if (tieneCompras)
            {
                TempData["Error"] = "No se puede eliminar el insumo porque tiene compras registradas. Puede desactivarlo en su lugar.";
                return RedirectToAction(nameof(Listar));
            }

            using var transaccion = await _contexto.Database.BeginTransactionAsync();
            try
            {
                var stockAsociado = await _contexto.Stocks.FirstOrDefaultAsync(s => s.IdInsumo == id);
                if (stockAsociado != null)
                {
                    _contexto.Stocks.Remove(stockAsociado);
                }

                _contexto.Insumos.Remove(insumo);
                await _contexto.SaveChangesAsync();
                await transaccion.CommitAsync();

                TempData["Mensaje"] = "Insumo eliminado exitosamente junto con su registro de stock.";
            }
            catch (Exception)
            {
                await transaccion.RollbackAsync();
                TempData["Error"] = "No se pudo eliminar el insumo. Verifique que no tenga otros registros asociados.";
            }

            return RedirectToAction(nameof(Listar));
        }
    }
}
