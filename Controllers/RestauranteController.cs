using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class RestauranteController : Controller
    {
        private readonly ContextoRestaurante _contexto;
        public RestauranteController(ContextoRestaurante contexto) => _contexto = contexto;
        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            return View(await _contexto.Restaurantes.ToListAsync());
        }

        public IActionResult Crear()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Nombre,Direccion,Ciudad,Telefono,Estado")] Restaurante Restaurante)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            if (string.IsNullOrWhiteSpace(Restaurante.Nombre))
            {
                TempData["Error"] = "El nombre de la Restaurante es obligatorio";
                return View(Restaurante);
            }

            if (string.IsNullOrWhiteSpace(Restaurante.Ciudad))
            {
                TempData["Error"] = "La ciudad es obligatoria";
                return View(Restaurante);
            }

            var nombreExistente = await _contexto.Restaurantes
                .FirstOrDefaultAsync(s => s.Nombre.ToLower() == Restaurante.Nombre.ToLower());
            if (nombreExistente != null)
            {
                TempData["Error"] = $"Ya existe una Restaurante con el nombre '{Restaurante.Nombre}'";
                return View(Restaurante);
            }

            _contexto.Add(Restaurante);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Restaurante creada exitosamente";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var Restaurante = await _contexto.Restaurantes.FindAsync(id);
            return Restaurante == null ? NotFound() : View(Restaurante);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdRestaurante,Nombre,Direccion,Ciudad,Telefono,Estado")] Restaurante Restaurante)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id != Restaurante.IdRestaurante) return NotFound();

            if (string.IsNullOrWhiteSpace(Restaurante.Nombre))
            {
                TempData["Error"] = "El nombre de la Restaurante es obligatorio";
                return View(Restaurante);
            }

            if (string.IsNullOrWhiteSpace(Restaurante.Ciudad))
            {
                TempData["Error"] = "La ciudad es obligatoria";
                return View(Restaurante);
            }

            var nombreExistente = await _contexto.Restaurantes
                .FirstOrDefaultAsync(s => s.Nombre.ToLower() == Restaurante.Nombre.ToLower() && s.IdRestaurante != Restaurante.IdRestaurante);
            if (nombreExistente != null)
            {
                TempData["Error"] = $"Ya existe otra Restaurante con el nombre '{Restaurante.Nombre}'";
                return View(Restaurante);
            }

            _contexto.Update(Restaurante);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Restaurante actualizada";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var Restaurante = await _contexto.Restaurantes.FirstOrDefaultAsync(m => m.IdRestaurante == id);
            return Restaurante == null ? NotFound() : View(Restaurante);
        }

        [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var Restaurante = await _contexto.Restaurantes.FindAsync(id);
            if (Restaurante != null)
            {
                try
                {
                    _contexto.Restaurantes.Remove(Restaurante);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Restaurante eliminada";
                }
                catch (Exception)
                {
                    TempData["Error"] = "No se puede eliminar esta Restaurante porque tiene registros asociados.";
                }
            }
            return RedirectToAction(nameof(Listar));
        }
    }
}
