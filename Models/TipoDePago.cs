using System;
using System.Collections.Generic;

namespace la_paceña_v2.Models;

public partial class TipoDePago
{
    public int IdTipoPago { get; set; }

    public string Nombre { get; set; } = null!;

    public bool? EsBancarizado { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}
