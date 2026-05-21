namespace la_paceña_v2.Models;

public partial class Insumo
{
    public int IdInsumo { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdUnidad { get; set; }


    public bool? Estado { get; set; }

    public virtual UnidadDeMedida IdUnidadNavigation { get; set; } = null!;

    public virtual ICollection<DetalleCompraInsumo> DetalleCompras { get; set; } = new List<DetalleCompraInsumo>();

    public virtual ICollection<HistorialDeMovimiento> HistorialDeMovimientos { get; set; } = new List<HistorialDeMovimiento>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
