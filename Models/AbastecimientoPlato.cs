namespace la_paceña_v2.Models;

public partial class AbastecimientoPlato
{
    public int IdAbastecimientoPlato { get; set; }
    public int IdUsuario { get; set; }
    public DateTime? FechaAbastecimiento { get; set; }
    public string? NroReferencia { get; set; }
    public decimal TotalAbastecimiento { get; set; }
    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
    public virtual ICollection<DetalleAbastecimientoPlato> DetalleAbastecimientoPlatos { get; set; } = new List<DetalleAbastecimientoPlato>();
}
