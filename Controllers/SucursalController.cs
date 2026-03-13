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
    public class SucursalController : Controller
    {
        private readonly ContextoRestaurante _contexto;
        public SucursalController(ContextoRestaurante contexto) => _contexto = contexto;
        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            return View(await _contexto.Sucursals.ToListAsync());
        }

        public IActionResult Crear()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Nombre,Direccion,Ciudad,Telefono,Estado")] Sucursal sucursal)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            
            if (string.IsNullOrWhiteSpace(sucursal.Nombre))
            {
                TempData["Error"] = "El nombre de la sucursal es obligatorio";
                return View(sucursal);
            }
            
            if (string.IsNullOrWhiteSpace(sucursal.Ciudad))
            {
                TempData["Error"] = "La ciudad es obligatoria";
                return View(sucursal);
            }
            
            var nombreExistente = await _contexto.Sucursals
                .FirstOrDefaultAsync(s => s.Nombre.ToLower() == sucursal.Nombre.ToLower());
            if (nombreExistente != null)
            {
                TempData["Error"] = $"Ya existe una sucursal con el nombre '{sucursal.Nombre}'";
                return View(sucursal);
            }
            
            _contexto.Add(sucursal);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Sucursal creada exitosamente";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var sucursal = await _contexto.Sucursals.FindAsync(id);
            return sucursal == null ? NotFound() : View(sucursal);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdSucursal,Nombre,Direccion,Ciudad,Telefono,Estado")] Sucursal sucursal)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id != sucursal.IdSucursal) return NotFound();
            
            if (string.IsNullOrWhiteSpace(sucursal.Nombre))
            {
                TempData["Error"] = "El nombre de la sucursal es obligatorio";
                return View(sucursal);
            }
            
            if (string.IsNullOrWhiteSpace(sucursal.Ciudad))
            {
                TempData["Error"] = "La ciudad es obligatoria";
                return View(sucursal);
            }
            
            var nombreExistente = await _contexto.Sucursals
                .FirstOrDefaultAsync(s => s.Nombre.ToLower() == sucursal.Nombre.ToLower() && s.IdSucursal != sucursal.IdSucursal);
            if (nombreExistente != null)
            {
                TempData["Error"] = $"Ya existe otra sucursal con el nombre '{sucursal.Nombre}'";
                return View(sucursal);
            }
            
            _contexto.Update(sucursal);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Sucursal actualizada";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var sucursal = await _contexto.Sucursals.FirstOrDefaultAsync(m => m.IdSucursal == id);
            return sucursal == null ? NotFound() : View(sucursal);
        }

        [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var sucursal = await _contexto.Sucursals.FindAsync(id);
            if (sucursal != null)
            {
                try
                {
                    _contexto.Sucursals.Remove(sucursal);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Sucursal eliminada";
                }
                catch (Exception)
                {
                    TempData["Error"] = "No se puede eliminar esta sucursal porque tiene registros asociados.";
                }
            }
            return RedirectToAction(nameof(Listar));
        }
    }
}
