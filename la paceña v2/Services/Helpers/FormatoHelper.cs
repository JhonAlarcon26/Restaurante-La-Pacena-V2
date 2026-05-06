namespace la_paceña_v2.Services.Helpers
{
    public static class FormatoHelper
    {
        public static string FormatearMoneda(decimal? valor)
        {
            if (!valor.HasValue) return "0";
            return valor.Value.ToString("#,##0.##");
        }

        public static string FormatearDecimal(decimal? valor)
        {
            if (!valor.HasValue) return "0";
            return valor.Value.ToString("#,##0.##");
        }
    }
}
