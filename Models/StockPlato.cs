using System;
using System.Collections.Generic;

namespace la_paceña_v2.Models;

public partial class StockPlato
{
    public int IdStockPlato { get; set; }

    public int IdRestaurante { get; set; }

    public int IdPlato { get; set; }

    public decimal CantidadActual { get; set; }

    public decimal CantidadMinima { get; set; }

    public DateTime? UltimaActualizacion { get; set; }

    public virtual Plato IdPlatoNavigation { get; set; } = null!;

    public virtual Restaurante IdSucursalNavigation { get; set; } = null!;
}
