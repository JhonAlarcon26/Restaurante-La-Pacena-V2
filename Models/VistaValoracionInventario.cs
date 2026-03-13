using System;
using System.Collections.Generic;

namespace la_paceña_v2.Models;

public partial class VistaValoracionInventario
{
    public string Categoria { get; set; } = null!;

    public int TotalItems { get; set; }

    public decimal? DineroInvertido { get; set; }
}
