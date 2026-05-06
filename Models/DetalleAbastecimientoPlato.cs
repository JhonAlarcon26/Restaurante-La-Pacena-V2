namespace la_paceña_v2.Models;

public partial class DetalleAbastecimientoPlato
{
    public int IdDetalleAbastecimientoPlato { get; set; }
    public int IdAbastecimientoPlato { get; set; }
    public int IdPlato { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }

    public virtual AbastecimientoPlato IdAbastecimientoPlatoNavigation { get; set; } = null!;
    public virtual Plato IdPlatoNavigation { get; set; } = null!;
}
