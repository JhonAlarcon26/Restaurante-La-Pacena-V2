namespace la_paceña_v2.Models;

public partial class Proveedor
{
    public int IdProveedor { get; set; }

    public string NombreEmpresa { get; set; } = null!;

    public string? ContactoNombre { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public bool? Estado { get; set; }

    public string? NitCarnet { get; set; }

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();
    public virtual ICollection<Avastecimiento> Avastecimiento { get; set; } = new List<Avastecimiento>();
}
