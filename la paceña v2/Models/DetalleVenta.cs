namespace la_paceña_v2.Models;

public partial class DetalleVenta
{
    public int IdDetalleVenta { get; set; }

    public int IdVenta { get; set; }

    public int? IdPlato { get; set; }

    public int? IdProducto { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Plato? IdPlatoNavigation { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }

    public virtual Venta IdVentaNavigation { get; set; } = null!;
}
