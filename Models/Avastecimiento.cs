namespace la_paceña_v2.Models;

public partial class Avastecimiento
{
    public int IdAvastecimiento { get; set; }

    public int IdProveedor { get; set; }

    public int IdUsuario { get; set; }

    public DateTime? FechaAvastecimiento { get; set; }

    public string? NroFacturaProveedor { get; set; }

    public decimal TotalAvastecimiento { get; set; }

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<DetalleAvastecimiento> DetalleAvastecimiento { get; set; } = new List<DetalleAvastecimiento>();
}
