using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            if (Request.Cookies.TryGetValue("RecordarUsername", out string usernameRecordado))
            {
                ViewBag.UsernameRecordado = usernameRecordado;
            }

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, bool recordarSesion)
        {

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Debe ingresar usuario y contraseña";
                return View();
            }

            var intentosKey = $"Intentos_{username}";
            var bloqueoKey = $"Bloqueo_{username}";

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
                .FirstOrDefaultAsync(u => u.Username == username);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.Password))
            {
                RegistrarIntentoFallido(username, intentosKey, bloqueoKey);
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View();
            }

            bool passwordValido = false;
            bool necesitaActualizarHash = false;

            if (usuario.Password.StartsWith("$2") && (usuario.Password.Length == 60 || usuario.Password.Length == 59))
            {
                try
                {
                    if (BCrypt.Net.BCrypt.Verify(password, usuario.Password))
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
                if (usuario.Password == password)
                {
                    passwordValido = true;
                    necesitaActualizarHash = true;
                }
            }

            if (!passwordValido)
            {
                RegistrarIntentoFallido(username, intentosKey, bloqueoKey);
                ViewBag.Error = "Credenciales incorrectas.";
                return View();
            }

            if (necesitaActualizarHash)
            {
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(password);
                _contexto.Usuarios.Update(usuario);
                await _contexto.SaveChangesAsync();
            }

            HttpContext.Session.Remove(intentosKey);
            HttpContext.Session.Remove(bloqueoKey);

            HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);
            HttpContext.Session.SetString("NombreCompleto", usuario.Nombre);
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

                Response.Cookies.Append("RecordarUsername", username, cookieOptions);
            }
            else
            {

                Response.Cookies.Delete("RecordarUsername");
            }

            TempData["Mensaje"] = $"¡Bienvenido {usuario.Nombre}!";
            return RedirectToAction("Index", "Dashboard");
        }

        private void RegistrarIntentoFallido(string username, string intentosKey, string bloqueoKey)
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
            var adminUser = await _contexto.Usuarios.FirstOrDefaultAsync(u => u.Username == "admin");
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
                    Nombre = "Administrador del Sistema",
                    Username = "admin",
                    Password = "admin",
                    IdRol = rolAdmin.IdRol,
                    IdSucursal = sucursal.IdSucursal,
                    Estado = true
                };
                _contexto.Usuarios.Add(adminUser);
                await _contexto.SaveChangesAsync();
                return Content("ÉXITO: Usuario admin creado con password 'admin'.");
            }
            else
            {
                adminUser.Password = "admin";
                adminUser.Estado = true;
                await _contexto.SaveChangesAsync();
                return Content("ÉXITO: Usuario admin reseteado.");
            }
        }
    }
}
