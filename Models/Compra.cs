namespace la_paceña_v2.Models;

public partial class Compra
{
    public int IdCompra { get; set; }

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El proveedor es obligatorio")]
    public int IdProveedor { get; set; }

    public int IdUsuario { get; set; }

    public DateTime? FechaCompra { get; set; }

    public string? NroFacturaProveedor { get; set; }

    public decimal TotalCompra { get; set; }

    public string? Estado { get; set; }

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
