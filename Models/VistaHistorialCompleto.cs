using System;
using System.Collections.Generic;

namespace la_paceña_v2.Models;

public partial class VistaHistorialCompleto
{
    public DateTime? Fecha { get; set; }

    public string TipoMercancia { get; set; } = null!;

    public int IdItem { get; set; }

    public string Item { get; set; } = null!;

    public string TipoMovimiento { get; set; } = null!;

    public decimal Cantidad { get; set; }

    public string Unidad { get; set; } = null!;
}
