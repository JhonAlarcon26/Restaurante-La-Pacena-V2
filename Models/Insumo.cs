using System;
using System.Collections.Generic;

namespace la_paceña_v2.Models;

public partial class Insumo
{
    public int IdInsumo { get; set; }

    public string Nombre { get; set; } = null!;

    public int IdUnidad { get; set; }

    public decimal? CostoPromedio { get; set; }

    public decimal? StockActual { get; set; }

    public decimal? StockMinimo { get; set; }

    public decimal? MermaPorcentaje { get; set; }

    public bool? Estado { get; set; }

    public virtual UnidadDeMedidum IdUnidadNavigation { get; set; } = null!;

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual ICollection<Recetum> Receta { get; set; } = new List<Recetum>();

    public virtual ICollection<KardexGeneral> KardexGenerals { get; set; } = new List<KardexGeneral>();
}
