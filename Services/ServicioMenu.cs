using System.Security.Claims;

namespace la_paceña_v2.Services
{
    public class ItemMenuDto
    {
        public string Titulo { get; set; }
        public string Controlador { get; set; }
        public string Accion { get; set; }
        public string Icono { get; set; }
        public string PoliticaRequerida { get; set; }
        public List<ItemMenuDto> SubItems { get; set; } = new List<ItemMenuDto>();
        public bool EsCabecera { get; set; }
    }

    public interface IServicioMenu
    {
        Task<IEnumerable<ItemMenuDto>> ObtenerMenuPermitidoAsync(ClaimsPrincipal usuario);
    }

    public class ServicioMenu : IServicioMenu
    {
        public async Task<IEnumerable<ItemMenuDto>> ObtenerMenuPermitidoAsync(ClaimsPrincipal usuario)
        {
            var menuCompleto = GenerarMenuBase();
            var menuFiltrado = new List<ItemMenuDto>();

            foreach (var grupo in menuCompleto)
            {
                var subItemsFiltrados = grupo.SubItems
                    .Where(si => string.IsNullOrEmpty(si.PoliticaRequerida) || usuario.HasClaim("Permiso", si.PoliticaRequerida))
                    .ToList();

                if (subItemsFiltrados.Any() || string.IsNullOrEmpty(grupo.PoliticaRequerida))
                {
                    var nuevoGrupo = new ItemMenuDto
                    {
                        Titulo = grupo.Titulo,
                        Icono = grupo.Icono,
                        Controlador = grupo.Controlador,
                        Accion = grupo.Accion,
                        SubItems = subItemsFiltrados,
                        EsCabecera = grupo.EsCabecera
                    };
                    menuFiltrado.Add(nuevoGrupo);
                }
            }

            return await Task.FromResult(menuFiltrado);
        }

        private List<ItemMenuDto> GenerarMenuBase()
        {
            return new List<ItemMenuDto>
            {
                new ItemMenuDto { Titulo = "Inicio", Controlador = "Dashboard", Accion = "Index", Icono = "M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6", PoliticaRequerida = "" },

                new ItemMenuDto
                {
                    Titulo = "Administración",
                    Icono = "M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z",
                    SubItems = new List<ItemMenuDto>
                    {
                        new ItemMenuDto { Titulo = "Usuario", Controlador = "Usuario", Accion = "Listar", PoliticaRequerida = "VerUsuarios" },
                        new ItemMenuDto { Titulo = "Rol", Controlador = "Rol", Accion = "Listar", PoliticaRequerida = "VerRoles" },
                        new ItemMenuDto { Titulo = "Permisos", Controlador = "Permisos", Accion = "Listar", PoliticaRequerida = "VerPermisos" }
                    }
                },

                new ItemMenuDto
                {
                    Titulo = "Configuración",
                    Icono = "M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065zM15 12a3 3 0 11-6 0 3 3 0 016 0z",
                    SubItems = new List<ItemMenuDto>
                    {
                        new ItemMenuDto { Titulo = "Tipo de Producto", Controlador = "TipoProducto", Accion = "Listar", PoliticaRequerida = "VerTipoProducto" },
                        new ItemMenuDto { Titulo = "Unidad de Medida", Controlador = "UnidadDeMedida", Accion = "Listar", PoliticaRequerida = "VerUnidadMedida" },
                        new ItemMenuDto { Titulo = "Tipo de Pago", Controlador = "TipoDePago", Accion = "Listar", PoliticaRequerida = "VerTipoPago" },
                        new ItemMenuDto { Titulo = "Categoria Plato", Controlador = "CategoriaPlato", Accion = "Listar", PoliticaRequerida = "VerCategorias" },
                        new ItemMenuDto { Titulo = "Sucursales", Controlador = "Sucursales", Accion = "Listar", PoliticaRequerida = "VerSucursales" }
                    }
                },

                new ItemMenuDto
                {
                    Titulo = "Compra",
                    Icono = "M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z",
                    SubItems = new List<ItemMenuDto>
                    {
                        new ItemMenuDto { Titulo = "Proveedor", Controlador = "Proveedor", Accion = "Listar", PoliticaRequerida = "VerProveedores" },
                        new ItemMenuDto { Titulo = "Insumo", Controlador = "Insumo", Accion = "Listar", PoliticaRequerida = "VerInsumos" },
                        new ItemMenuDto { Titulo = "Producto", Controlador = "Producto", Accion = "Listar", PoliticaRequerida = "VerProductos" },
                        new ItemMenuDto { Titulo = "Compra Producto", Controlador = "CompraProducto", Accion = "Listar", PoliticaRequerida = "VerCompraProducto" },
                        new ItemMenuDto { Titulo = "Compra Insumo", Controlador = "CompraInsumo", Accion = "Listar", PoliticaRequerida = "VerCompraInsumo" }
                    }
                },

                new ItemMenuDto
                {
                    Titulo = "Venta",
                    Icono = "M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2-2v14a2 2 0 002 2z",
                    SubItems = new List<ItemMenuDto>
                    {
                        new ItemMenuDto { Titulo = "Cliente", Controlador = "Cliente", Accion = "Listar", PoliticaRequerida = "VerClientes" },
                        new ItemMenuDto { Titulo = "Platos", Controlador = "Platos", Accion = "Listar", PoliticaRequerida = "VerPlatos" },
                        new ItemMenuDto { Titulo = "Abastecimiento Platos", Controlador = "AbastecimientoPlato", Accion = "Listar", PoliticaRequerida = "VerAbastecimientos" },
                        new ItemMenuDto { Titulo = "Ventas", Controlador = "Ventas", Accion = "Listar", PoliticaRequerida = "VerVentas" }
                    }
                },

                new ItemMenuDto
                {
                    Titulo = "Stock",
                    Icono = "M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4",
                    SubItems = new List<ItemMenuDto>
                    {
                        new ItemMenuDto { Titulo = "Stock", Controlador = "Stock", Accion = "Listar", PoliticaRequerida = "VerStock" },
                        new ItemMenuDto { Titulo = "Stock Platos", Controlador = "StockPlatos", Accion = "Listar", PoliticaRequerida = "VerStockPlatos" },
                        new ItemMenuDto { Titulo = "Kardex General", Controlador = "KardexGeneral", Accion = "Listar", PoliticaRequerida = "VerKardex" }
                    }
                },

                new ItemMenuDto
                {
                    Titulo = "Reporte",
                    Icono = "M9 17v-2m3 2v-4m3 4v-6m2 10H7a2 2 0 00-2 2v14a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4",
                    SubItems = new List<ItemMenuDto>
                    {
                        new ItemMenuDto { Titulo = "Reporte de compra insumo", Controlador = "ReporteCompraInsumo", Accion = "Index", PoliticaRequerida = "VerReporteCompraInsumo" },
                        new ItemMenuDto { Titulo = "Reporte de compra producto", Controlador = "ReporteCompraProducto", Accion = "Index", PoliticaRequerida = "VerReporteCompraProducto" },
                        new ItemMenuDto { Titulo = "Reporte de Venta", Controlador = "ReporteVenta", Accion = "Index", PoliticaRequerida = "VerReporteVentas" },
                        new ItemMenuDto { Titulo = "Reporte de Stock", Controlador = "ReporteStock", Accion = "Index", PoliticaRequerida = "VerReporteStock" }
                    }
                }
            };
        }
    }
}
