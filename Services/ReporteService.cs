using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using la_paceña_v2.Datos;
using la_paceña_v2.Models;
using la_paceña_v2.Services.DTOs;

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
                    Usuario = v.IdUsuarioNavigation?.NombreCompleto ?? "N/A",
                    TipoPago = v.IdTipoPagoNavigation?.Nombre ?? "N/A",
                    Total = v.TotalVenta,
                    Estado = v.Estado ?? "N/A"
                }).ToList()
            };

            return reporte;
        }

        public async Task<ReporteComprasDTO> ObtenerReporteCompras(DateTime fechaInicio, DateTime fechaFin, int? idProveedor)
        {
            var query = _contexto.Compras
                .Include(c => c.IdProveedorNavigation)
                .Include(c => c.IdUsuarioNavigation)
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
                    Proveedor = c.IdProveedorNavigation?.Empresa ?? "N/A",
                    Usuario = c.IdUsuarioNavigation?.NombreCompleto ?? "N/A",
                    NroFactura = c.NroFacturaProveedor ?? "S/N",
                    Total = c.TotalCompra,
                    Estado = c.Estado ?? "N/A"
                }).ToList()
            };

            return reporte;
        }

        public async Task<List<ReporteStockDTO>> ObtenerReporteStock()
        {
            var insumos = await _contexto.Insumos
                .Include(i => i.IdUnidadNavigation)
                .Where(i => i.Estado == true)
                .Select(i => new ReporteStockDTO
                {
                    Tipo = "INSUMO",
                    Nombre = i.Nombre,
                    StockActual = i.StockActual ?? 0,
                    StockMinimo = i.StockMinimo ?? 0,
                    Unidad = i.IdUnidadNavigation != null ? i.IdUnidadNavigation.Abreviatura : "N/A",
                    CostoPromedio = i.CostoPromedio ?? 0,
                    ValorTotal = (i.StockActual ?? 0) * (i.CostoPromedio ?? 0),
                    EstadoStock = ((i.StockActual ?? 0) <= (i.StockMinimo ?? 0)) ? "Bajo" : "Normal",
                    EnAlerta = (i.StockActual ?? 0) <= (i.StockMinimo ?? 0)
                }).ToListAsync();

            var productos = await _contexto.Productos
                .Include(p => p.IdUnidadNavigation)
                .Where(p => p.Estado == true)
                .Select(p => new ReporteStockDTO
                {
                    Tipo = "PRODUCTO",
                    Nombre = p.Nombre,
                    StockActual = p.StockActual ?? 0,
                    StockMinimo = p.StockMinimo ?? 0,
                    Unidad = p.IdUnidadNavigation != null ? p.IdUnidadNavigation.Abreviatura : "N/A",
                    CostoPromedio = p.PrecioCompraActual ?? 0,
                    ValorTotal = (p.StockActual ?? 0) * (p.PrecioCompraActual ?? 0),
                    EstadoStock = ((p.StockActual ?? 0) <= (p.StockMinimo ?? 0)) ? "Bajo" : "Normal",
                    EnAlerta = (p.StockActual ?? 0) <= (p.StockMinimo ?? 0)
                }).ToListAsync();

            insumos.AddRange(productos);
            return insumos;
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

            // Compras Mes
            var comprasMesTotal = await _contexto.Compras
                .Where(c => c.FechaCompra >= inicioMes && c.Estado != "Anulado")
                .SumAsync(c => (decimal?)c.TotalCompra) ?? 0;

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
