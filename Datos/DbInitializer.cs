using System;
using System.Linq;
using la_paceña_v2.Models;

namespace la_paceña_v2.Datos
{
    public static class DbInitializer
    {
        public static void Initialize(ContextoRestaurante context)
        {
            context.Database.EnsureCreated();


            var rolAdmin = context.Rols.FirstOrDefault(r => r.Nombre == "Administrador");
            if (rolAdmin == null)
            {
                rolAdmin = new Rol { Nombre = "Administrador", Descripcion = "Acceso total", Estado = true };
                context.Rols.Add(rolAdmin);
                context.SaveChanges();
            }

            var sucursal = context.Sucursals.FirstOrDefault();
            if (sucursal == null)
            {
                sucursal = new Sucursal { Nombre = "Sucursal Principal", Direccion = "Central", Ciudad = "La Paz", Telefono = "000", Estado = true };
                context.Sucursals.Add(sucursal);
                context.SaveChanges();
            }

            var usuarioJhon = context.Usuarios.FirstOrDefault(u => u.Email == "jhonalarcon88@gmail.com");
            if (usuarioJhon != null)
            {
                context.Usuarios.Remove(usuarioJhon);
                context.SaveChanges();
            }

            usuarioJhon = new Usuario
            {
                NombreCompleto = "Jhon Alarcon",
                Email = "jhonalarcon88@gmail.com",

                PasswordHash = BCrypt.Net.BCrypt.HashPassword("75629487"),
                IdRol = rolAdmin.IdRol,
                IdSucursal = sucursal.IdSucursal,
                Estado = true
            };
            context.Usuarios.Add(usuarioJhon);
            context.SaveChanges();





            var adminUser = context.Usuarios.FirstOrDefault(u => u.Email == "admin@lapacena.com");
            if (adminUser == null)
            {
                adminUser = new Usuario
                {
                    NombreCompleto = "Administrador del Sistema",
                    Email = "admin@lapacena.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    IdRol = rolAdmin.IdRol,
                    IdSucursal = sucursal.IdSucursal,
                    Estado = true
                };
                context.Usuarios.Add(adminUser);
            }
            else
            {
                // Asegurar que la contraseña sea 123456 si el usuario ya existe (PLANT TEXT TEMPORAL para debug)
                adminUser.PasswordHash = "123456";
                adminUser.Estado = true; // Asegurar que esté activo
            }
            context.SaveChanges();
        }
    }
}
