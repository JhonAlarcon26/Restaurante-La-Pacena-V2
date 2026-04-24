namespace la_paceña_v2.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string RazonSocial { get; set; } = null!;

    public string? NitCarnet { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}
