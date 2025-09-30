namespace TipoCambioMVC.ViewModels;

public class DashboardVM
{
    public string BaseCurrency { get; set; } = "USD";
    public DateTime? Fecha { get; set; }
    public decimal Monto { get; set; } = 1m;
    public List<string> Favoritas { get; set; } = new();
    public Dictionary<string, decimal> Tasas { get; set; } = new();
}
