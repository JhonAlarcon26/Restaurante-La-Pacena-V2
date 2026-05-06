namespace la_paceña_v2.Services.DTOs
{
    public class DashboardKPIsDTO
    {
        public decimal VentasHoy { get; set; }
        public int TransaccionesHoy { get; set; }
        public decimal VentasMes { get; set; }
        public decimal ComprasMes { get; set; }
        public List<PlatoTopDTO> PlatosTop { get; set; } = new List<PlatoTopDTO>();
        public List<PlatoTopDTO> TopPlatos => PlatosTop;
        public List<TopProductoDTO> ProductosTop { get; set; } = new List<TopProductoDTO>();
        public int AlertasStockBajo { get; set; }
        public List<VentaPorFechaDTO> VentasUltimos7Dias { get; set; } = new List<VentaPorFechaDTO>();
    }

    public class PlatoTopDTO
    {
        public string Nombre { get; set; }
        public int CantidadVendida { get; set; }
        public decimal IngresosGenerados { get; set; }
    }

    public class TopProductoDTO
    {
        public string Nombre { get; set; }
        public int CantidadVendida { get; set; }
        public decimal IngresosGenerados { get; set; }
    }
}
