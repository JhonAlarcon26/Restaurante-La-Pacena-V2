using System;
using System.Collections.Generic;

namespace la_paceña_v2.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public string? CodigoBarras { get; set; }

    public int IdTipoProducto { get; set; }

    public int? StockActual { get; set; }

    public int? StockMinimo { get; set; }

    public decimal? PrecioCompraActual { get; set; }

    public decimal PrecioVenta { get; set; }

    public bool? Estado { get; set; }

    public int IdUnidad { get; set; }

    public virtual TipoProducto IdTipoProductoNavigation { get; set; } = null!;

    public virtual UnidadDeMedidum IdUnidadNavigation { get; set; } = null!;

    public virtual ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();

    public virtual ICollection<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();

    public virtual ICollection<KardexGeneral> KardexGenerals { get; set; } = new List<KardexGeneral>();
}
