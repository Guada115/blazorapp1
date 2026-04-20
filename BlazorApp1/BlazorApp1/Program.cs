using BlazorApp1.Client.Pages;
using BlazorApp1.Components;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.Data;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorApp1.Shared.Models;
using BlazorApp1.Client.Models;
using ClosedXML.Excel;

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
api.MapPost("/zonascomunes", async (AppDbContext db, ZonaComun z) => { if (z.ZonaComunId == 0) db.ZonasComunes.Add(z); else db.ZonasComunes.Update(z); await db.SaveChangesAsync(); return Results.Ok(z); });

api.MapGet("/tiposmantenimiento", async (AppDbContext db) => await db.TiposMantenimiento.ToListAsync());
api.MapPost("/tiposmantenimiento", async (AppDbContext db, TipoMantenimiento t) => { if (t.TipoMantenimientoId == 0) db.TiposMantenimiento.Add(t); else db.TiposMantenimiento.Update(t); await db.SaveChangesAsync(); return Results.Ok(t); });

api.MapGet("/mantenimientos", async (AppDbContext db) => await db.Mantenimientos.ToListAsync());
api.MapPost("/mantenimientos", async (AppDbContext db, Mantenimiento m) => { if (m.MantenimientoId == 0) db.Mantenimientos.Add(m); else db.Mantenimientos.Update(m); await db.SaveChangesAsync(); return Results.Ok(m); });
api.MapGet("/parqueaderos", async (AppDbContext db) => await db.Parqueaderos.ToListAsync());
api.MapPost("/parqueaderos", async (AppDbContext db, Parqueadero p) => { if (p.ParqueaderoId == 0) db.Parqueaderos.Add(p); else db.Parqueaderos.Update(p); await db.SaveChangesAsync(); return Results.Ok(p); });

api.MapGet("/visitantes", async (AppDbContext db) => await db.ParqueaderosVisitantes.ToListAsync());
api.MapPost("/visitantes", async (AppDbContext db, ParqueaderoVisitante pv) => { if (pv.ParqueaderoVisitanteId == 0) db.ParqueaderosVisitantes.Add(pv); else db.ParqueaderosVisitantes.Update(pv); await db.SaveChangesAsync(); return Results.Ok(pv); });

api.MapGet("/reservas", async (AppDbContext db) => await db.Reservas.ToListAsync());
api.MapPost("/reservas", async (AppDbContext db, Reserva r) => { if (r.ReservaId == 0) db.Reservas.Add(r); else db.Reservas.Update(r); await db.SaveChangesAsync(); return Results.Ok(r); });

api.MapGet("/residentesunidades", async (AppDbContext db) => await db.ResidentesUnidades.ToListAsync());
api.MapPost("/residentesunidades", async (AppDbContext db, ResidenteUnidad ru) => { if (ru.ResidenteUnidadId == 0) db.ResidentesUnidades.Add(ru); else db.ResidentesUnidades.Update(ru); await db.SaveChangesAsync(); return Results.Ok(ru); });

