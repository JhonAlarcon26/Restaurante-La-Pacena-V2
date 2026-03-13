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
    public class AutenticacionController : Controller
    {
        private readonly ContextoRestaurante _contexto;

        public AutenticacionController(ContextoRestaurante contexto)
        {
            _contexto = contexto;
        }

        public IActionResult Login()
        {

            if (HttpContext.Session.GetInt32("IdUsuario") != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            if (Request.Cookies.TryGetValue("RecordarEmail", out string emailRecordado))
            {
                ViewBag.EmailRecordado = emailRecordado;
            }
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, bool recordarSesion = false)
        {

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Por favor ingrese email y contraseña";
                return View();
            }

            if (!email.Contains("@") || email.Length < 5)
            {
                ViewBag.Error = "El formato del email no es válido";
                return View();
            }

            if (password.Length < 4)
            {
                ViewBag.Error = "La contraseña es demasiado corta";
                return View();
            }

            var intentosKey = $"LoginAttempts_{email}";
            var bloqueoKey = $"LoginBlocked_{email}";

            var bloqueadoHasta = HttpContext.Session.GetString(bloqueoKey);
            if (!string.IsNullOrEmpty(bloqueadoHasta))
            {
                var tiempoBloqueo = DateTime.Parse(bloqueadoHasta);
                if (DateTime.Now < tiempoBloqueo)
                {
                    var segundosRestantes = (int)(tiempoBloqueo - DateTime.Now).TotalSeconds;
                    ViewBag.Error = $"Cuenta bloqueada temporalmente. Intente nuevamente en {segundosRestantes} segundos";
                    return View();
                }
                else
                {

                    HttpContext.Session.Remove(bloqueoKey);
                    HttpContext.Session.Remove(intentosKey);
                }
            }

            var usuario = await _contexto.Usuarios
                .Include(u => u.IdRolNavigation)
                .Include(u => u.IdSucursalNavigation)
                .FirstOrDefaultAsync(u => u.Email == email && u.Estado == true);

            if (usuario == null)
            {
                // Debug
                var debugUser = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
                if (debugUser == null)
                {
                    ViewBag.Error = "Usuario no encontrado en la base de datos.";
                }
                else if (debugUser.Estado == false)
                {
                    ViewBag.Error = "El usuario existe pero está inactivo (Estado = false).";
                }
                else
                {
                    ViewBag.Error = "Credenciales incorrectas (Usuario encontrado).";
                }
                RegistrarIntentoFallido(email, intentosKey, bloqueoKey);
                return View();
            }

            bool passwordValido = false;
            bool necesitaActualizarHash = false;

            if (usuario.PasswordHash.StartsWith("$2") && (usuario.PasswordHash.Length == 60 || usuario.PasswordHash.Length == 59))
            {
                try
                {
                    if (BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
                    {
                        passwordValido = true;
                    }
                }
                catch (Exception)
                {

                }
            }

            if (!passwordValido)
            {
                if (usuario.PasswordHash == password)
                {
                    passwordValido = true;
                    necesitaActualizarHash = true;
                }
            }

            if (!passwordValido)
            {
                RegistrarIntentoFallido(email, intentosKey, bloqueoKey);
                RegistrarIntentoFallido(email, intentosKey, bloqueoKey);
                ViewBag.Error = $"Credenciales incorrectas. Hash en DB: {usuario.PasswordHash?.Substring(0, Math.Min(10, usuario.PasswordHash?.Length ?? 0))}..."; 
                return View();
                return View();
            }

            if (necesitaActualizarHash)
            {
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                _contexto.Usuarios.Update(usuario);
                await _contexto.SaveChangesAsync();
            }

            HttpContext.Session.Remove(intentosKey);
            HttpContext.Session.Remove(bloqueoKey);

            HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);
            HttpContext.Session.SetString("NombreCompleto", usuario.NombreCompleto);
            HttpContext.Session.SetInt32("IdRol", usuario.IdRol);
            HttpContext.Session.SetString("NombreRol", usuario.IdRolNavigation?.Nombre ?? "Usuario");
            HttpContext.Session.SetInt32("IdSucursal", usuario.IdSucursal);
            HttpContext.Session.SetString("NombreSucursal", usuario.IdSucursalNavigation?.Nombre ?? "Sin sucursal");

            if (recordarSesion)
            {
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddHours(12),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    IsEssential = true
                };

                Response.Cookies.Append("RecordarEmail", email, cookieOptions);
            }
            else
            {

                Response.Cookies.Delete("RecordarEmail");
            }

            TempData["Mensaje"] = $"¡Bienvenido {usuario.NombreCompleto}!";
            return RedirectToAction("Index", "Dashboard");
        }

        private void RegistrarIntentoFallido(string email, string intentosKey, string bloqueoKey)
        {
            var intentos = HttpContext.Session.GetInt32(intentosKey) ?? 0;
            intentos++;
            HttpContext.Session.SetInt32(intentosKey, intentos);

            if (intentos >= 5)
            {
                var tiempoBloqueo = DateTime.Now.AddMinutes(5);
                HttpContext.Session.SetString(bloqueoKey, tiempoBloqueo.ToString());
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Mensaje"] = "Sesión cerrada correctamente";
            return RedirectToAction("Login");
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> FixAdmin()
        {
            var adminUser = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Email == "admin@lapacena.com");
            var rolAdmin = await _contexto.Rols.FirstOrDefaultAsync(r => r.Nombre == "Administrador");
            var sucursal = await _contexto.Sucursals.FirstOrDefaultAsync();

            if (rolAdmin == null || sucursal == null)
            {
                 return Content("Error: Faltan roles o sucursales. No se puede crear admin.");
            }

            if (adminUser == null)
            {
                adminUser = new Usuario
                {
                    NombreCompleto = "Administrador del Sistema",
                    Email = "admin@lapacena.com",
                    PasswordHash = "123456", // Texto plano para que el login lo detecte y actualice
                    IdRol = rolAdmin.IdRol,
                    IdSucursal = sucursal.IdSucursal,
                    Estado = true
                };
                _contexto.Usuarios.Add(adminUser);
                await _contexto.SaveChangesAsync();
                return Content("ÉXITO: Usuario admin@lapacena.com creado con password '123456'. Intenta loguearte ahora.");
            }
            else
            {
                adminUser.PasswordHash = "123456";
                adminUser.Estado = true;
                await _contexto.SaveChangesAsync();
                return Content("ÉXITO: Usuario admin@lapacena.com existía. Se reseteó password a '123456' y se activó. Intenta loguearte ahora.");
            }
        }
    }
}
