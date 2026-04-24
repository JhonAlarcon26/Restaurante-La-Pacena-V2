namespace la_paceña_v2.Models;

public partial class Stock
{
    public int IdStock { get; set; }

    public int? IdInsumo { get; set; }

    public int? IdProducto { get; set; }

    public decimal CantidadActual { get; set; }

    public decimal CantidadMinima { get; set; }

    public DateTime? UltimaActualizacion { get; set; }

    public virtual Insumo? IdInsumoNavigation { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }
}
