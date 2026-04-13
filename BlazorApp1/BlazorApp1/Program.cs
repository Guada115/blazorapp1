using BlazorApp1.Client.Pages;
using BlazorApp1.Components;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.Data;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorApp1.Shared.Models;
using BlazorApp1.Client.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.AddAuthorization();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));

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
    .AddAdditionalAssemblies(typeof(BlazorApp1.Client._Imports).Assembly)
    .AllowAnonymous();

// Minimal API Endpoints
var api = app.MapGroup("/api").DisableAntiforgery();

api.MapGet("/roles", async (AppDbContext db) => await db.Roles.ToListAsync());
api.MapPost("/roles", async (AppDbContext db, Rol rol) => { if (rol.RolId == 0) db.Roles.Add(rol); else db.Roles.Update(rol); await db.SaveChangesAsync(); return Results.Ok(rol); });

api.MapGet("/usuarios", async (AppDbContext db) => await db.Usuarios.ToListAsync());
api.MapPost("/usuarios", async (AppDbContext db, Usuario usuario) => { if (usuario.UsuarioId == 0) db.Usuarios.Add(usuario); else db.Usuarios.Update(usuario); await db.SaveChangesAsync(); return Results.Ok(usuario); });

api.MapGet("/conjuntos", async (AppDbContext db) => await db.Conjuntos.ToListAsync());
api.MapPost("/conjuntos", async (AppDbContext db, Conjunto conjunto) => { if (conjunto.ConjuntoId == 0) db.Conjuntos.Add(conjunto); else db.Conjuntos.Update(conjunto); await db.SaveChangesAsync(); return Results.Ok(conjunto); });

api.MapGet("/torres", async (AppDbContext db) => await db.Torres.ToListAsync());
api.MapPost("/torres", async (AppDbContext db, Torre t) => { if (t.TorreId == 0) db.Torres.Add(t); else db.Torres.Update(t); await db.SaveChangesAsync(); return Results.Ok(t); });

api.MapGet("/apartamentos", async (AppDbContext db) => await db.Apartamentos.ToListAsync());
api.MapPost("/apartamentos", async (AppDbContext db, Apartamento a) => { if (a.UnidadId == 0) db.Apartamentos.Add(a); else db.Apartamentos.Update(a); await db.SaveChangesAsync(); return Results.Ok(a); });

api.MapGet("/bitacora", async (AppDbContext db) => await db.BitacorasVigilancia.ToListAsync());
api.MapPost("/bitacora", async (AppDbContext db, BitacoraVigilancia b) => { if (b.BitacoraId == 0) db.BitacorasVigilancia.Add(b); else db.BitacorasVigilancia.Update(b); await db.SaveChangesAsync(); return Results.Ok(b); });

api.MapGet("/ingresos", async (AppDbContext db) => await db.Ingresos.ToListAsync());
api.MapPost("/ingresos", async (AppDbContext db, Ingreso i) => { if (i.IngresoId == 0) db.Ingresos.Add(i); else db.Ingresos.Update(i); await db.SaveChangesAsync(); return Results.Ok(i); });
api.MapGet("/zonascomunes", async (AppDbContext db) => await db.ZonasComunes.ToListAsync());
api.MapGet("/tiposmantenimiento", async (AppDbContext db) => await db.TiposMantenimiento.ToListAsync());
api.MapGet("/mantenimientos", async (AppDbContext db) => await db.Mantenimientos.ToListAsync());
api.MapGet("/parqueaderos", async (AppDbContext db) => await db.Parqueaderos.ToListAsync());
api.MapGet("/visitantes", async (AppDbContext db) => await db.ParqueaderosVisitantes.ToListAsync());
api.MapGet("/reservas", async (AppDbContext db) => await db.Reservas.ToListAsync());

api.MapGet("/residentesunidades", async (AppDbContext db) => await db.ResidentesUnidades.ToListAsync());
api.MapPost("/residentesunidades", async (AppDbContext db, ResidenteUnidad ru) => { if (ru.ResidenteUnidadId == 0) db.ResidentesUnidades.Add(ru); else db.ResidentesUnidades.Update(ru); await db.SaveChangesAsync(); return Results.Ok(ru); });


// Login endpoint
api.MapPost("/login", async (BlazorApp1.Shared.Models.LoginRequest request, AppDbContext db, HttpContext context) =>
{
    var user = await db.Usuarios.FirstOrDefaultAsync(u =>
        u.Email == request.Email &&
        u.Clave == request.Password &&
        u.Activo);

    if (user != null)
    {
        // 1) Generar el Cookie para que la vista del Servidor no bloquee al cliente
        var claims = new List<System.Security.Claims.Claim> {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.UsuarioId.ToString()),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Nombre),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.RolId.ToString())
        };
        var identity = new System.Security.Claims.ClaimsIdentity(claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
        await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignInAsync(
            context, 
            Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, 
            new System.Security.Claims.ClaimsPrincipal(identity));

        // 2) Generar el token (para el StateProvider del WASM)
        var token = $"{user.UsuarioId}:{user.Email}:{user.RolId}:{user.Nombre}";
        return Results.Ok(new
        {
            Token = token,
            Nombre = user.Nombre,
            RolId = user.RolId
        });
    }

    return Results.Unauthorized();
});
app.Run();
