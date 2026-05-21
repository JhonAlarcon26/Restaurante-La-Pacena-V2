namespace la_paceña_v2.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int IdRol { get; set; }

    public int IdRestaurante { get; set; }

    public string Nombre { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool? Estado { get; set; }

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual Restaurante IdSucursalNavigation { get; set; } = null!;

    public virtual ICollection<CompraInsumo> Compras { get; set; } = new List<CompraInsumo>();

    public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();

    public virtual ICollection<CompraProducto> Abastecimientos { get; set; } = new List<CompraProducto>();
}
