using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class CategoriaPlatoController : Controller
    {
        private readonly ContextoRestaurante _contexto;

        public CategoriaPlatoController(ContextoRestaurante contexto)
        {
            _contexto = contexto;
        }

        public async Task<IActionResult> Listar()
        {
            return View(await _contexto.CategoriaPlatos.ToListAsync());
        }

        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaPlato = await _contexto.CategoriaPlatos
                .FirstOrDefaultAsync(m => m.IdCategoria == id);
            if (categoriaPlato == null)
            {
                return NotFound();
            }

            return View(categoriaPlato);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("IdCategoria,Nombre,ImpresoraDestino,Estado")] CategoriaPlato categoriaPlato)
        {

            if (string.IsNullOrWhiteSpace(categoriaPlato.Nombre))
            {
                TempData["Error"] = "El nombre de la categoría es obligatorio";
                return View(categoriaPlato);
            }

            var categoriaExistente = await _contexto.CategoriaPlatos
                .FirstOrDefaultAsync(c => c.Nombre.ToLower() == categoriaPlato.Nombre.ToLower());
            if (categoriaExistente != null)
            {
                TempData["Error"] = $"Ya existe una categoría con el nombre '{categoriaPlato.Nombre}'";
                return View(categoriaPlato);
            }

            // Asignar impresora por defecto si no se especificó (aunque el formulario lo envía hidden)
            if (string.IsNullOrEmpty(categoriaPlato.ImpresoraDestino))
            {
                categoriaPlato.ImpresoraDestino = "COCINA";
            }

            _contexto.Add(categoriaPlato);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Categoría creada exitosamente";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaPlato = await _contexto.CategoriaPlatos.FindAsync(id);
            if (categoriaPlato == null)
            {
                return NotFound();
            }
            return View(categoriaPlato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdCategoria,Nombre,ImpresoraDestino,Estado")] CategoriaPlato categoriaPlato)
        {
            if (id != categoriaPlato.IdCategoria)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(categoriaPlato.Nombre))
            {
                TempData["Error"] = "El nombre de la categoría es obligatorio";
                return View(categoriaPlato);
            }

            var categoriaExistente = await _contexto.CategoriaPlatos
                .FirstOrDefaultAsync(c => c.Nombre.ToLower() == categoriaPlato.Nombre.ToLower() && c.IdCategoria != categoriaPlato.IdCategoria);
            if (categoriaExistente != null)
            {
                TempData["Error"] = $"Ya existe otra categoría con el nombre '{categoriaPlato.Nombre}'";
                return View(categoriaPlato);
            }

            _contexto.Update(categoriaPlato);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Categoría actualizada";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaPlato = await _contexto.CategoriaPlatos
                .FirstOrDefaultAsync(m => m.IdCategoria == id);
            if (categoriaPlato == null)
            {
                return NotFound();
            }

            return View(categoriaPlato);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var categoriaPlato = await _contexto.CategoriaPlatos.FindAsync(id);
            if (categoriaPlato != null)
            {

                var tienePlatos = await _contexto.Platos.AnyAsync(p => p.IdCategoria == id);
                if (tienePlatos)
                {
                    TempData["Error"] = "No se puede eliminar la categoría porque tiene platos asignados. Puede desactivarla en su lugar.";
                    return RedirectToAction(nameof(Listar));
                }

                try
                {
                    _contexto.CategoriaPlatos.Remove(categoriaPlato);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Categoría eliminada";
                }
                catch (Exception)
                {
                    TempData["Error"] = "No se puede eliminar esta categoría porque tiene registros asociados.";
                }
            }

            return RedirectToAction(nameof(Listar));
        }

        private bool CategoriaPlatoExiste(int id)
        {
            return _contexto.CategoriaPlatos.Any(e => e.IdCategoria == id);
        }
    }
}
