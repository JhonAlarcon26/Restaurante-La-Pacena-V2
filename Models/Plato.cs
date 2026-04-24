namespace la_paceña_v2.Models;

public partial class Plato
{
    public int IdPlato { get; set; }

    public string Nombre { get; set; } = null!;

    public string? DescripcionCarta { get; set; }

    public decimal PrecioVenta { get; set; }

    public int IdCategoria { get; set; }

    public bool? Estado { get; set; }

    public virtual CategoriaPlato IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<Recetum> Receta { get; set; } = new List<Recetum>();

    public virtual ICollection<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();
}
