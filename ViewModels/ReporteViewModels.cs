using System;
using la_paceña_v2.Services.DTOs;

namespace la_paceña_v2.Models.ViewModels
{
    public class FiltroReporteVentasViewModel
    {
        public DateTime FechaInicio { get; set; } = DateTime.Today;
        public DateTime FechaFin { get; set; } = DateTime.Today;
        public int? IdSucursal { get; set; }
        public int? IdUsuario { get; set; }
        public ReporteVentasDTO? Reporte { get; set; }
    }

    public class FiltroReporteComprasViewModel
    {
        public DateTime FechaInicio { get; set; } = DateTime.Today;
        public DateTime FechaFin { get; set; } = DateTime.Today;
        public int? IdProveedor { get; set; }
        public ReporteComprasDTO? Reporte { get; set; }
    }
}
