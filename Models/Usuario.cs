namespace la_paceña_v2.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int IdRol { get; set; }

    public int IdSucursal { get; set; }

    public string Nombre { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool? Estado { get; set; }

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual Sucursal IdSucursalNavigation { get; set; } = null!;

    public virtual ICollection<Compra> Compras { get; set; } = new List<Compra>();

    public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();

    public virtual ICollection<Avastecimiento> Avastecimientos { get; set; } = new List<Avastecimiento>();
}
