namespace la_paceña_v2.Services.DTOs
{
    public class ReporteComprasDTO
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal TotalCompras { get; set; }
        public int CantidadCompras { get; set; }
        public int CantidadTransacciones => CantidadCompras;
        public List<CompraDetalleDTO> Detalles { get; set; } = new List<CompraDetalleDTO>();
    }

    public class CompraDetalleDTO
    {
        public int IdCompra { get; set; }
        public DateTime Fecha { get; set; }
        public string Proveedor { get; set; }
        public string Usuario { get; set; }
        public string NroFactura { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public int CantidadItems { get; set; }
        public int IdTransaccion => IdCompra;
    }
}