api.MapGet("/export/{entidad}", async (AppDbContext db, string entidad, DateTime? inicio, DateTime? fin) =>
{
    using var workbook = new XLWorkbook();
    var worksheet = workbook.Worksheets.Add("Reporte");

    switch (entidad.ToLower())
    {
        case "conjuntos":
            var conjuntos = await db.Conjuntos.Select(c => new
            {
                ID = c.ConjuntoId,
                Nombre = c.Nombre,
                Direccion = c.Direccion,
                Ciudad = c.Ciudad,
                Nit = c.Nit,
                Telefono = c.Telefono
            }).ToListAsync();
            worksheet.Cell(1, 1).InsertTable(conjuntos);
            break;
        case "roles":
            var roles = await db.Roles.Select(r => new
            {
                ID = r.RolId,
                Nombre = r.Nombre
            }).ToListAsync();
            worksheet.Cell(1, 1).InsertTable(roles);
            break;
        case "parqueaderos":
            var parqueaderos = await db.Parqueaderos.Select(p => new
            {
                ID = p.ParqueaderoId,
                Tipo = p.Tipo,
                Numero = p.Numero,
                TorreID = p.TorreId
            }).ToListAsync();
            worksheet.Cell(1, 1).InsertTable(parqueaderos);
            break;
        case "residentes":
            var residentes = await db.ResidentesUnidades.Select(r => new
            {
                ID = r.ResidenteUnidadId,
                UsuarioID = r.UsuarioId,
                TorreID = r.TorreId,
                EsPropietario = r.EsPropietario ? "Sí" : "No"
            }).ToListAsync();
            worksheet.Cell(1, 1).InsertTable(residentes);
            break;
        case "zonascomunes":
            var zonas = await db.ZonasComunes.Select(z => new
            {
                ID = z.ZonaComunId,
                Nombre = z.Nombre,
                RequierePago = z.RequierePago ? "Sí" : "No",
                ValorHora = z.ValorHora
            }).ToListAsync();
            worksheet.Cell(1, 1).InsertTable(zonas);
            break;
        case "tiposmantenimiento":
            var tipos = await db.TiposMantenimiento.Select(t => new
            {
                ID = t.TipoMantenimientoId,
                Nombre = t.Nombre
            }).ToListAsync();
            worksheet.Cell(1, 1).InsertTable(tipos);
            break;
        case "mantenimientos":
            var queryMant = db.Mantenimientos.AsQueryable();
            if (inicio.HasValue) queryMant = queryMant.Where(m => m.Fecha >= DateOnly.FromDateTime(inicio.Value));
            if (fin.HasValue) queryMant = queryMant.Where(m => m.Fecha <= DateOnly.FromDateTime(fin.Value));
            var mant = await queryMant.Select(m => new
            {
                ID = m.MantenimientoId,
                TipoMantenimientoID = m.TipoMantenimientoId,
                Fecha = m.Fecha,
                Proveedor = m.Provider,
                Descripcion = m.Descripcion,
                Costo = m.Costo,
                ZonaComunID = m.ZonaComunId
            }).ToListAsync();
            worksheet.Cell(1, 1).InsertTable(mant);
            break;
        case "reservas":
            var queryRes = db.Reservas.AsQueryable();
            if (inicio.HasValue) queryRes = queryRes.Where(r => r.Fecha >= DateOnly.FromDateTime(inicio.Value));
            if (fin.HasValue) queryRes = queryRes.Where(r => r.Fecha <= DateOnly.FromDateTime(fin.Value));
            var reservas = await queryRes.Select(r => new
            {
                ID = r.ReservaId,
                ZonaComunID = r.ZonaComunId,
                UsuarioID = r.UsuarioId,
                Fecha = r.Fecha,
                HoraInicio = r.HoraInicio,
                HoraFin = r.HoraFin,
                Estado = r.Estado,
                TotalCobrar = r.TotalCobrar
            }).ToListAsync();
            worksheet.Cell(1, 1).InsertTable(reservas);
            break;
        case "apartamentos":
            var aptos = await (from a in db.Apartamentos
                               join t in db.Torres on a.TorreId equals t.TorreId into at
                               from t in at.DefaultIfEmpty()
                               select new
                               {
                                   ID = a.UnidadId,
                                   Numero = a.Numero,
                                   Tipo = a.Tipo,
                                   Area = a.Area,
                                   Torre = t != null ? t.Nombre : "N/A"
                               }).ToListAsync();
            worksheet.Cell(1, 1).InsertTable(aptos);
            break;
        case "visitantes":
            var queryVis = db.ParqueaderosVisitantes.AsQueryable();
            if (inicio.HasValue) queryVis = queryVis.Where(v => v.FechaHoraIngreso >= inicio.Value);
            if (fin.HasValue) queryVis = queryVis.Where(v => v.FechaHoraSalida <= fin.Value);
            var visitantes = await queryVis.Select(v => new
            {
                ID = v.ParqueaderoVisitanteId,
                Placa = v.Placa,
                Ingreso = v.FechaHoraIngreso,
                Salida = v.FechaHoraSalida,
                AsignadoA = v.ParqueaderoId
            }).ToListAsync();
            worksheet.Cell(1, 1).InsertTable(visitantes);
            break;
        case "bitacora":
            var queryBit = db.BitacorasVigilancia.AsQueryable();
            if (inicio.HasValue) queryBit = queryBit.Where(b => b.FechaHora >= inicio.Value);
            if (fin.HasValue) queryBit = queryBit.Where(b => b.FechaHora <= fin.Value);
            var bitacora = await (from b in queryBit
                                  join u in db.Usuarios on b.VigilanteId equals u.UsuarioId into bu
                                  from u in bu.DefaultIfEmpty()
                                  select new
                                  {
                                      ID = b.BitacoraId,
                                      Fecha = b.FechaHora,
                                      Observacion = b.Observacion,
                                      Vigilante = u != null ? u.Nombre : "N/A"
                                  }).ToListAsync();
            worksheet.Cell(1, 1).InsertTable(bitacora);
            break;
        case "ingresos":
             var queryIng = db.Ingresos.AsQueryable();
            if (inicio.HasValue) queryIng = queryIng.Where(i => i.FechaHoraIngreso >= inicio.Value);
            if (fin.HasValue) queryIng = queryIng.Where(i => i.FechaHoraSalida <= fin.Value);
            var ingresos = await (from i in queryIng
                                  join a in db.Apartamentos on i.ApartamentoId equals (int)a.UnidadId into ia
                                  from a in ia.DefaultIfEmpty()
                                  join t in db.Torres on i.TorreId equals t.TorreId into it
                                  from t in it.DefaultIfEmpty()
                                  select new {
                                      ID = i.IngresoId,
                                      Tipo = i.Tipo,
                                      Nombre = i.NombrePersona,
                                      Documento = i.Documento,
                                      Ingreso = i.FechaHoraIngreso,
                                      Salida = i.FechaHoraSalida,
                                      Apartamento = a != null ? a.Numero : "N/A",
                                      Torre = t != null ? t.Nombre : "N/A",
                                      Observaciones = i.Observaciones
                                  }).ToListAsync();
             worksheet.Cell(1, 1).InsertTable(ingresos);
             break;
        case "torres":
             var torres = await (from t in db.Torres
                                 join c in db.Conjuntos on t.ConjuntoId equals c.ConjuntoId into tc
                                 from c in tc.DefaultIfEmpty()
                                 select new {
                                     ID = t.TorreId,
                                     Nombre = t.Nombre,
                                     Conjunto = c != null ? c.Nombre : "N/A"
                                 }).ToListAsync();
            worksheet.Cell(1, 1).InsertTable(torres);
            break;
        case "usuarios":
            var usuarios = await (from u in db.Usuarios
                                  join r in db.Roles on (long)u.RolId equals r.RolId into ur
                                  from r in ur.DefaultIfEmpty()
                                  join a in db.Apartamentos on u.ApartamentoId equals (int)a.UnidadId into ua
                                  from a in ua.DefaultIfEmpty()
                                  select new {
                                      ID = u.UsuarioId,
                                      Nombre = u.Nombre,
                                      Email = u.Email,
                                      Documento = u.Documento,
                                      Telefono = u.Telefono,
                                      Rol = r != null ? r.Nombre : "N/A",
                                      EsPropietario = u.EsPropietario == true ? "Sí" : "No",
                                      Apartamento = a != null ? a.Numero : "N/A"
                                  }).ToListAsync();
            worksheet.Cell(1, 1).InsertTable(usuarios);
            break;
        default:
            return Results.BadRequest("Entidad no soportada para reporte.");
    }

    worksheet.Columns().AdjustToContents();
    using var stream = new MemoryStream();
    workbook.SaveAs(stream);
    stream.Position = 0;
    return Results.File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Reporte_{entidad}.xlsx");
});

// Login endpoint
api.MapPost("/login", async (BlazorApp1.Shared.Models.LoginRequest request, AppDbContext db, HttpContext context) =>
{
    var user = await db.Usuarios.FirstOrDefaultAsync(u =>
        u.Email == request.Email &&
        u.Clave == request.Password);

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
