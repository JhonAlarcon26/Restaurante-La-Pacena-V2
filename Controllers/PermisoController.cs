using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using la_paceña_v2.Datos;
using la_paceña_v2.Models;

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
        public async Task<IActionResult> Crear([Bind("NombreTecnico,Descripcion")] Permiso permiso)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            
            if (string.IsNullOrWhiteSpace(permiso.NombreTecnico))
            {
                TempData["Error"] = "El nombre técnico es obligatorio";
                return View(permiso);
            }
            
            var nombreExistente = await _contexto.Permisos
                .FirstOrDefaultAsync(p => p.NombreTecnico.ToLower() == permiso.NombreTecnico.ToLower());
            if (nombreExistente != null)
            {
                TempData["Error"] = $"Ya existe un permiso con el nombre '{permiso.NombreTecnico}'";
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
        public async Task<IActionResult> Editar(int id, [Bind("IdPermisos,NombreTecnico,Descripcion")] Permiso permiso)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id != permiso.IdPermisos) return NotFound();
            
            if (string.IsNullOrWhiteSpace(permiso.NombreTecnico))
            {
                TempData["Error"] = "El nombre técnico es obligatorio";
                return View(permiso);
            }
            
            var nombreExistente = await _contexto.Permisos
                .FirstOrDefaultAsync(p => p.NombreTecnico.ToLower() == permiso.NombreTecnico.ToLower() && p.IdPermisos != permiso.IdPermisos);
            if (nombreExistente != null)
            {
                TempData["Error"] = $"Ya existe otro permiso con el nombre '{permiso.NombreTecnico}'";
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
            var permiso = await _contexto.Permisos.FirstOrDefaultAsync(m => m.IdPermisos == id);
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
