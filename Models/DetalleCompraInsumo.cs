namespace la_paceña_v2.Models;

public partial class DetalleCompraInsumo
{
    public int IdDetalleCompra { get; set; }

    public int IdCompra { get; set; }

    public int IdInsumo { get; set; }

    public decimal Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public virtual CompraInsumo IdCompraNavigation { get; set; } = null!;

    public virtual Insumo IdInsumoNavigation { get; set; } = null!;
}
