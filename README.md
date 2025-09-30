# Calculadora de Tipo de Cambio (Starter)

Proyecto base ASP.NET Core MVC + EF Core + Bootstrap para consumir la API de tipos de cambio de Frankfurter.

## Requisitos
- .NET 8 SDK
- SQL Server (local o Docker)
- EF Core Tools: `dotnet tool install --global dotnet-ef`

## Configuración
1. Actualiza la cadena de conexión en `TipoCambioMVC/appsettings.json`.
2. Desde la carpeta `TipoCambioMVC/` aplicar migraciones:
   ```bash
   dotnet ef migrations add Init
   dotnet ef database update
   ```
3. Ejecuta:
   ```bash
   dotnet run --project TipoCambioMVC/TipoCambioMVC.csproj
   ```

## Estructura
- `Models/`: Entidades (`Moneda`, `DivisaFavorita`, `BitacoraPeticion`).
- `Data/`: `AppDbContext`, repositorios genéricos y `UnitOfWork`.
- `Services/`: `ICambioService` y `CambioService` (HttpClient hacia `https://api.frankfurter.dev/`).
- `Filters/`: `LogActionFilter` para bitácora de peticiones.
- `Controllers/`: `HomeController` (Dashboard), `MonedasController` (catálogo y favoritas).
- `ViewModels/`: `DashboardVM`.
- `Views/`: Razor con Bootstrap.

## Notas
- El filtro de bitácora guarda cada petición con método, ruta e IP.
- El Dashboard permite decidir moneda base, fecha e importe. Muestra conversiones a favoritas.
- Para autenticación y roles, puedes agregar Identity más adelante.
