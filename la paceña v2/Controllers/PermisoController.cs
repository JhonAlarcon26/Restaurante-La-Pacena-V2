using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class PermisoController : Controller
    {
        private readonly ContextoRestaurante _contexto;
        public PermisoController(ContextoRestaurante contexto) => _contexto = contexto;
        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            return View(await _contexto.Permisos.ToListAsync());
        }

        public IActionResult Crear()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Nombre,Descripcion")] Permiso permiso)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            if (string.IsNullOrWhiteSpace(permiso.Nombre))
            {
                TempData["Error"] = "El nombre técnico es obligatorio";
                return View(permiso);
            }

            var nombreExistente = await _contexto.Permisos
                .FirstOrDefaultAsync(p => p.Nombre.ToLower() == permiso.Nombre.ToLower());
            if (nombreExistente != null)
            {
                TempData["Error"] = $"Ya existe un permiso con el nombre '{permiso.Nombre}'";
                return View(permiso);
            }

            _contexto.Add(permiso);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Permiso creado exitosamente";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var permiso = await _contexto.Permisos.FindAsync(id);
            return permiso == null ? NotFound() : View(permiso);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdPermiso,Nombre,Descripcion")] Permiso permiso)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id != permiso.IdPermiso) return NotFound();

            if (string.IsNullOrWhiteSpace(permiso.Nombre))
            {
                TempData["Error"] = "El nombre técnico es obligatorio";
                return View(permiso);
            }

            var nombreExistente = await _contexto.Permisos
                .FirstOrDefaultAsync(p => p.Nombre.ToLower() == permiso.Nombre.ToLower() && p.IdPermiso != permiso.IdPermiso);
            if (nombreExistente != null)
            {
                TempData["Error"] = $"Ya existe otro permiso con el nombre '{permiso.Nombre}'";
                return View(permiso);
            }

            _contexto.Update(permiso);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Permiso actualizado";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var permiso = await _contexto.Permisos.FirstOrDefaultAsync(m => m.IdPermiso == id);
            return permiso == null ? NotFound() : View(permiso);
        }

        [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            try
            {
                var permiso = await _contexto.Permisos.FindAsync(id);
                if (permiso != null)
                {
                    _contexto.Permisos.Remove(permiso);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Permiso eliminado";
                }
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "No se puede eliminar este permiso porque está asignado a uno o más roles. Desvincúlelo primero.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error inesperado al intentar eliminar el permiso.";
            }
            return RedirectToAction(nameof(Listar));
        }
    }
}
