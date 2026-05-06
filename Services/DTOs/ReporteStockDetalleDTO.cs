namespace la_paceña_v2.Services.DTOs
{
    public class ReporteStockDetalleDTO
    {
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public string Unidad { get; set; } = null!;
        public decimal StockActual { get; set; }
        public List<MovimientoKardexDTO> Movimientos { get; set; } = new();
    }

    public class MovimientoKardexDTO
    {
        public DateTime Fecha { get; set; }
        public string TipoMovimiento { get; set; } = null!;
        public decimal Cantidad { get; set; }
        public decimal? Costo { get; set; }
    }
}
