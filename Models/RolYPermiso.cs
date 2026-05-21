using System;
using System.Collections.Generic;

namespace la_paceña_v2.Models;

public partial class RolYPermiso
{
    public int IdRolYPermiso { get; set; }

    public int IdRol { get; set; }

    public int IdPermiso { get; set; }

    public string? Vista { get; set; }

    public string? Controlador { get; set; }

    public bool? Estado { get; set; }

    public virtual Permiso IdPermisoNavigation { get; set; } = null!;

    public virtual Rol IdRolNavigation { get; set; } = null!;
}
