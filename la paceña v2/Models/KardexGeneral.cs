namespace la_paceña_v2.Models;

public partial class KardexGeneral
{
    public long IdKardex { get; set; }

    public DateTime? Fecha { get; set; }

    public string TipoMovimiento { get; set; } = null!;

    public int? IdInsumo { get; set; }

    public int? IdProducto { get; set; }

    public int? IdPlato { get; set; }

    public decimal Cantidad { get; set; }

    public decimal? CostoUnitario { get; set; }

    public int? IdReferencia { get; set; }

    public virtual Insumo? IdInsumoNavigation { get; set; }

    public virtual Producto? IdProductoNavigation { get; set; }

    public virtual Plato? IdPlatoNavigation { get; set; }
}
