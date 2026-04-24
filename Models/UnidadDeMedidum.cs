namespace la_paceña_v2.Models;

public partial class UnidadDeMedidum
{
    public int IdUnidad { get; set; }

    public string Nombre { get; set; } = null!;

    public string Abreviatura { get; set; } = null!;


    public virtual ICollection<Insumo> Insumos { get; set; } = new List<Insumo>();

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
