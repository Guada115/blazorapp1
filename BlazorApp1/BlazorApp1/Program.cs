using BlazorApp1.Client.Pages;
using BlazorApp1.Components;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.Data;
using BlazorApp1.Client.Models;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, BlazorApp1.ServerAuthStateProvider>();

var provider = builder.Configuration.GetValue("DatabaseProvider", "PostgreSQL");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (provider == "SqlServer")
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"));
    }
    else
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection"));
    }
});

// Register HttpClient for server-side prerendering
builder.Services.AddHttpClient();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5131") }); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
// app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorApp1.Client._Imports).Assembly);

// Minimal API Endpoints
var api = app.MapGroup("/api");

api.MapGet("/roles", async (AppDbContext db) => await db.Roles.ToListAsync());
api.MapGet("/usuarios", async (AppDbContext db) => await db.Usuarios.ToListAsync());
api.MapGet("/conjuntos", async (AppDbContext db) => await db.Conjuntos.ToListAsync());
api.MapGet("/torres", async (AppDbContext db) => await db.Torres.ToListAsync());
api.MapGet("/apartamentos", async (AppDbContext db) => await db.Apartamentos.ToListAsync());
api.MapGet("/bitacora", async (AppDbContext db) => await db.BitacorasVigilancia.ToListAsync());
api.MapGet("/ingresos", async (AppDbContext db) => await db.Ingresos.ToListAsync());
api.MapGet("/zonascomunes", async (AppDbContext db) => await db.ZonasComunes.ToListAsync());
api.MapGet("/tiposmantenimiento", async (AppDbContext db) => await db.TiposMantenimiento.ToListAsync());
api.MapGet("/mantenimientos", async (AppDbContext db) => await db.Mantenimientos.ToListAsync());
api.MapGet("/parqueaderos", async (AppDbContext db) => await db.Parqueaderos.ToListAsync());
api.MapGet("/visitantes", async (AppDbContext db) => await db.ParqueaderosVisitantes.ToListAsync());
api.MapGet("/reservas", async (AppDbContext db) => await db.Reservas.ToListAsync());
api.MapGet("/residentesunidades", async (AppDbContext db) => await db.ResidentesUnidades.ToListAsync());

// Login endpoint
api.MapPost("/login", async (BlazorApp1.Client.Models.LoginRequest request, AppDbContext db) =>
{
    var user = await db.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email && u.Clave == request.Password && u.Activo);
    if (user != null)
    {
        // Simple token containing user info, in a real app use JWT
        var token = $"{user.UsuarioId}:{user.Email}:{user.RolId}:{user.Nombre}";
        return Results.Ok(new { Token = token, Nombre = user.Nombre, RolId = user.RolId });
    }
    return Results.Unauthorized();
});
app.Run();
