using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TipoCambioMVC.Data;
using TipoCambioMVC.Models.Auth;
using TipoCambioMVC.Models;
using TipoCambioMVC.Security;
using Microsoft.Data.SqlClient;

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
            { ModelState.AddModelError(string.Empty, "Correo y/o contraseña incorrectos."); return View(vm); }

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
            {
                ModelState.AddModelError(nameof(vm.Email), "El email ya está registrado.");
                return View(vm);
            }

            try
            {
                var rolNormal = await _ctx.Roles.FirstOrDefaultAsync(r => r.TipoRol == "Normal")
                                ?? (await _ctx.Roles.AddAsync(new Rol { TipoRol = "Normal" })).Entity;

                var hash = _hasher.Hash(vm.Password);
                _ctx.Usuarios.Add(new Usuario { IdUsuario = vm.Email, Nombre = vm.Nombre, Contrasena = hash, IdRol = rolNormal.IdRol });
                await _ctx.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
            {
                // Si alguien registró el mismo email justo antes de tu SaveChanges
                ModelState.AddModelError(nameof(vm.Email), "El email ya está registrado.");
                return View(vm);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "No se pudo completar el registro. Intenta más tarde.");
                return View(vm);
            }

            // Autologin
            return await Login(new LoginVM { Email = vm.Email, Password = vm.Password, RememberMe = true });
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        { await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); return RedirectToAction("Index", "Home"); }

        [HttpGet] public IActionResult AccessDenied() => View();
    }

}
