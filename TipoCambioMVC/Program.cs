using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TipoCambioMVC.Data;
using TipoCambioMVC.Services;
using TipoCambioMVC.Security;
using TipoCambioMVC.Models;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// MVC
builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

// HttpClient para Frankfurter API
builder.Services.AddHttpClient<ICambioService, CambioService>(client =>
{
    client.BaseAddress = new Uri(configuration["frankfurter:Api"]);
});

// Repos y UoW
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Password hashing (Argon2/BCrypt)
builder.Services.Configure<PasswordOptions>(
    builder.Configuration.GetSection("PasswordHashing"));
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();

// Cookie Auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.LogoutPath = "/Auth/Logout";
        options.Cookie.Name = ".TipoCambio.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

// roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Administrador"));
});

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Seed mínimo de roles y admin por defecto
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await ctx.Database.MigrateAsync();

    if (!await ctx.Roles.AnyAsync(r => r.TipoRol == "Administrador"))
        ctx.Roles.Add(new Rol { TipoRol = "Administrador" });

    if (!await ctx.Roles.AnyAsync(r => r.TipoRol == "Normal"))
        ctx.Roles.Add(new Rol { TipoRol = "Normal" });

    await ctx.SaveChangesAsync();

    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();
    var adminEmail = "admin@tipocambio.local";

    if (!await ctx.Usuarios.AnyAsync(u => u.IdUsuario == adminEmail))
    {
        ctx.Usuarios.Add(new Usuario
        {
            IdUsuario = adminEmail,
            Nombre = "Admin",
            Contrasena = hasher.Hash("Admin123!"),
            IdRol = (await ctx.Roles.FirstAsync(r => r.TipoRol == "Administrador")).IdRol
        });
        await ctx.SaveChangesAsync();
    }
}

app.Run();

