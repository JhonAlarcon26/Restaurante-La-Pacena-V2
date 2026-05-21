namespace la_paceña_v2.Models;

public partial class Permiso
{
    public int IdPermiso { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<RolYPermiso> RolYPermisos { get; set; } = new List<RolYPermiso>();
}
