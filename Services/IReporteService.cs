using la_paceña_v2.Services.DTOs;

namespace la_paceña_v2.Services
{
    public interface IReporteService
    {
        Task<ReporteVentasDTO> ObtenerReporteVentas(DateTime fechaInicio, DateTime fechaFin, int? idSucursal, int? idUsuario);
        Task<ReporteComprasDTO> ObtenerReporteCompras(DateTime fechaInicio, DateTime fechaFin, int? idProveedor);
        Task<List<ReporteStockDTO>> ObtenerReporteStock(string? tipo = null, string? estado = null);
        Task<DashboardKPIsDTO> ObtenerKPIsDashboard();
    }
}
