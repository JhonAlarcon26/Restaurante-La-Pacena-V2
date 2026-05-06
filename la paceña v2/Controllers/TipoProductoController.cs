using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class TipoProductoController : Controller
    {
        private readonly ContextoRestaurante _contexto;
        public TipoProductoController(ContextoRestaurante contexto) => _contexto = contexto;
        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            return View(await _contexto.TipoProductos.ToListAsync());
        }

        public IActionResult Crear()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Nombre,Descripcion")] TipoProducto tipoProducto)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            if (ModelState.IsValid)
            {
                tipoProducto.Estado = true;
                _contexto.Add(tipoProducto);
                await _contexto.SaveChangesAsync();
                TempData["Mensaje"] = "Tipo de producto creado exitosamente";
                return RedirectToAction(nameof(Listar));
            }
            return View(tipoProducto);
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();

            var tipoProducto = await _contexto.TipoProductos.FindAsync(id);
            if (tipoProducto == null) return NotFound();
            return View(tipoProducto);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdTipoProducto,Nombre,Descripcion,Estado")] TipoProducto tipoProducto)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id != tipoProducto.IdTipoProducto) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _contexto.Update(tipoProducto);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Tipo de producto actualizado";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoProductoExists(tipoProducto.IdTipoProducto)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Listar));
            }
            return View(tipoProducto);
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();

            var tipoProducto = await _contexto.TipoProductos
                .FirstOrDefaultAsync(m => m.IdTipoProducto == id);
            if (tipoProducto == null) return NotFound();

            return View(tipoProducto);
        }

        [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var tipoProducto = await _contexto.TipoProductos.FindAsync(id);
            if (tipoProducto != null)
            {
                try
                {
                    _contexto.TipoProductos.Remove(tipoProducto);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Tipo de producto eliminado";
                }
                catch (DbUpdateException)
                {
                    TempData["Error"] = "No se puede eliminar este tipo porque tiene productos asociados.";
                }
            }
            return RedirectToAction(nameof(Listar));
        }

        private bool TipoProductoExists(int id)
        {
            return _contexto.TipoProductos.Any(e => e.IdTipoProducto == id);
        }
    }
}
