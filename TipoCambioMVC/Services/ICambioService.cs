namespace TipoCambioMVC.Services;

public record TasasResponse(string Base, IDictionary<string, decimal> Rates, DateTime? Date);

public interface ICambioService
{
    Task<IDictionary<string, decimal>> ObtenerTasasAsync(string baseCurrency, IEnumerable<string> favoritas, DateTime? fecha = null);
    Task<IDictionary<string, string>> ObtenerCatalogoMonedasAsync();
}
