namespace la_paceña_v2.Models;

public partial class VistaAlertaStock
{
    public string Tipo { get; set; } = null!;

    public string Item { get; set; } = null!;

    public decimal? StockActual { get; set; }

    public decimal? StockMinimo { get; set; }

    public string Unidad { get; set; } = null!;

    public string? Estado { get; set; }
}
