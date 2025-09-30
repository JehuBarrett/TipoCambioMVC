using System.Text.Json;
using System.Net.Http.Json;

namespace TipoCambioMVC.Services;

public class CambioService : CambioServiceBase, ICambioService
{
    private readonly HttpClient _http;
    public CambioService(HttpClient http) => _http = http;

    public async Task<IDictionary<string, decimal>> ObtenerTasasAsync(string baseCurrency, IEnumerable<string> favoritas, DateTime? fecha = null)
    {
        try
        {
            var url = BuildRatesPath(fecha, baseCurrency, favoritas);
            var resp = await _http.GetFromJsonAsync<FrankfurterRates>(url);
            return resp?.Rates ?? new Dictionary<string, decimal>();
        }
        catch (TaskCanceledException ex)
        {
            throw new ExternalApiException("Tiempo de espera agotado al consultar Frankfurter.", ex);
        }
        // MÁS GENERAL después: cancelación cooperativa (shutdown, token, etc.)
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            throw new ExternalApiException("Error de red al consultar Frankfurter.", ex);
        }
        catch (NotSupportedException ex)
        {
            throw new ExternalApiException("Contenido no soportado en la respuesta de Frankfurter.", ex);
        }
        catch (JsonException ex)
        {
            throw new ExternalApiException("No se pudo interpretar la respuesta de Frankfurter.", ex);
        }

    }

    public async Task<IDictionary<string, string>> ObtenerCatalogoMonedasAsync()
    {
        try
        {
            var resp = await _http.GetFromJsonAsync<Dictionary<string, string>>("currencies");
            return resp ?? new Dictionary<string, string>();
        }
        catch (TaskCanceledException ex)
        {
            throw new ExternalApiException("Tiempo de espera agotado al consultar el catálogo.", ex);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            throw new ExternalApiException("Error de red al consultar el catálogo.", ex);
        }
        catch (NotSupportedException ex)
        {
            throw new ExternalApiException("Contenido no soportado al consultar el catálogo.", ex);
        }
        catch (JsonException ex)
        {
            throw new ExternalApiException("No se pudo interpretar el catálogo de monedas.", ex);
        }

    }

    private class FrankfurterRates
    {
        public string? Base { get; set; }
        public Dictionary<string, decimal> Rates { get; set; } = new();
        public string? Date { get; set; }
    }
}
