namespace la_paceña_v2.Models;

public partial class Abastecimiento
{
    public int IdAbastecimiento { get; set; }

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El proveedor es obligatorio")]
    public int IdProveedor { get; set; }

    public int IdUsuario { get; set; }

    public DateTime? FechaAbastecimiento { get; set; }

    public string? NroReferencia { get; set; }

    public decimal TotalAbastecimiento { get; set; }

    public virtual Proveedor IdProveedorNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<DetalleAbastecimiento> DetalleAbastecimientos { get; set; } = new List<DetalleAbastecimiento>();
}
