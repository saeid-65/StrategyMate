using Microsoft.EntityFrameworkCore;
using StrategyMate.Components;
using StrategyMate.Data;
using StrategyMate.Services;

var builder = WebApplication.CreateBuilder(args);

// ========================================================================
// Blazor services
// ========================================================================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ========================================================================
// SQLite database (reference data: symbols, industry groups, sub-groups)
// ========================================================================
// In Development the database lives inside the project folder under
// Data/Databases/ so it sits next to the schema script and stays under
// version control (if desired). ContentRootPath points at the project
// folder, NOT bin/, so `dotnet clean` / `dotnet build` will never touch it.
//
// In Production we move it to a stable per-user location so that
// re-publishing or relocating the app folder cannot destroy user data.
string sqlitePath;

if (builder.Environment.IsDevelopment())
{
    // {ProjectRoot}/Data/Databases/strategy-mate.db
    sqlitePath = Path.Combine(
        builder.Environment.ContentRootPath,
        "Data",
        "Databases",
        "strategy-mate.db");
}
else
{
    // Linux:   ~/.local/share/StrategyMate/strategy-mate.db
    // Windows: %APPDATA%\StrategyMate\strategy-mate.db
    // macOS:   ~/Library/Application Support/StrategyMate/strategy-mate.db
    var appData = Environment.GetFolderPath(
        Environment.SpecialFolder.LocalApplicationData);

    var dataDir = Path.Combine(appData, "StrategyMate");
    Directory.CreateDirectory(dataDir);

    sqlitePath = Path.Combine(dataDir, "strategy-mate.db");
}

// Ensure the parent directory exists (relevant for the dev path).
Directory.CreateDirectory(Path.GetDirectoryName(sqlitePath)!);

// A DbContextFactory (not a scoped DbContext) is registered so that
// multiple Blazor circuits can safely create their own short-lived
// contexts concurrently.
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlite($"Data Source={sqlitePath};Foreign Keys=True"));


// ========================================================================
// Application services
// ========================================================================
// Read-only access to the reference data tables.
builder.Services.AddScoped<ISymbolService, SymbolService>();

// ========================================================================
// Build the application
// ========================================================================
var app = builder.Build();

// ========================================================================
// Ensure database schemas exist on startup
// ========================================================================
// We deliberately use a hand-written SQL script (Data/sqlite-schema.sql)
// rather than EF Core's EnsureCreated, because the script defines:
//   - TEXT timestamps (SQLite has no real DATE/TIMESTAMP type),
//   - specific indexes with explicit names,
//   - an AFTER UPDATE trigger that maintains UPDATED_AT,
//   - foreign keys with ON DELETE RESTRICT.
// EF Core's EnsureCreated would produce a slightly different schema and
// miss the trigger.
using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider
        .GetRequiredService<IDbContextFactory<AppDbContext>>();

    await using var ctx = await dbFactory.CreateDbContextAsync();

    var scriptPath = Path.Combine(
        builder.Environment.ContentRootPath,
        "Data",
        "sqlite-schema.sql");

    if (File.Exists(scriptPath))
    {
        var sql = await File.ReadAllTextAsync(scriptPath);

        // SQLite's ADO.NET provider executes multiple statements in a
        // single ExecuteSqlRaw call, which is exactly what we need.
        await ctx.Database.ExecuteSqlRawAsync(sql);
    }
    else
    {
        // Fallback only if the script is missing. This will produce a
        // slightly different schema (no trigger, no explicit index names).
        await ctx.Database.EnsureCreatedAsync();
    }
}

// ========================================================================
// HTTP request pipeline
// ========================================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();

// Serve static assets (CSS, JS, images) from wwwroot.
app.MapStaticAssets();

// Map the Blazor root component and enable interactive Server rendering.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();