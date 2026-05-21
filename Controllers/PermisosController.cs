using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Controllers
{
    public class PermisosController : Controller
    {
        private readonly ContextoRestaurante _contexto;
        public PermisosController(ContextoRestaurante contexto) => _contexto = contexto;
        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var roles = await _contexto.Rols
                .Include(r => r.RolYPermisos)
                .ThenInclude(rp => rp.IdPermisoNavigation)
                .ToListAsync();
            return View(roles);
        }

        public async Task<IActionResult> Asignar(int? idRol)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (idRol == null) return NotFound();

            var rol = await _contexto.Rols
                .Include(r => r.RolYPermisos)
                .ThenInclude(rp => rp.IdPermisoNavigation)
                .FirstOrDefaultAsync(r => r.IdRol == idRol);
                
            if (rol == null) return NotFound();

            ViewBag.Rol = rol;
            ViewBag.PermisosAsignados = rol.RolYPermisos.Select(rp => rp.IdPermisoNavigation.Nombre).ToList();
            ViewBag.CatalogoPermisos = ObtenerCatalogoPermisos();

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarAsignacion(int idRol, string[] permisosSeleccionados)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            var asignacionesActuales = await _contexto.RolYPermisos.Where(p => p.IdRol == idRol).ToListAsync();
            _contexto.RolYPermisos.RemoveRange(asignacionesActuales);

            if (permisosSeleccionados != null)
            {
                foreach (var nombrePermiso in permisosSeleccionados)
                {
                    var permiso = await _contexto.Permisos.FirstOrDefaultAsync(p => p.Nombre == nombrePermiso);
                    if (permiso == null)
                    {
                        permiso = new Permiso { Nombre = nombrePermiso, Descripcion = "Permiso automático", Estado = true };
                        _contexto.Permisos.Add(permiso);
                        await _contexto.SaveChangesAsync();
                    }

                    _contexto.RolYPermisos.Add(new RolYPermiso
                    {
                        IdRol = idRol,
                        IdPermiso = permiso.IdPermiso,
                        Estado = true
                    });
                }
            }

            await _contexto.SaveChangesAsync();
            TempData["Mensaje"] = "Permisos actualizados correctamente para el rol seleccionado";
            return RedirectToAction(nameof(Listar));
        }

        private Dictionary<string, List<string>> ObtenerCatalogoPermisos()
        {
            return new Dictionary<string, List<string>>
            {
                { "Administración", new List<string> { "VerUsuarios", "CrearUsuarios", "EditarUsuarios", "EliminarUsuarios", "VerRoles", "CrearRoles", "EditarRoles", "EliminarRoles", "AsignarPermisos", "VerPermisos", "CrearPermisos", "EditarPermisos", "EliminarPermisos" } },
                { "Configuración", new List<string> { "VerTipoProducto", "CrearTipoProducto", "EditarTipoProducto", "EliminarTipoProducto", "VerUnidadMedida", "CrearUnidadMedida", "EditarUnidadMedida", "EliminarUnidadMedida", "VerTipoPago", "CrearTipoPago", "EditarTipoPago", "EliminarTipoPago", "VerCategorias", "CrearCategorias", "EditarCategorias", "EliminarCategorias", "VerSucursales", "CrearSucursales", "EditarSucursales", "EliminarSucursales" } },
                { "Compra", new List<string> { "VerProveedores", "CrearProveedores", "EditarProveedores", "EliminarProveedores", "VerInsumos", "CrearInsumos", "EditarInsumos", "EliminarInsumos", "VerProductos", "CrearProductos", "EditarProductos", "EliminarProductos", "VerCompraProducto", "CrearCompraProducto", "EliminarCompraProducto", "VerCompraInsumo", "CrearCompraInsumo", "EliminarCompraInsumo" } },
                { "Venta", new List<string> { "VerClientes", "CrearClientes", "EditarClientes", "EliminarClientes", "VerPlatos", "CrearPlatos", "EditarPlatos", "EliminarPlatos", "VerAbastecimientos", "CrearAbastecimientos", "EliminarAbastecimientos", "VerVentas", "CrearVentas", "EliminarVentas" } },
                { "Stock", new List<string> { "VerStock", "CrearStock", "EditarStock", "EliminarStock", "VerStockPlatos", "EditarStockPlatos", "EliminarStockPlatos", "VerKardex" } },
                { "Reporte", new List<string> { "VerReporteCompraInsumo", "VerReporteCompraProducto", "VerReporteVentas", "VerReporteStock" } }
            };
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

            permiso.Estado = true;
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
            if (permiso == null) return NotFound();
            return View(permiso);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdPermiso,Nombre,Descripcion,Estado")] Permiso permiso)
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
