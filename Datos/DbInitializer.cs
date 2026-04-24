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

            var usuarioJhon = context.Usuarios.FirstOrDefault(u => u.Username == "jhonalarcon");
            if (usuarioJhon != null)
            {
                context.Usuarios.Remove(usuarioJhon);
                context.SaveChanges();
            }

            usuarioJhon = new Usuario
            {
                Nombre = "Jhon Alarcon",
                Username = "jhonalarcon",
                Password = BCrypt.Net.BCrypt.HashPassword("756294871"),
                IdRol = rolAdmin.IdRol,
                IdSucursal = sucursal.IdSucursal,
                Estado = true
            };
            context.Usuarios.Add(usuarioJhon);
            context.SaveChanges();





            var adminUser = context.Usuarios.FirstOrDefault(u => u.Username == "admin");
            if (adminUser == null)
            {
                adminUser = new Usuario
                {
                    Nombre = "Administrador del Sistema",
                    Username = "admin",
                    Password = BCrypt.Net.BCrypt.HashPassword("admin"),
                    IdRol = rolAdmin.IdRol,
                    IdSucursal = sucursal.IdSucursal,
                    Estado = true
                };
                context.Usuarios.Add(adminUser);
            }
            else
            {
                adminUser.Password = BCrypt.Net.BCrypt.HashPassword("admin");
                adminUser.Estado = true;
            }
            context.SaveChanges();
        }
    }
}
