namespace la_paceña_v2.Models;

public partial class CompraInsumo
{
    public int IdCompra { get; set; }

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El proveedor es obligatorio")]
    public int IdProveedor { get; set; }

    public int IdUsuario { get; set; }

    public DateTime? FechaCompra { get; set; }

    public string? NroReferencia { get; set; }

    public decimal TotalCompra { get; set; }

    public string? Estado { get; set; }

    public virtual ICollection<DetalleCompraInsumo> DetalleCompras { get; set; } = new List<DetalleCompraInsumo>();

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
