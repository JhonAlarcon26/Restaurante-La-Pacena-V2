using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class RolController : Controller
    {
        private readonly ContextoRestaurante _contexto;
        public RolController(ContextoRestaurante contexto) => _contexto = contexto;
        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            return View(await _contexto.Rols.ToListAsync());
        }

        public IActionResult Crear()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Nombre,Descripcion,Estado")] Rol rol)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            if (string.IsNullOrWhiteSpace(rol.Nombre))
            {
                TempData["Error"] = "El nombre del rol es obligatorio";
                return View(rol);
            }

            if (string.IsNullOrWhiteSpace(rol.Descripcion))
            {
                TempData["Error"] = "La descripción es obligatoria";
                return View(rol);
            }

            var nombreExistente = await _contexto.Rols
                .FirstOrDefaultAsync(r => r.Nombre.ToLower() == rol.Nombre.ToLower());
            if (nombreExistente != null)
            {
                TempData["Error"] = $"Ya existe un rol con el nombre '{rol.Nombre}'";
                return View(rol);
            }

            _contexto.Add(rol);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Rol creado exitosamente";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var rol = await _contexto.Rols.FindAsync(id);
            return rol == null ? NotFound() : View(rol);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdRol,Nombre,Descripcion,Estado")] Rol rol)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id != rol.IdRol) return NotFound();

            if (string.IsNullOrWhiteSpace(rol.Nombre))
            {
                TempData["Error"] = "El nombre del rol es obligatorio";
                return View(rol);
            }

            if (string.IsNullOrWhiteSpace(rol.Descripcion))
            {
                TempData["Error"] = "La descripción es obligatoria";
                return View(rol);
            }

            var nombreExistente = await _contexto.Rols
                .FirstOrDefaultAsync(r => r.Nombre.ToLower() == rol.Nombre.ToLower() && r.IdRol != rol.IdRol);
            if (nombreExistente != null)
            {
                TempData["Error"] = $"Ya existe otro rol con el nombre '{rol.Nombre}'";
                return View(rol);
            }

            _contexto.Update(rol);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Rol actualizado";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var rol = await _contexto.Rols.FirstOrDefaultAsync(m => m.IdRol == id);
            return rol == null ? NotFound() : View(rol);
        }

        [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var rol = await _contexto.Rols.FindAsync(id);
            if (rol != null)
            {

                var usuariosConRol = await _contexto.Usuarios.AnyAsync(u => u.IdRol == id);
                if (usuariosConRol)
                {
                    TempData["Error"] = "No se puede eliminar el rol porque hay usuarios asignados a él. Reasigne los usuarios antes de eliminar.";
                    return RedirectToAction(nameof(Listar));
                }


                var permisosAsociados = await _contexto.RolPermisos.Where(rp => rp.IdRol == id).ToListAsync();
                if (permisosAsociados.Any())
                {
                    _contexto.RolPermisos.RemoveRange(permisosAsociados);
                }

                try
                {
                    _contexto.Rols.Remove(rol);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Rol eliminado";
                }
                catch (Exception)
                {
                    TempData["Error"] = "No se puede eliminar este rol porque tiene usuarios o permisos asignados.";
                }
            }
            return RedirectToAction(nameof(Listar));
        }
    }
}
