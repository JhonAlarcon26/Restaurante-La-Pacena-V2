namespace la_paceña_v2.Services.DTOs
{
    public class ReporteStockDTO
    {
        public int IdItem { get; set; }
        public string Tipo { get; set; }
        public string Nombre { get; set; }
        public string Insumo => Nombre;
        public string Unidad { get; set; }
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal CostoPromedio { get; set; }
        public decimal ValorTotal { get; set; }
        public string EstadoStock { get; set; }
        public bool EnAlerta { get; set; }
    }
}
