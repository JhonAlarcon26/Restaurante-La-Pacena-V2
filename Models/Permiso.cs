using System;
using System.Collections.Generic;

namespace la_paceña_v2.Models;

public partial class Permiso
{
    public int IdPermisos { get; set; }

    public string NombreTecnico { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
}
