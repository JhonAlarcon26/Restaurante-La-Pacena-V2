namespace la_paceña_v2.Models;

public partial class Venta
{
    public int IdVenta { get; set; }

    public int IdUsuario { get; set; }

    public int IdCliente { get; set; }

    public int IdSucursal { get; set; }

    public DateTime? FechaVenta { get; set; }

    public int? NroMesa { get; set; }

    public decimal TotalVenta { get; set; }

    public int? IdTipoPago { get; set; }

    public string? Estado { get; set; }

    public virtual ICollection<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Sucursal IdSucursalNavigation { get; set; } = null!;

    public virtual TipoDePago? IdTipoPagoNavigation { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
