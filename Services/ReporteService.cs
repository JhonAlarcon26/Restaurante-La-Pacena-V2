using la_paceña_v2.Datos;
using la_paceña_v2.Services.DTOs;
using Microsoft.EntityFrameworkCore;

namespace la_paceña_v2.Services
{
    public class ReporteService : IReporteService
    {
        private readonly ContextoRestaurante _contexto;
        public ReporteService(ContextoRestaurante contexto) => _contexto = contexto;

        public async Task<ReporteVentasDTO> ObtenerReporteVentas(DateTime fechaInicio, DateTime fechaFin, int? idSucursal, int? idUsuario)
        {
            var query = _contexto.Ventas
                .Include(v => v.IdUsuarioNavigation)
                .Include(v => v.IdClienteNavigation)
                .Include(v => v.IdTipoPagoNavigation)
                .Include(v => v.IdSucursalNavigation)
                .Include(v => v.DetalleVentas)
                .Where(v => v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin);

            if (idSucursal.HasValue)
                query = query.Where(v => v.IdSucursal == idSucursal.Value);

            if (idUsuario.HasValue)
                query = query.Where(v => v.IdUsuario == idUsuario.Value);

            var ventas = await query.OrderByDescending(v => v.FechaVenta).ToListAsync();

            var reporte = new ReporteVentasDTO
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                TotalVentas = ventas.Sum(v => v.TotalVenta),
                CantidadVentas = ventas.Count,
                Detalles = ventas.Select(v => new VentaDetalleDTO
                {
                    IdVenta = (int)v.IdVenta,
                    Fecha = v.FechaVenta ?? DateTime.MinValue,
                    Cliente = v.IdClienteNavigation?.RazonSocial ?? "Sin cliente",
                    Usuario = v.IdUsuarioNavigation?.Nombre ?? "N/A",
                    TipoPago = v.IdTipoPagoNavigation?.Nombre ?? "N/A",
                    Total = v.TotalVenta,
                    Estado = v.Estado ?? "N/A",
                    Sucursal = v.IdSucursalNavigation?.Nombre ?? "S/N",
                    CantidadItems = v.DetalleVentas.Count
                }).ToList()
            };

            return reporte;
        }

        public async Task<ReporteComprasDTO> ObtenerReporteCompraInsumos(DateTime fechaInicio, DateTime fechaFin, int? idProveedor)
        {
            var query = _contexto.Compras
                .Include(c => c.IdProveedorNavigation)
                .Include(c => c.IdUsuarioNavigation)
                .Include(c => c.DetalleCompras)
                .Where(c => c.FechaCompra >= fechaInicio && c.FechaCompra <= fechaFin);

            if (idProveedor.HasValue)
                query = query.Where(c => c.IdProveedor == idProveedor.Value);

            var compras = await query.OrderByDescending(c => c.FechaCompra).ToListAsync();

            var reporte = new ReporteComprasDTO
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                TotalCompras = compras.Sum(c => c.TotalCompra),
                CantidadCompras = compras.Count,
                Detalles = compras.Select(c => new CompraDetalleDTO
                {
                    IdCompra = c.IdCompra,
                    Fecha = c.FechaCompra ?? DateTime.MinValue,
                    Proveedor = c.IdProveedorNavigation?.NombreEmpresa ?? "N/A",
                    Usuario = c.IdUsuarioNavigation?.Nombre ?? "N/A",
                    NroFactura = c.NroFacturaProveedor ?? "S/N",
                    Total = c.TotalCompra,
                    Estado = c.Estado ?? "Completada",
                    CantidadItems = c.DetalleCompras.Count
                }).ToList()
            };

