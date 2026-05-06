using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class ProveedorController : Controller
    {
        private readonly ContextoRestaurante _contexto;
        public ProveedorController(ContextoRestaurante contexto) => _contexto = contexto;
        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            return View(await _contexto.Proveedors.ToListAsync());
        }

        public IActionResult Crear()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("NombreEmpresa,ContactoNombre,Telefono,Email,Estado,NitCarnet")] Proveedor proveedor)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            if (string.IsNullOrWhiteSpace(proveedor.NombreEmpresa))
            {
                TempData["Error"] = "El nombre de la empresa es obligatorio";
                return View(proveedor);
            }

            var proveedorExistente = await _contexto.Proveedors
                .FirstOrDefaultAsync(p => p.NombreEmpresa.ToLower() == proveedor.NombreEmpresa.ToLower());
            if (proveedorExistente != null)
            {
                TempData["Error"] = $"Ya existe un proveedor con el nombre '{proveedor.NombreEmpresa}'";
                return View(proveedor);
            }

            if (!string.IsNullOrWhiteSpace(proveedor.Email))
            {
                if (!proveedor.Email.Contains("@"))
                {
                    TempData["Error"] = "El formato del email no es válido";
                    return View(proveedor);
                }
            }

            _contexto.Add(proveedor);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Proveedor creado exitosamente";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var proveedor = await _contexto.Proveedors.FindAsync(id);
            return proveedor == null ? NotFound() : View(proveedor);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdProveedor,NombreEmpresa,ContactoNombre,Telefono,Email,Estado,NitCarnet")] Proveedor proveedor)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id != proveedor.IdProveedor) return NotFound();

            if (string.IsNullOrWhiteSpace(proveedor.NombreEmpresa))
            {
                TempData["Error"] = "El nombre de la empresa es obligatorio";
                return View(proveedor);
            }

            var proveedorExistente = await _contexto.Proveedors
                .FirstOrDefaultAsync(p => p.NombreEmpresa.ToLower() == proveedor.NombreEmpresa.ToLower() && p.IdProveedor != proveedor.IdProveedor);
            if (proveedorExistente != null)
            {
                TempData["Error"] = $"Ya existe otro proveedor con el nombre '{proveedor.NombreEmpresa}'";
                return View(proveedor);
            }

            if (!string.IsNullOrWhiteSpace(proveedor.Email) && !proveedor.Email.Contains("@"))
            {
                TempData["Error"] = "El formato del email no es válido";
                return View(proveedor);
            }

            _contexto.Update(proveedor);
            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Proveedor actualizado";
            return RedirectToAction(nameof(Listar));
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var proveedor = await _contexto.Proveedors.FirstOrDefaultAsync(m => m.IdProveedor == id);
            return proveedor == null ? NotFound() : View(proveedor);
        }

        [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var proveedor = await _contexto.Proveedors.FindAsync(id);
            if (proveedor != null)
            {

                var tieneCompras = await _contexto.Compras.AnyAsync(c => c.IdProveedor == id);

                if (tieneCompras)
                {
                    TempData["Error"] = "No se puede eliminar el proveedor porque tiene compras registradas. Puede desactivarlo en su lugar.";
                    return RedirectToAction(nameof(Listar));
                }

                try
                {
                    _contexto.Proveedors.Remove(proveedor);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Proveedor eliminado";
                }
                catch (Exception)
                {
                    TempData["Error"] = "No se puede eliminar este proveedor porque tiene compras registradas.";
                }
            }
            return RedirectToAction(nameof(Listar));
        }
    }
}
