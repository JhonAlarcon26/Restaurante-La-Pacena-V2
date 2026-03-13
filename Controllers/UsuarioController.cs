using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using la_paceña_v2.Datos;
using la_paceña_v2.Models;

namespace la_paceña_v2.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ContextoRestaurante _contexto;
        public UsuarioController(ContextoRestaurante contexto) => _contexto = contexto;
        private bool VerificarAuth() => HttpContext.Session.GetInt32("IdUsuario") != null;

        public async Task<IActionResult> Listar()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var usuarios = await _contexto.Usuarios
                .Include(u => u.IdRolNavigation)
                .Include(u => u.IdSucursalNavigation)
                .ToListAsync();
            return View(usuarios);
        }

        public IActionResult Crear()
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            ViewData["Roles"] = new SelectList(_contexto.Rols, "IdRol", "Nombre");
            ViewData["Sucursales"] = new SelectList(_contexto.Sucursals, "IdSucursal", "Nombre");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("NombreCompleto,Email,PasswordHash,IdRol,IdSucursal,Estado")] Usuario usuario)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");

            if (!string.IsNullOrEmpty(usuario.NombreCompleto) && !System.Text.RegularExpressions.Regex.IsMatch(usuario.NombreCompleto.Trim(), @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$"))
            {
                ModelState.AddModelError("NombreCompleto", "El nombre solo debe contener letras y espacios");
                ViewData["Roles"] = new SelectList(_contexto.Rols, "IdRol", "Nombre", usuario.IdRol);
                ViewData["Sucursales"] = new SelectList(_contexto.Sucursals, "IdSucursal", "Nombre", usuario.IdSucursal);
                return View(usuario);
            }

            if (string.IsNullOrEmpty(usuario.PasswordHash) || usuario.PasswordHash.Length < 6)
            {
                ModelState.AddModelError("PasswordHash", "La contraseña debe tener al menos 6 caracteres");
                ViewData["Roles"] = new SelectList(_contexto.Rols, "IdRol", "Nombre", usuario.IdRol);
                ViewData["Sucursales"] = new SelectList(_contexto.Sucursals, "IdSucursal", "Nombre", usuario.IdSucursal);
                return View(usuario);
            }

            var emailExiste = await _contexto.Usuarios.AnyAsync(u => u.Email == usuario.Email);
            if (emailExiste)
            {
                ModelState.AddModelError("Email", "Este email ya está registrado");
                ViewData["Roles"] = new SelectList(_contexto.Rols, "IdRol", "Nombre", usuario.IdRol);
                ViewData["Sucursales"] = new SelectList(_contexto.Sucursals, "IdSucursal", "Nombre", usuario.IdSucursal);
                return View(usuario);
            }

            ModelState.Remove("IdRolNavigation");
            ModelState.Remove("IdSucursalNavigation");
            ModelState.Remove("Ventas");
            ModelState.Remove("Compras");

            if (ModelState.IsValid)
            {

                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);

                _contexto.Add(usuario);
                await _contexto.SaveChangesAsync();
                TempData["Mensaje"] = "Usuario creado exitosamente";
                return RedirectToAction(nameof(Listar));
            }
            ViewData["Roles"] = new SelectList(_contexto.Rols, "IdRol", "Nombre", usuario.IdRol);
            ViewData["Sucursales"] = new SelectList(_contexto.Sucursals, "IdSucursal", "Nombre", usuario.IdSucursal);
            return View(usuario);
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var usuario = await _contexto.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();
            ViewData["Roles"] = new SelectList(_contexto.Rols, "IdRol", "Nombre", usuario.IdRol);
            ViewData["Sucursales"] = new SelectList(_contexto.Sucursals, "IdSucursal", "Nombre", usuario.IdSucursal);
            return View(usuario);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdUsuario,NombreCompleto,Email,PasswordHash,IdRol,IdSucursal,Estado")] Usuario usuario)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id != usuario.IdUsuario) return NotFound();

            if (!string.IsNullOrEmpty(usuario.NombreCompleto) && !System.Text.RegularExpressions.Regex.IsMatch(usuario.NombreCompleto.Trim(), @"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$"))
            {
                ModelState.AddModelError("NombreCompleto", "El nombre solo debe contener letras y espacios");
                ViewData["Roles"] = new SelectList(_contexto.Rols, "IdRol", "Nombre", usuario.IdRol);
                ViewData["Sucursales"] = new SelectList(_contexto.Sucursals, "IdSucursal", "Nombre", usuario.IdSucursal);
                return View(usuario);
            }

            ModelState.Remove("IdRolNavigation");
            ModelState.Remove("IdSucursalNavigation");
            ModelState.Remove("Ventas");
            ModelState.Remove("Compras");

            if (ModelState.IsValid)
            {
                try
                {

                    var usuarioActual = await _contexto.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == id);
                    
                    if (string.IsNullOrEmpty(usuario.PasswordHash))
                    {

                        usuario.PasswordHash = usuarioActual.PasswordHash;
                    }
                    else
                    {

                        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.PasswordHash);
                    }

                    _contexto.Update(usuario);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Usuario actualizado";
                    return RedirectToAction(nameof(Listar));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_contexto.Usuarios.Any(e => e.IdUsuario == id)) return NotFound();
                    else throw;
                }
            }
            ViewData["Roles"] = new SelectList(_contexto.Rols, "IdRol", "Nombre", usuario.IdRol);
            ViewData["Sucursales"] = new SelectList(_contexto.Sucursals, "IdSucursal", "Nombre", usuario.IdSucursal);
            return View(usuario);
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            if (id == null) return NotFound();
            var usuario = await _contexto.Usuarios
                .Include(u => u.IdRolNavigation)
                .Include(u => u.IdSucursalNavigation)
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            return usuario == null ? NotFound() : View(usuario);
        }

        [HttpPost, ActionName("Eliminar"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            if (!VerificarAuth()) return RedirectToAction("Login", "Autenticacion");
            var usuario = await _contexto.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                try
                {
                    _contexto.Usuarios.Remove(usuario);
                    await _contexto.SaveChangesAsync();
                    TempData["Mensaje"] = "Usuario eliminado";
                }
                catch (Exception)
                {
                    TempData["Error"] = "No se puede eliminar este usuario porque tiene ventas o registros asociados.";
                }
            }
            return RedirectToAction(nameof(Listar));
        }
    }
}
