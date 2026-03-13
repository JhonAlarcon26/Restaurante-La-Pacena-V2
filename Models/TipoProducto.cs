using System;
using System.Collections.Generic;

namespace la_paceña_v2.Models;

public partial class TipoProducto
{
    public int IdTipoProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
