namespace la_paceña_v2.Models;

public partial class DetalleAbastecimiento
{
    public int IdDetalleAbastecimiento { get; set; }

    public int IdAbastecimiento { get; set; }

    public int IdProducto { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Abastecimiento IdAbastecimientoNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
