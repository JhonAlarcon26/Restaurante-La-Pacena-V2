namespace la_paceña_v2.Models;

public partial class Plato
{
    public int IdPlato { get; set; }

    public string Nombre { get; set; } = null!;

    public string? DescripcionCarta { get; set; }

    public decimal PrecioVenta { get; set; }

    public int IdCategoria { get; set; }

    public bool? Estado { get; set; }

    public virtual CategoriaPlato IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();

    public virtual ICollection<DetalleAbastecimientoPlato> DetalleAbastecimientoPlatos { get; set; } = new List<DetalleAbastecimientoPlato>();

    public virtual ICollection<StockPlato> StockPlatos { get; set; } = new List<StockPlato>();

    public virtual ICollection<KardexGeneral> KardexGenerals { get; set; } = new List<KardexGeneral>();
}
