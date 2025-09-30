using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TipoCambioMVC.Data;
using TipoCambioMVC.Models.Auth;
using TipoCambioMVC.Models;
using TipoCambioMVC.Security;

namespace TipoCambioMVC.Controllers
{

    public class AuthController : Controller
    {
        private readonly AppDbContext _ctx;
        private readonly IPasswordHasherService _hasher;

        public AuthController(AppDbContext ctx, IPasswordHasherService hasher)
        { _ctx = ctx; _hasher = hasher; }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null) { ViewData["ReturnUrl"] = returnUrl; return View(new LoginVM()); }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM vm, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _ctx.Usuarios.Include(u => u.Rol).FirstOrDefaultAsync(u => u.IdUsuario == vm.Email);
            if (user is null || !_hasher.Verify(vm.Password, user.Contrasena))
            { ModelState.AddModelError(string.Empty, "Credenciales inválidas."); return View(vm); }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.IdUsuario),
            new Claim(ClaimTypes.Name, user.Nombre),
            new Claim(ClaimTypes.Email, user.IdUsuario),
            new Claim(ClaimTypes.Role, user.Rol.TipoRol) // "Administrador" o "Normal"
        };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = vm.RememberMe,
                    AllowRefresh = true,
                    ExpiresUtc = vm.RememberMe ? DateTimeOffset.UtcNow.AddDays(14) : DateTimeOffset.UtcNow.AddHours(2)
                });

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet] public IActionResult Register() => View(new RegisterVM());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            if (await _ctx.Usuarios.AnyAsync(u => u.IdUsuario == vm.Email))
            { ModelState.AddModelError(nameof(vm.Email), "El email ya está registrado."); return View(vm); }

            var rolNormal = await _ctx.Roles.FirstOrDefaultAsync(r => r.TipoRol == "Normal");
            if (rolNormal is null) { rolNormal = new Rol { TipoRol = "Normal" }; _ctx.Roles.Add(rolNormal); await _ctx.SaveChangesAsync(); }

            var hash = _hasher.Hash(vm.Password);
            var user = new Usuario { IdUsuario = vm.Email, Nombre = vm.Nombre, Contrasena = hash, IdRol = rolNormal.IdRol };
            _ctx.Usuarios.Add(user);
            await _ctx.SaveChangesAsync();

            return await Login(new LoginVM { Email = vm.Email, Password = vm.Password, RememberMe = true });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        { await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); return RedirectToAction("Index", "Home"); }

        [HttpGet] public IActionResult AccessDenied() => View();
    }

}
