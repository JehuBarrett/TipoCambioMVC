# TipoCambioMVC — Calculadora de tipo de cambio (ASP.NET Core MVC 6)

Aplicación MVC que consume **Frankfurter** para:

- gestionar divisas favoritas por usuario,
- definir divisa principal,
- ver dashboard con conversiones actuales,
- consultar historial,
- y convertir montos.

Incluye: autenticación por **cookies**, **roles** (Administrador/Normal), hashing con **Argon2id** o **BCrypt** (configurable), **Repository + UnitOfWork**, **Action Filter** para bitácora y **EF Core** (SQL Server) con migraciones.

---

## Requisitos

- .NET 6 SDK  
- SQL Server (LocalDB/Express/Developer)

---

## Pasos previos
- Ejecutar el script TipoCambioDB.sql para crear la base de datos y las tablas
- Abrir proyecto con Visual Studio

---

## Configuración rápida

### Connection string  
En `appsettings.json` ajusta:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TipoCambio;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Hashing de contraseñas (elige algoritmo)

```json
{
  "PasswordHashing": {
    "Algorithm": "Argon2id",             // o "BCrypt"
    "BCrypt":   { "WorkFactor": 12 },
    "Argon2id": { "Iterations": 3, "MemoryKb": 65536, "Parallelism": 2, "SaltSize": 16 }
  }
}
```


### Cliente HTTP (Frankfurter)  
Ya configurado en `appsettings.json`:

```json
  "frankfurter": {
    "Api": "https://api.frankfurter.dev/v1/"
  }

```

En `Program.cs` se hace seed de roles (“Administrador”, “Normal”) y, si no existe, un admin por defecto:  
**Usuario:** `admin@tipocambio.local` — **Contraseña:** `Admin123!`.

---

## Ejecutar

### Visual Studio
F5 (IIS Express/Kestrel). Anota el puerto HTTPS (p. ej. `https://localhost:59045`).

### CLI
```bash
dotnet run
```

---

## Rutas principales

- Registro: `/Auth/Register`  
- Login: `/Auth/Login`  
- Logout (POST): `/Auth/Logout`  
- Dashboard/Home: `/`  
- Catálogo de divisas: `/Monedas/Catalogo`  
- Favoritas: `/Monedas/Favoritas`


Endpoints sensibles se protegen con `[Authorize]` y para admin con `[Authorize(Roles="Administrador")]`.

---

## Estructura del proyecto

```
Controllers/
  AuthController.cs   // registro/login/logout
  HomeController.cs   // dashboard
  MonedasController.cs// catálogo/favoritas
Data/
  AppDbContext.cs, Repository<T>.cs, UnitOfWork.cs, IUnitOfWork.cs
Models/
  Auth/ 
  DivisaUsuario.cs, Rol.cs, Usuario.cs
Security/
  IPasswordHasherService.cs, PasswordHasherService.cs, PasswordOptions.cs
Services/
  ICambioService.cs, CambioService.cs  // consumo API Frankfurter
ViewModels/
  DashboardVM.cs      // modelos de vista (dashboard, etc.)
Views/
  Auth/ Login.cshtml, Register.cshtml, AccessDenied.cshtml
  Home/ Index.cshtml
  Monedas/ Catalogo.cshtml, Favoritas.cshtml
  Shared/ _Layout.cshtml, _LoginPartial.cshtml, _ValidationScriptsPartial.cshtml, _ViewImports.cshtml, _ViewStart.cshtml
wwwroot/
```

---

## Validación del lado cliente (Razor)

El parcial `Views/Shared/_ValidationScriptsPartial.cshtml` ya está en el repo.  
Puedes usar **CDN** (recomendado) o **LibMan** para archivos locales.

**CDN (contenido sugerido):**
```html
<script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/jquery-validation@1.19.5/dist/jquery.validate.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/jquery-validation-unobtrusive@4.0.0/dist/jquery.validate.unobtrusive.min.js"></script>
```

En cada vista con formulario incluye:
```cshtml
@section Scripts { <partial name="_ValidationScriptsPartial" /> }
```

---

## Notas técnicas

- **Autenticación:** Cookies configuradas en `Program.cs` (HttpOnly, SameSite=Strict, SecurePolicy=Always, expiración deslizante).
- **Hashing:** `Security/PasswordHasherService` (Argon2id = HybridAddressing en Isopoh, o BCrypt).
- **Repository + UoW:** en `Data/`.
- **EF Core mapping:** `Rol` está mapeado a la tabla **Rol** (singular) para evitar el error “Invalid object name 'Roles'”.

---

## Troubleshooting

- **404 en /Auth/Login o /Auth/Register**  
  Verifica que `AuthController` y las vistas existan; revisa el puerto real.

- **“Invalid object name 'Roles'”**  
  Asegúrate de mapear `Rol` a la tabla `Rol` (Fluent o `[Table("Rol")]`) y que tu connection string apunte a la BD correcta.

- **_ValidationScriptsPartial no encontrado**  
  Crea `Views/Shared/_ValidationScriptsPartial.cshtml` (usa CDN de arriba).

- **No se guardan cambios al marcar principal**  
  Si leíste con `AsNoTracking()`, adjunta con `Update(...)` o usa actualización parcial.

- **Problemas con Argon2**  
  En Isopoh usa `Argon2Type.HybridAddressing`. Si cambia la firma, usa la variante con `Argon2Config`.

---

## Licencia / Créditos

- Tipos de cambio: **Frankfurter** (https://frankfurter.dev/)
