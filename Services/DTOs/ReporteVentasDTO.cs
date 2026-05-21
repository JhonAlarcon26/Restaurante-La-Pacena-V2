namespace la_paceña_v2.Services.DTOs
{
    public class ReporteVentasDTO
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal TotalVentas { get; set; }
        public int CantidadVentas { get; set; }
        public int CantidadTransacciones => CantidadVentas;
        public decimal TicketPromedio => CantidadVentas > 0 ? TotalVentas / CantidadVentas : 0;
        public List<VentaDetalleDTO> Detalles { get; set; } = new List<VentaDetalleDTO>();
    }

    public class VentaDetalleDTO
    {
        public int IdVenta { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public string Usuario { get; set; }
        public string TipoPago { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public string Restaurante { get; set; }
        public int CantidadItems { get; set; }
        public int IdTransaccion => IdVenta;
    }

    public class VentaPorFechaDTO
    {
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public int Cantidad { get; set; }
    }
}
