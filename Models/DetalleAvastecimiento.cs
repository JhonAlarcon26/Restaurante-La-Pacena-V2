namespace la_paceña_v2.Models;

public partial class DetalleAvastecimiento
{
    public int IdDetalleAvastecimiento { get; set; }

    public int IdAvastecimiento { get; set; }

    public int IdProducto { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Avastecimiento IdAvastecimientoNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
