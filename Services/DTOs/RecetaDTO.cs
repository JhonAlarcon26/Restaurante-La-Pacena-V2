using System.Collections.Generic;

namespace la_paceña_v2.Services.DTOs
{
    public class RecetaDTO
    {
        public int IdPlato { get; set; }
        public string DetallesJson { get; set; }
    }

    public class DetalleRecetaDTO
    {
        public int IdInsumo { get; set; }
        public decimal Cantidad { get; set; }
    }
}
