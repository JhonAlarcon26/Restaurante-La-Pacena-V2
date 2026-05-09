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
        public decimal MetaDiaria { get; set; }
        public decimal PorcentajeMeta { get; set; }
        public decimal VentasMesAnterior { get; set; }
        public List<MovimientoKardexSimplificadoDTO> UltimosMovimientos { get; set; } = new List<MovimientoKardexSimplificadoDTO>();
        public PlatoTopDTO PlatoEstrella { get; set; }
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

    public class MovimientoKardexSimplificadoDTO
    {
        public string ItemNombre { get; set; }
        public string TipoMovimiento { get; set; }
        public decimal Cantidad { get; set; }
        public DateTime Fecha { get; set; }
    }
}
