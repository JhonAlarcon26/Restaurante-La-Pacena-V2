using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class RolPermisoController : Controller
    {
        private readonly ContextoRestaurante _contexto;
        public RolPermisoController(ContextoRestaurante contexto) => _contexto = contexto;
        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public IActionResult Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            return RedirectToAction(nameof(Asignar));
        }

        public async Task<IActionResult> Asignar(int? idRol)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var roles = await _contexto.Rols.Where(r => r.Estado == true).ToListAsync();
            var permisos = await _contexto.Permisos.OrderBy(p => p.Nombre).ToListAsync();

            ViewBag.Roles = roles;
            ViewBag.Permisos = permisos;

            if (idRol.HasValue)
            {
                var permisosAsignados = await _contexto.RolPermisos
                    .Where(rp => rp.IdRol == idRol.Value)
                    .Select(rp => rp.IdPermiso)
                    .ToListAsync();
                ViewBag.PermisosAsignados = permisosAsignados;
                ViewBag.IdRolSeleccionado = idRol.Value;
            }
            else
            {
                ViewBag.PermisosAsignados = new System.Collections.Generic.List<int>();
                ViewBag.IdRolSeleccionado = 0;
            }

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar(int idRol, int[] permisosSeleccionados)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            try
            {
                var permisosExistentes = await _contexto.RolPermisos
                    .Where(rp => rp.IdRol == idRol)
                    .ToListAsync();

                _contexto.RolPermisos.RemoveRange(permisosExistentes);

                if (permisosSeleccionados != null)
                {
                    foreach (var idPermiso in permisosSeleccionados)
                    {
                        var rolPermiso = new RolPermiso
                        {
                            IdRol = idRol,
                            IdPermiso = idPermiso
                        };
                        _contexto.RolPermisos.Add(rolPermiso);
                    }
                }

                await _contexto.SaveChangesAsync();
                TempData["Mensaje"] = "Permisos actualizados correctamente";
            }
            catch (Exception)
            {
                TempData["Error"] = "Error al actualizar los permisos. Intente nuevamente.";
            }

            return RedirectToAction(nameof(Asignar), new { idRol });
        }
    }
}
