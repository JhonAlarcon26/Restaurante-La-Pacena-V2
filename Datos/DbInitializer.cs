using la_paceña_v2.Models;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Datos
{
    public static class DbInitializer
    {
        public static void Initialize(ContextoRestaurante context)
        {
            context.Database.EnsureCreated();

            try
            {
                context.Database.ExecuteSqlRaw(@"
                    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AvasP_Proveedor' AND parent_object_id = OBJECT_ID('avastecimiento_plato'))
                    BEGIN
                        ALTER TABLE avastecimiento_plato DROP CONSTRAINT FK_AvasP_Proveedor
                    END
                ");

                context.Database.ExecuteSqlRaw(@"
                    IF EXISTS (SELECT * FROM sys.columns WHERE name = 'id_proveedor' AND object_id = OBJECT_ID('avastecimiento_plato'))
                    BEGIN
                        ALTER TABLE avastecimiento_plato DROP COLUMN id_proveedor
                    END
                ");
                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'id_plato' AND object_id = OBJECT_ID('kardex_general'))
                    BEGIN
                        ALTER TABLE kardex_general ADD id_plato INT NULL
                    END
                ");

                context.Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Kardex_Plato' AND parent_object_id = OBJECT_ID('kardex_general'))
                    BEGIN
                        ALTER TABLE kardex_general ADD CONSTRAINT FK_Kardex_Plato FOREIGN KEY (id_plato) REFERENCES plato(id_plato)
                    END
                ");

                context.Database.ExecuteSqlRaw(@"
                    IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Kardex_Item' AND parent_object_id = OBJECT_ID('kardex_general'))
                    BEGIN
                        ALTER TABLE kardex_general DROP CONSTRAINT CK_Kardex_Item
                    END
                ");

                context.Database.ExecuteSqlRaw(@"
                    ALTER TABLE kardex_general ADD CONSTRAINT CK_Kardex_Item CHECK (
                        ([id_insumo] IS NOT NULL AND [id_producto] IS NULL AND [id_plato] IS NULL) OR 
                        ([id_insumo] IS NULL AND [id_producto] IS NOT NULL AND [id_plato] IS NULL) OR 
                        ([id_insumo] IS NULL AND [id_producto] IS NULL AND [id_plato] IS NOT NULL)
                    )
                ");
            }
            catch { }

            var rolAdmin = context.Rols.FirstOrDefault(r => r.Nombre == "Administrador");
            if (rolAdmin == null)
            {
                rolAdmin = new Rol { Nombre = "Administrador", Descripcion = "Acceso total", Estado = true };
                context.Rols.Add(rolAdmin);
                context.SaveChanges();
            }

            var sucursal = context.Restaurantes.FirstOrDefault();
            if (sucursal == null)
            {
                sucursal = new Restaurante { Nombre = "Sucursal Principal", Direccion = "Central", Ciudad = "La Paz", Telefono = "000", Estado = true };
                context.Restaurantes.Add(sucursal);
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
                IdRestaurante = sucursal.IdRestaurante,
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
                    IdRestaurante = sucursal.IdRestaurante,
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

            // Inyectar Permisos Base para el Rol Administrador (Catálogo Estandarizado)
            var permisosBase = new List<string>
            {
                "VerDashboard",
                // Administración
                "VerUsuarios", "CrearUsuarios", "EditarUsuarios", "EliminarUsuarios",
                "VerRoles", "CrearRoles", "EditarRoles", "EliminarRoles",
                "VerPermisos", "AsignarPermisos", "VerLogs",
                "VerSucursales", "CrearSucursales", "EditarSucursales", "EliminarSucursales",
                
                // Inventario
                "VerProductos", "CrearProductos", "EditarProductos", "EliminarProductos",
                "VerInsumos", "CrearInsumos", "EditarInsumos", "EliminarInsumos",
                "VerCategorias", "CrearCategorias", "EditarCategorias", "EliminarCategorias",
                "VerAlmacenes", "CrearAlmacenes", "EditarAlmacenes", "EliminarAlmacenes",

                // Operaciones
                "VerCompras", "RegistrarCompras", "AnularCompras",
                "VerVentas", "RegistrarVentas", "AnularVentas",
                "VerAbastecimiento", "RegistrarAbastecimiento",
                
                // Reportes e Impresión
                "VerReportes", "ImprimirReportes", "ImprimirTickets", "ExportarExcel",
                
                // Otros específicos solicitados
                "VerClientes", "CrearClientes", "EditarClientes", "EliminarClientes",
                "VerProveedores", "CrearProveedores", "EditarProveedores", "EliminarProveedores"
            };

            foreach (var p in permisosBase)
            {
                var permiso = context.Permisos.FirstOrDefault(x => x.Nombre == p);
                if (permiso == null)
                {
                    permiso = new Permiso { Nombre = p, Descripcion = "Permiso del sistema", Estado = true };
                    context.Permisos.Add(permiso);
                    context.SaveChanges();
                }

                if (!context.RolYPermisos.Any(x => x.IdPermiso == permiso.IdPermiso && x.IdRol == rolAdmin.IdRol))
                {
                    context.RolYPermisos.Add(new RolYPermiso { IdRol = rolAdmin.IdRol, IdPermiso = permiso.IdPermiso, Estado = true });
                }
            }
            context.SaveChanges();
        }
    }
}
