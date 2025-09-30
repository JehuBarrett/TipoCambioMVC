namespace TipoCambioMVC.Services
{
    //Centraliza utilidades de dominio para el servicio de cambio
    public abstract class CambioServiceBase
    {
        protected static string NormalizeCode(string? code)
            => string.IsNullOrWhiteSpace(code) ? string.Empty : code.Trim().ToUpperInvariant();

        protected static IEnumerable<string> NormalizeCodes(IEnumerable<string>? codes)
            => (codes ?? Enumerable.Empty<string>())
               .Where(c => !string.IsNullOrWhiteSpace(c))
               .Select(c => c.Trim().ToUpperInvariant())
               .Distinct();

        protected static string BuildRatesPath(DateTime? fecha, string baseCurrency, IEnumerable<string> favoritas)
        {
            var datePart = fecha.HasValue ? fecha.Value.ToString("yyyy-MM-dd") : "latest";
            var @base = NormalizeCode(baseCurrency);
            var to = string.Join(",", NormalizeCodes(favoritas));
            return string.IsNullOrEmpty(to)
                ? $"{datePart}?from={@base}"
                : $"{datePart}?from={@base}&to={to}";
        }
    }
}
