using la_paceña_v2.Services.DTOs;

namespace la_paceña_v2.Services
{
    public interface IReporteService
    {
        Task<ReporteVentasDTO> ObtenerReporteVentas(DateTime fechaInicio, DateTime fechaFin, int? IdRestaurante, int? idUsuario);
        Task<ReporteComprasDTO> ObtenerReporteCompraInsumos(DateTime fechaInicio, DateTime fechaFin, int? idProveedor);
        Task<ReporteComprasDTO> ObtenerReporteCompraProductos(DateTime fechaInicio, DateTime fechaFin, int? idProveedor);
        Task<List<ReporteStockDTO>> ObtenerReporteStock(string? tipo = null, string? estado = null);
        Task<ReporteStockDetalleDTO?> ObtenerDetalleStock(string tipo, string nombre);
        Task<DashboardKPIsDTO> ObtenerKPIsDashboard();
    }
}
