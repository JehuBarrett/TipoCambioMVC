using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TipoCambioMVC.Data;
using TipoCambioMVC.Models;
using TipoCambioMVC.Services;

namespace TipoCambioMVC.Controllers;

public class MonedasController : Controller
{
    private readonly IUnitOfWork _uow;
    private readonly ICambioService _svc;

    public MonedasController(IUnitOfWork uow, ICambioService svc)
    {
        _uow = uow;
        _svc = svc;
    }

    [Authorize(Roles = "Administrador, Normal")]
    public async Task<IActionResult> Catalogo()
    {
        var catalogo = await _svc.ObtenerCatalogoMonedasAsync();
        return View(catalogo);
    }

    [Authorize(Roles = "Administrador, Normal")]
    public async Task<IActionResult> Favoritas()
    {
        var Usuario = User.FindFirstValue(ClaimTypes.Email)
                      ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? string.Empty;
        var list = (await _uow.DivisasUsuario.FindAsync(x => x.IdUsuario == Usuario)).ToList();
        return View(list);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador, Normal")]
    public async Task<IActionResult> AgregarFavorita(string codigo, bool esPrincipal = false)
    {
        int id = 0;
        var Usuario = User.FindFirstValue(ClaimTypes.Email)
                      ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? string.Empty;

        if (string.IsNullOrWhiteSpace(codigo)) return RedirectToAction(nameof(Favoritas));
        if (esPrincipal)
        {
            var actuales = (await _uow.DivisasUsuario.FindAsync(x => x.IdUsuario == Usuario)).ToList();
            foreach (var d in actuales)
            {
                d.IsPrincipal = false;
                _uow.DivisasUsuario.Update(d);
                if (d.Codigo == codigo) id = d.Id;
            };
            await _uow.SaveChangesAsync();
        }

        if (id > 0)
        {
            var divisa = await _uow.DivisasUsuario.GetByIdAsync(id);
            if (divisa != null)
            {
                divisa.IsPrincipal = esPrincipal;
                _uow.DivisasUsuario.Update(divisa);
            }
        }
        else
        {
            await _uow.DivisasUsuario.AddAsync(new DivisaUsuario { IdUsuario = Usuario, Codigo = codigo, IsPrincipal = esPrincipal });
        }
        await _uow.SaveChangesAsync();
        return RedirectToAction(nameof(Favoritas));
    }

    [HttpPost]
    [Authorize(Roles = "Administrador, Normal")]
    public async Task<IActionResult> EliminarFavorita(int id)
    {
        var item = await _uow.DivisasUsuario.GetByIdAsync(id);
        if (item != null)
        {
            _uow.DivisasUsuario.Remove(item);
            await _uow.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Favoritas));
    }
}