            return reporte;
        }

        public async Task<ReporteComprasDTO> ObtenerReporteCompraProductos(DateTime fechaInicio, DateTime fechaFin, int? idProveedor)
        {
            var query = _contexto.Abastecimientos
                .Include(c => c.IdProveedorNavigation)
                .Include(c => c.IdUsuarioNavigation)
                .Include(c => c.DetalleAbastecimientos)
                .Where(c => c.FechaAbastecimiento >= fechaInicio && c.FechaAbastecimiento <= fechaFin);

            if (idProveedor.HasValue)
                query = query.Where(c => c.IdProveedor == idProveedor.Value);

            var abastecimientos = await query.OrderByDescending(c => c.FechaAbastecimiento).ToListAsync();

            var reporte = new ReporteComprasDTO
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                TotalCompras = abastecimientos.Sum(c => c.TotalAbastecimiento),
                CantidadCompras = abastecimientos.Count,
                Detalles = abastecimientos.Select(c => new CompraDetalleDTO
                {
                    IdCompra = c.IdAbastecimiento,
                    Fecha = c.FechaAbastecimiento ?? DateTime.MinValue,
                    Proveedor = c.IdProveedorNavigation?.NombreEmpresa ?? "N/A",
                    Usuario = c.IdUsuarioNavigation?.Nombre ?? "N/A",
                    NroFactura = c.NroFacturaProveedor ?? "S/N",
                    Total = c.TotalAbastecimiento,
                    Estado = "Completado",
                    CantidadItems = c.DetalleAbastecimientos.Count
                }).ToList()
            };

            return reporte;
        }

        public async Task<List<ReporteStockDTO>> ObtenerReporteStock(string? tipo = null, string? estado = null)
        {
            var resultado = new List<ReporteStockDTO>();
            tipo = tipo?.Trim().ToUpper();

            if (string.IsNullOrEmpty(tipo) || tipo == "INSUMO")
            {
                var insumos = await _contexto.Insumos
                    .Include(i => i.IdUnidadNavigation)
                    .Include(i => i.Stocks)
                    .Where(i => i.Estado == true)
                    .Select(i => new
                    {
                        Insumo = i,
                        Stock = i.Stocks.FirstOrDefault()
                    })
                    .Select(x => new ReporteStockDTO
                    {
                        IdItem = x.Insumo.IdInsumo,
                        Tipo = "INSUMO",
                        Nombre = x.Insumo.Nombre,
                        StockActual = x.Stock != null ? x.Stock.CantidadActual : 0,
                        StockMinimo = x.Stock != null ? x.Stock.CantidadMinima : 0,
                        Unidad = x.Insumo.IdUnidadNavigation != null ? x.Insumo.IdUnidadNavigation.Abreviatura : "N/A",
                        CostoPromedio = x.Insumo.CostoPromedio ?? 0,
                        ValorTotal = (x.Stock != null ? x.Stock.CantidadActual : 0) * (x.Insumo.CostoPromedio ?? 0),
                        EstadoStock = (x.Stock != null && x.Stock.CantidadActual <= x.Stock.CantidadMinima) ? "Bajo" : "Normal",
                        EnAlerta = x.Stock != null && x.Stock.CantidadActual <= x.Stock.CantidadMinima
                    }).ToListAsync();
                resultado.AddRange(insumos);
            }

            if (string.IsNullOrEmpty(tipo) || tipo == "PRODUCTO")
            {
                var productos = await _contexto.Productos
                    .Include(p => p.IdUnidadNavigation)
                    .Include(p => p.Stocks)
                    .Where(p => p.Estado == true)
                    .Select(p => new
                    {
                        Producto = p,
                        Stock = p.Stocks.FirstOrDefault()
                    })
                    .Select(x => new ReporteStockDTO
                    {
                        IdItem = x.Producto.IdProducto,
                        Tipo = "PRODUCTO",
                        Nombre = x.Producto.Nombre,
                        StockActual = x.Stock != null ? x.Stock.CantidadActual : 0,
                        StockMinimo = x.Stock != null ? x.Stock.CantidadMinima : 0,
                        Unidad = x.Producto.IdUnidadNavigation != null ? x.Producto.IdUnidadNavigation.Abreviatura : "N/A",
                        CostoPromedio = x.Producto.PrecioCompraActual ?? 0,
                        ValorTotal = (x.Stock != null ? x.Stock.CantidadActual : 0) * (x.Producto.PrecioCompraActual ?? 0),
                        EstadoStock = (x.Stock != null && x.Stock.CantidadActual <= x.Stock.CantidadMinima) ? "Bajo" : "Normal",
                        EnAlerta = x.Stock != null && x.Stock.CantidadActual <= x.Stock.CantidadMinima
                    }).ToListAsync();
                resultado.AddRange(productos);
            }

            if (string.IsNullOrEmpty(tipo) || tipo == "PLATO")
            {
                var platos = await _contexto.StockPlatos
                    .Include(s => s.IdPlatoNavigation)
                    .Select(x => new ReporteStockDTO
                    {
                        IdItem = x.IdPlato,
                        Tipo = "PLATO",
                        Nombre = x.IdPlatoNavigation.Nombre,
                        StockActual = x.CantidadActual,
                        StockMinimo = x.CantidadMinima,
                        Unidad = "UNID",
                        CostoPromedio = x.IdPlatoNavigation.PrecioVenta,
                        ValorTotal = x.CantidadActual * x.IdPlatoNavigation.PrecioVenta,
                        EstadoStock = (x.CantidadActual <= x.CantidadMinima) ? "Bajo" : "Normal",
                        EnAlerta = x.CantidadActual <= x.CantidadMinima
                    }).ToListAsync();
                resultado.AddRange(platos);
            }

            if (!string.IsNullOrEmpty(estado))
            {
                resultado = resultado.Where(x => x.EstadoStock == estado).ToList();
            }

            return resultado.OrderBy(x => x.Nombre).ToList();
        }

        public async Task<ReporteStockDetalleDTO?> ObtenerDetalleStock(string tipo, string nombre)
        {
            var detalle = new ReporteStockDetalleDTO { Nombre = nombre, Tipo = tipo };

            var query = _contexto.KardexGenerals.AsQueryable();

            if (tipo == "INSUMO")
                query = query.Where(k => k.IdInsumoNavigation.Nombre == nombre);
            else if (tipo == "PRODUCTO")
                query = query.Where(k => k.IdProductoNavigation.Nombre == nombre);
            else if (tipo == "PLATO")
                query = query.Where(k => k.IdPlatoNavigation.Nombre == nombre);
            else
                return null;

            var movimientos = await query
                .OrderByDescending(k => k.Fecha)
                .Take(10)
                .Select(k => new MovimientoKardexDTO
                {
                    Fecha = k.Fecha ?? DateTime.MinValue,
                    TipoMovimiento = k.TipoMovimiento,
                    Cantidad = k.Cantidad,
                    Costo = k.CostoUnitario
                }).ToListAsync();

            detalle.Movimientos = movimientos;

            // Obtener unidad
            if (tipo == "INSUMO")
                detalle.Unidad = (await _contexto.Insumos.Include(i => i.IdUnidadNavigation).FirstOrDefaultAsync(i => i.Nombre == nombre))?.IdUnidadNavigation?.Abreviatura ?? "N/A";
            else if (tipo == "PRODUCTO")
                detalle.Unidad = (await _contexto.Productos.Include(p => p.IdUnidadNavigation).FirstOrDefaultAsync(p => p.Nombre == nombre))?.IdUnidadNavigation?.Abreviatura ?? "N/A";
            else
                detalle.Unidad = "UNID";

            return detalle;
        }

        public async Task<DashboardKPIsDTO> ObtenerKPIsDashboard()
        {
            var hoy = DateTime.Today;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

            // Ventas Hoy (Todas)
            var ventasHoyQuery = _contexto.Ventas
                .Where(v => v.FechaVenta >= hoy && v.FechaVenta < hoy.AddDays(1) && v.Estado != "Anulado");

            var ventasHoy = await ventasHoyQuery.SumAsync(v => (decimal?)v.TotalVenta) ?? 0;
            var cantidadVentasHoy = await ventasHoyQuery.CountAsync();

            // Ventas Mes
            var ventasMesTotal = await _contexto.Ventas
                .Where(v => v.FechaVenta >= inicioMes && v.Estado != "Anulado")
                .SumAsync(v => (decimal?)v.TotalVenta) ?? 0;

            // Compras Mes (Insumos + Productos)
            var comprasInsumosMes = await _contexto.Compras
                .Where(c => c.FechaCompra >= inicioMes && c.Estado != "Anulado")
                .SumAsync(c => (decimal?)c.TotalCompra) ?? 0;

            var abastecimientosProductosMes = await _contexto.Abastecimientos
                .Where(a => a.FechaAbastecimiento >= inicioMes)
                .SumAsync(a => (decimal?)a.TotalAbastecimiento) ?? 0;

            var comprasMesTotal = comprasInsumosMes + abastecimientosProductosMes;

            // Top Platos (DetalleVenta donde IdPlato != null)
            var platosTop = await _contexto.DetalleVentas
                .Include(d => d.IdPlatoNavigation)
                .Where(d => d.IdVentaNavigation.FechaVenta >= inicioMes && d.IdPlato != null)
                .GroupBy(d => new { d.IdPlato, d.IdPlatoNavigation.Nombre })
                .Select(g => new PlatoTopDTO
                {
                    Nombre = g.Key.Nombre,
                    CantidadVendida = (int)g.Sum(d => d.Cantidad),
                    IngresosGenerados = g.Sum(d => d.Subtotal)
                })
                .OrderByDescending(p => p.CantidadVendida)
                .Take(5)
                .ToListAsync();

            // Top Productos (DetalleVenta donde IdProducto != null)
            var productosTop = await _contexto.DetalleVentas
                .Include(d => d.IdProductoNavigation)
                .Where(d => d.IdVentaNavigation.FechaVenta >= inicioMes && d.IdProducto != null)
                .GroupBy(d => new { d.IdProducto, d.IdProductoNavigation.Nombre })
                .Select(g => new TopProductoDTO
                {
                    Nombre = g.Key.Nombre,
                    CantidadVendida = (int)g.Sum(d => d.Cantidad), // Cast needed as detail quantity is int but sum might differ? No, detail qty is int.
                    IngresosGenerados = g.Sum(d => d.Subtotal)
                })
                .OrderByDescending(p => p.CantidadVendida)
                .Take(5)
                .ToListAsync();

            // Ventas 7 dias
            var ventasUltimos7Dias = await _contexto.Ventas
                .Where(v => v.FechaVenta >= hoy.AddDays(-6) && v.Estado != "Anulado")
                .GroupBy(v => v.FechaVenta.Value.Date)
                .Select(g => new VentaPorFechaDTO
                {
                    Fecha = g.Key,
                    Total = g.Sum(v => v.TotalVenta),
                    Cantidad = g.Count()
                })
                .OrderBy(v => v.Fecha)
                .ToListAsync();

            return new DashboardKPIsDTO
            {
                VentasHoy = ventasHoy,
                TransaccionesHoy = cantidadVentasHoy,
                VentasMes = ventasMesTotal,
                ComprasMes = comprasMesTotal,
                PlatosTop = platosTop,
                ProductosTop = productosTop,
                VentasUltimos7Dias = ventasUltimos7Dias
            };
        }
    }
}
