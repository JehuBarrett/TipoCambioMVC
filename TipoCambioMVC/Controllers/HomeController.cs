using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TipoCambioMVC.Data;
using TipoCambioMVC.Services;
using TipoCambioMVC.ViewModels;

namespace TipoCambioMVC.Controllers;

public class HomeController : Controller
{
    private readonly IUnitOfWork _uow;
    private readonly ICambioService _svc;

    public HomeController(IUnitOfWork uow, ICambioService svc)
    {
        _uow = uow;
        _svc = svc;
    }
    [Authorize(Roles = "Administrador, Normal")]
    public async Task<IActionResult> Index(DateTime? fecha, decimal? monto)
    {
        var Usuario = User.FindFirstValue(ClaimTypes.Email)
         ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
         ?? string.Empty;
        var favoritas = (await _uow.DivisasUsuario.FindAsync(x => x.IdUsuario == Usuario)).ToList();
        var Principal = favoritas.FirstOrDefault(x => x.IsPrincipal)?.Codigo;
        var BCG = string.IsNullOrWhiteSpace(Principal) ? "USD" : Principal;

        var vm = new DashboardVM
        {
            BaseCurrency = BCG,
            Fecha = fecha,
            Monto = monto ?? 1m,
            Favoritas = favoritas.Where(x => !x.Codigo.Equals(BCG)).Any() ? favoritas.Where(x => !x.Codigo.Equals(BCG)).Select(x => x.Codigo).ToList() : new List<string> { "EUR", "MXN", "JPY" }
        };
        try {
            vm.Tasas = (await _svc.ObtenerTasasAsync(vm.BaseCurrency, vm.Favoritas, vm.Fecha))
                .ToDictionary(k => k.Key, v => v.Value * vm.Monto);
        }
        catch (ExternalApiException)
        {
            TempData["Error"] = "No se pudo obtener las tasas de cambio. Intenta de nuevo más tarde.";
            vm.Tasas = new(); // fallback: tabla vacía
        }

        return View(vm);

    }
}
