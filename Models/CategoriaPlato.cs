namespace la_paceña_v2.Models;

public partial class CategoriaPlato
{
    public int IdCategoria { get; set; }

    public string Nombre { get; set; } = null!;

    public string? ImpresoraDestino { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<Plato> Platos { get; set; } = new List<Plato>();


}
