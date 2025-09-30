namespace TipoCambioMVC.Services;

public class CambioService : CambioServiceBase, ICambioService
{
    private readonly HttpClient _http;
    public CambioService(HttpClient http) => _http = http;

    public async Task<IDictionary<string, decimal>> ObtenerTasasAsync(string baseCurrency, IEnumerable<string> favoritas, DateTime? fecha = null)
    {
        var url = BuildRatesPath(fecha, baseCurrency, favoritas);
        var resp = await _http.GetFromJsonAsync<FrankfurterRates>(url);
        return resp?.Rates ?? new Dictionary<string, decimal>();
    }

    public async Task<IDictionary<string, string>> ObtenerCatalogoMonedasAsync()
    {
        var resp = await _http.GetFromJsonAsync<Dictionary<string, string>>("currencies");
        return resp ?? new Dictionary<string, string>();
    }

    private class FrankfurterRates
    {
        public string? Base { get; set; }
        public Dictionary<string, decimal> Rates { get; set; } = new();
        public string? Date { get; set; }
    }
}
