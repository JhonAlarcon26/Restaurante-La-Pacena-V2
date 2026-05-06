namespace la_paceña_v2.Models;

public partial class AvastecimientoPlato
{
    public int IdAvastecimientoPlato { get; set; }
    public int IdUsuario { get; set; }
    public DateTime? FechaAvastecimiento { get; set; }
    public string? NroReferencia { get; set; }
    public decimal TotalAvastecimiento { get; set; }
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
    public virtual ICollection<DetalleAvastecimientoPlato> DetalleAvastecimientoPlatos { get; set; } = new List<DetalleAvastecimientoPlato>();
}
