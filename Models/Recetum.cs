namespace la_paceña_v2.Models;

public partial class Recetum
{
    public int IdReceta { get; set; }

    public int IdPlato { get; set; }

    public int IdInsumo { get; set; }

    public decimal Cantidad { get; set; }

    public virtual Plato IdPlatoNavigation { get; set; } = null!;

    public virtual Insumo IdInsumoNavigation { get; set; } = null!;
}
