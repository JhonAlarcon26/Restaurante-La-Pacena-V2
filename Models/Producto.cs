namespace la_paceña_v2.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public string? CodigoBarras { get; set; }

    public int IdTipoProducto { get; set; }

    public bool? Estado { get; set; }

    public int IdUnidad { get; set; }

    public virtual TipoProducto IdTipoProductoNavigation { get; set; } = null!;

    public virtual UnidadDeMedidum IdUnidadNavigation { get; set; } = null!;



    public virtual ICollection<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();

    public virtual ICollection<KardexGeneral> KardexGenerals { get; set; } = new List<KardexGeneral>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();

    public virtual ICollection<DetalleAbastecimiento> DetalleAbastecimientos { get; set; } = new List<DetalleAbastecimiento>();
}
