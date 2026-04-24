namespace la_paceña_v2.Models;

public partial class RolPermiso
{
    public int IdRolPermiso { get; set; }

    public int IdRol { get; set; }

    public int IdPermiso { get; set; }

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual Permiso IdPermisoNavigation { get; set; } = null!;
}
