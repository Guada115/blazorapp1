using BlazorApp1.Client.Pages;
using BlazorApp1.Components;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.Data;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorApp1.Shared.Models;
using BlazorApp1.Client.Models;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
QuestPDF.Settings.License = LicenseType.Community;
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
api.MapPost("/apartamentos", async (AppDbContext db, Apartamento a) => { if (a.ApartamentoId == 0) db.Apartamentos.Add(a); else db.Apartamentos.Update(a); await db.SaveChangesAsync(); return Results.Ok(a); });

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

api.MapGet("/cuartosutil", async (AppDbContext db) => await db.CuartosUtil.ToListAsync());
api.MapPost("/cuartosutil", async (AppDbContext db, CuartoUtil cu) =>
{
    if (cu.ApartamentoId.HasValue)
    {
        var apto = await db.Apartamentos.FindAsync(cu.ApartamentoId.Value);
        if (apto != null)
        {
            int maxPermitidos = apto.Tipo.ToLower() == "familiar" ? 2 : 1;
            int count = await db.CuartosUtil.CountAsync(c => c.ApartamentoId == cu.ApartamentoId && c.CuartoUtilId != cu.CuartoUtilId);
            if (count >= maxPermitidos)
                return Results.BadRequest($"Apartamento tipo '{apto.Tipo}' solo permite {maxPermitidos} cuarto(s) util(es).");
        }
    }
    if (cu.CuartoUtilId == 0) db.CuartosUtil.Add(cu); else db.CuartosUtil.Update(cu);
    await db.SaveChangesAsync();
    return Results.Ok(cu);
});

api.MapGet("/reservas", async (AppDbContext db) => await db.Reservas.ToListAsync());
api.MapPost("/reservas", async (AppDbContext db, Reserva r) =>
{
    var conflicto = await db.Reservas.AnyAsync(x =>
        x.ReservaId != r.ReservaId &&
        x.ZonaComunId == r.ZonaComunId &&
        x.Fecha == r.Fecha &&
        x.HoraInicio < r.HoraFin &&
        x.HoraFin > r.HoraInicio);
    if (conflicto)
        return Results.BadRequest("Ya existe una reserva para esa zona común en ese horario.");
    if (r.ReservaId == 0) db.Reservas.Add(r); else db.Reservas.Update(r);
    await db.SaveChangesAsync();
    return Results.Ok(r);
});

api.MapGet("/residentesunidades", async (AppDbContext db) => await db.ResidentesUnidades.ToListAsync());
api.MapPost("/residentesunidades", async (AppDbContext db, ResidenteUnidad ru) => { if (ru.ResidenteUnidadId == 0) db.ResidentesUnidades.Add(ru); else db.ResidentesUnidades.Update(ru); await db.SaveChangesAsync(); return Results.Ok(ru); });

api.MapGet("/export/{entidad}", async (AppDbContext db, string entidad, DateTime? inicio, DateTime? fin, int? rolId, int? conjuntoId, int? torreId, int? tipoMantenimientoId, int? zonaComunId, string? tipoIngreso, string? tipoPar, long? apartamentoIdPar, string? tipoVis, int? conjuntoVis, string? estadoCU, int? conjuntoCU, string? ocupadoVis) =>
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
            var qPark = db.Parqueaderos.AsQueryable();
            if (!string.IsNullOrEmpty(tipoPar)) qPark = qPark.Where(p => p.Tipo == tipoPar);
            if (apartamentoIdPar.HasValue && apartamentoIdPar.Value != 0) qPark = qPark.Where(p => p.ApartamentoId == apartamentoIdPar.Value);
            var parqueaderos = await (from p in qPark
                                      join a in db.Apartamentos on p.ApartamentoId equals a.ApartamentoId into pa from a in pa.DefaultIfEmpty()
                                      join t in db.Torres on p.TorreId equals t.TorreId into pt from t in pt.DefaultIfEmpty()
                                      join c in db.Conjuntos on p.ConjuntoId equals (long?)c.ConjuntoId into pc from c in pc.DefaultIfEmpty()
                                      select new { ID = p.ParqueaderoId, Tipo = p.Tipo, Apartamento = a != null ? a.Numero : "N/A", Torre = t != null ? t.Nombre : "N/A", Conjunto = c != null ? c.Nombre : "N/A" }).ToListAsync();
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
            if (tipoMantenimientoId.HasValue && tipoMantenimientoId.Value != 0) queryMant = queryMant.Where(m => m.TipoMantenimientoId == tipoMantenimientoId.Value);
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
            if (zonaComunId.HasValue && zonaComunId.Value != 0) queryRes = queryRes.Where(r => r.ZonaComunId == zonaComunId.Value);
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
            var queryApto = db.Apartamentos.AsQueryable();
            if (conjuntoId.HasValue && conjuntoId.Value != 0) queryApto = queryApto.Where(a => a.ConjuntoId == conjuntoId.Value);
            if (torreId.HasValue && torreId.Value != 0) queryApto = queryApto.Where(a => a.TorreId == torreId.Value);
            var aptos = await (from a in queryApto
                               join t in db.Torres on a.TorreId equals t.TorreId into at
                               from t in at.DefaultIfEmpty()
                               join c in db.Conjuntos on (long?)a.ConjuntoId equals c.ConjuntoId into ac
                               from c in ac.DefaultIfEmpty()
                               select new
                               {
                                   ID = a.ApartamentoId,
                                   Numero = a.Numero,
                                   Tipo = a.Tipo,
                                   Conjunto = c != null ? c.Nombre : "N/A",
                                   Torre = t != null ? t.Nombre : "N/A"
                               }).ToListAsync();
            worksheet.Cell(1, 1).InsertTable(aptos);
            break;
        case "visitantes":
            var queryVis = db.ParqueaderosVisitantes.AsQueryable();
            if (inicio.HasValue) queryVis = queryVis.Where(v => v.FechaHoraIngreso >= inicio.Value);
            if (fin.HasValue) queryVis = queryVis.Where(v => v.FechaHoraSalida <= fin.Value);
            if (!string.IsNullOrEmpty(tipoVis)) queryVis = queryVis.Where(v => v.Tipo == tipoVis);
            if (conjuntoVis.HasValue && conjuntoVis.Value != 0) queryVis = queryVis.Where(v => v.ConjuntoId == conjuntoVis.Value);
            if (ocupadoVis == "si") queryVis = queryVis.Where(v => v.Ocupado);
            else if (ocupadoVis == "no") queryVis = queryVis.Where(v => !v.Ocupado);
            var visitantes = await queryVis.Select(v => new
            {
                ID = v.ParqueaderoVisitanteId,
                Tipo = v.Tipo,
                Placa = v.Placa,
                Ingreso = v.FechaHoraIngreso,
                Salida = v.FechaHoraSalida,
                AsignadoA = v.ApartamentoId,
                Ocupado = v.Ocupado ? "Sí" : "No"
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
            if (!string.IsNullOrEmpty(tipoIngreso)) queryIng = queryIng.Where(i => i.Tipo == tipoIngreso);
            var ingresos = await (from i in queryIng
                                  join a in db.Apartamentos on i.ApartamentoId equals (int)a.ApartamentoId into ia
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
            var queryUsr = db.Usuarios.AsQueryable();
            if (rolId.HasValue && rolId.Value != 0) queryUsr = queryUsr.Where(u => u.RolId == rolId.Value);
            var usuarios = await (from u in queryUsr
                                  join r in db.Roles on (long)u.RolId equals r.RolId into ur
                                  from r in ur.DefaultIfEmpty()
                                  join a in db.Apartamentos on u.ApartamentoId equals (int)a.ApartamentoId into ua
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
        case "cuartosutil":
            var qCU = db.CuartosUtil.AsQueryable();
            if (!string.IsNullOrEmpty(estadoCU)) qCU = qCU.Where(c => c.Estado == estadoCU);
            if (conjuntoCU.HasValue && conjuntoCU.Value != 0) qCU = qCU.Where(c => c.ConjuntoId == conjuntoCU.Value);
            var cuartos = await (from c in qCU
                                 join a in db.Apartamentos on c.ApartamentoId equals a.ApartamentoId into ca
                                 from a in ca.DefaultIfEmpty()
                                 join t in db.Torres on c.TorreId equals (long?)t.TorreId into ct
                                 from t in ct.DefaultIfEmpty()
                                 join co in db.Conjuntos on c.ConjuntoId equals (long?)co.ConjuntoId into cc
                                 from co in cc.DefaultIfEmpty()
                                 select new {
                                     ID = c.CuartoUtilId,
                                     Numero = c.CuartoUtilNumero,
                                     Estado = c.Estado,
                                     Apartamento = a != null ? a.Numero : "N/A",
                                     Torre = t != null ? t.Nombre : "N/A",
                                     Conjunto = co != null ? co.Nombre : "N/A"
                                 }).ToListAsync();
            worksheet.Cell(1, 1).InsertTable(cuartos);
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

api.MapGet("/export/pdf/{entidad}", async (AppDbContext db, string entidad, DateTime? inicio, DateTime? fin, int? rolId, int? conjuntoId, int? torreId, int? tipoMantenimientoId, int? zonaComunId, string? tipoIngreso, string? tipoPar, long? apartamentoIdPar, string? tipoVis, int? conjuntoVis, string? estadoCU, int? conjuntoCU, string? ocupadoVis) =>
{
    QuestPDF.Settings.License = LicenseType.Community;

    string titulo = "";
    string[] headers = [];
    List<string[]> filas = new();

    switch (entidad.ToLower())
    {
        case "usuarios":
            titulo = "Usuarios";
            headers = ["ID", "Nombre", "Email", "Documento", "Teléfono", "Rol", "Propietario"];
            var qUsrPdf = db.Usuarios.AsQueryable();
            if (rolId.HasValue && rolId.Value != 0) qUsrPdf = qUsrPdf.Where(u => u.RolId == rolId.Value);
            var usrs = await (from u in qUsrPdf join r in db.Roles on (long)u.RolId equals r.RolId into ur from r in ur.DefaultIfEmpty()
                              select new { u.UsuarioId, u.Nombre, u.Email, u.Documento, u.Telefono, Rol = r != null ? r.Nombre : "N/A", Prop = u.EsPropietario == true ? "Sí" : "No" }).ToListAsync();
            filas = usrs.Select(x => new[] { x.UsuarioId.ToString(), x.Nombre, x.Email, x.Documento, x.Telefono, x.Rol, x.Prop }).ToList();
            break;
        case "conjuntos":
            titulo = "Conjuntos";
            headers = ["ID", "Nombre", "Dirección", "Ciudad", "NIT", "Teléfono"];
            var conjs = await db.Conjuntos.ToListAsync();
            filas = conjs.Select(x => new[] { x.ConjuntoId.ToString(), x.Nombre, x.Direccion, x.Ciudad, x.Nit, x.Telefono }).ToList();
            break;
        case "torres":
            titulo = "Torres";
            headers = ["ID", "Nombre", "Conjunto"];
            var torrs = await (from t in db.Torres join c in db.Conjuntos on t.ConjuntoId equals c.ConjuntoId into tc from c in tc.DefaultIfEmpty()
                               select new { t.TorreId, t.Nombre, Conjunto = c != null ? c.Nombre : "N/A" }).ToListAsync();
            filas = torrs.Select(x => new[] { x.TorreId.ToString(), x.Nombre, x.Conjunto }).ToList();
            break;
        case "apartamentos":
            titulo = "Apartamentos";
            headers = ["ID", "Número", "Tipo", "Torre", "Conjunto"];
            var qAptoPdf = db.Apartamentos.AsQueryable();
            if (conjuntoId.HasValue && conjuntoId.Value != 0) qAptoPdf = qAptoPdf.Where(a => a.ConjuntoId == conjuntoId.Value);
            if (torreId.HasValue && torreId.Value != 0) qAptoPdf = qAptoPdf.Where(a => a.TorreId == torreId.Value);
            var aptosPdf = await (from a in qAptoPdf join t in db.Torres on a.TorreId equals t.TorreId into at from t in at.DefaultIfEmpty()
                               join c in db.Conjuntos on (long?)a.ConjuntoId equals c.ConjuntoId into ac from c in ac.DefaultIfEmpty()
                               select new { a.ApartamentoId, a.Numero, a.Tipo, Torre = t != null ? t.Nombre : "N/A", Conjunto = c != null ? c.Nombre : "N/A" }).ToListAsync();
            filas = aptosPdf.Select(x => new[] { x.ApartamentoId.ToString(), x.Numero, x.Tipo, x.Torre, x.Conjunto }).ToList();
            break;
        case "roles":
            titulo = "Roles";
            headers = ["ID", "Nombre"];
            var roles = await db.Roles.ToListAsync();
            filas = roles.Select(x => new[] { x.RolId.ToString(), x.Nombre }).ToList();
            break;
        case "zonascomunes":
            titulo = "Zonas Comunes";
            headers = ["ID", "Nombre", "Requiere Pago", "Valor Hora"];
            var zonas = await db.ZonasComunes.ToListAsync();
            filas = zonas.Select(x => new[] { x.ZonaComunId.ToString(), x.Nombre, x.RequierePago ? "Sí" : "No", x.ValorHora?.ToString() ?? "0" }).ToList();
            break;
        case "tiposmantenimiento":
            titulo = "Tipos de Mantenimiento";
            headers = ["ID", "Nombre"];
            var tipos = await db.TiposMantenimiento.ToListAsync();
            filas = tipos.Select(x => new[] { x.TipoMantenimientoId.ToString(), x.Nombre }).ToList();
            break;
        case "mantenimientos":
            titulo = "Mantenimientos";
            headers = ["ID", "Tipo", "Fecha", "Proveedor", "Descripción", "Costo", "Zona"];
            var qMant = db.Mantenimientos.AsQueryable();
            if (inicio.HasValue) qMant = qMant.Where(m => m.Fecha >= DateOnly.FromDateTime(inicio.Value));
            if (fin.HasValue) qMant = qMant.Where(m => m.Fecha <= DateOnly.FromDateTime(fin.Value));
            if (tipoMantenimientoId.HasValue && tipoMantenimientoId.Value != 0) qMant = qMant.Where(m => m.TipoMantenimientoId == tipoMantenimientoId.Value);
            var mants = await (from m in qMant join t in db.TiposMantenimiento on m.TipoMantenimientoId equals t.TipoMantenimientoId into mt from t in mt.DefaultIfEmpty()
                               join z in db.ZonasComunes on m.ZonaComunId equals z.ZonaComunId into mz from z in mz.DefaultIfEmpty()
                               select new { m.MantenimientoId, Tipo = t != null ? t.Nombre : "N/A", m.Fecha, m.Provider, m.Descripcion, m.Costo, Zona = z != null ? z.Nombre : "N/A" }).ToListAsync();
            filas = mants.Select(x => new[] { x.MantenimientoId.ToString(), x.Tipo, x.Fecha.ToString(), x.Provider, x.Descripcion, x.Costo.ToString("C"), x.Zona }).ToList();
            break;
        case "reservas":
            titulo = "Reservas";
            headers = ["ID", "Zona", "Usuario", "Fecha", "Inicio", "Fin", "Estado", "Total"];
            var qRes = db.Reservas.AsQueryable();
            if (inicio.HasValue) qRes = qRes.Where(r => r.Fecha >= DateOnly.FromDateTime(inicio.Value));
            if (fin.HasValue) qRes = qRes.Where(r => r.Fecha <= DateOnly.FromDateTime(fin.Value));
            if (zonaComunId.HasValue && zonaComunId.Value != 0) qRes = qRes.Where(r => r.ZonaComunId == zonaComunId.Value);
            var reservas = await (from r in qRes join z in db.ZonasComunes on r.ZonaComunId equals z.ZonaComunId into rz from z in rz.DefaultIfEmpty()
                                  join u in db.Usuarios on r.UsuarioId equals u.UsuarioId into ru from u in ru.DefaultIfEmpty()
                                  select new { r.ReservaId, Zona = z != null ? z.Nombre : "N/A", Usuario = u != null ? u.Nombre : "N/A", r.Fecha, r.HoraInicio, r.HoraFin, r.Estado, r.TotalCobrar }).ToListAsync();
            filas = reservas.Select(x => new[] { x.ReservaId.ToString(), x.Zona, x.Usuario, x.Fecha.ToString(), x.HoraInicio.ToString(), x.HoraFin.ToString(), x.Estado, x.TotalCobrar?.ToString("C") ?? "0" }).ToList();
            break;
        case "ingresos":
            titulo = "Ingresos";
            headers = ["ID", "Tipo", "Nombre", "Documento", "Ingreso", "Salida", "Apartamento", "Torre", "Observaciones"];
            var qIng = db.Ingresos.AsQueryable();
            if (inicio.HasValue) qIng = qIng.Where(i => i.FechaHoraIngreso >= inicio.Value);
            if (fin.HasValue) qIng = qIng.Where(i => i.FechaHoraIngreso <= fin.Value);
            if (!string.IsNullOrEmpty(tipoIngreso)) qIng = qIng.Where(i => i.Tipo == tipoIngreso);
            var ings = await (from i in qIng join a in db.Apartamentos on i.ApartamentoId equals (int)a.ApartamentoId into ia from a in ia.DefaultIfEmpty()
                              join t in db.Torres on i.TorreId equals t.TorreId into it from t in it.DefaultIfEmpty()
                              select new { i.IngresoId, i.Tipo, i.NombrePersona, i.Documento, i.FechaHoraIngreso, i.FechaHoraSalida, Apto = a != null ? a.Numero : "N/A", Torre = t != null ? t.Nombre : "N/A", i.Observaciones }).ToListAsync();
            filas = ings.Select(x => new[] { x.IngresoId.ToString(), x.Tipo, x.NombrePersona, x.Documento, x.FechaHoraIngreso.ToString("dd/MM/yyyy HH:mm"), x.FechaHoraSalida?.ToString("dd/MM/yyyy HH:mm") ?? "", x.Apto, x.Torre, x.Observaciones ?? "" }).ToList();
            break;
        case "bitacora":
            titulo = "Bitácora de Vigilancia";
            headers = ["ID", "Vigilante", "Fecha Hora", "Observación"];
            var qBit = db.BitacorasVigilancia.AsQueryable();
            if (inicio.HasValue) qBit = qBit.Where(b => b.FechaHora >= inicio.Value);
            if (fin.HasValue) qBit = qBit.Where(b => b.FechaHora <= fin.Value);
            var bits = await (from b in qBit join u in db.Usuarios on b.VigilanteId equals u.UsuarioId into bu from u in bu.DefaultIfEmpty()
                              select new { b.BitacoraId, Vigilante = u != null ? u.Nombre : "N/A", b.FechaHora, b.Observacion }).ToListAsync();
            filas = bits.Select(x => new[] { x.BitacoraId.ToString(), x.Vigilante, x.FechaHora.ToString("dd/MM/yyyy HH:mm"), x.Observacion }).ToList();
            break;
        case "parqueaderos":
            titulo = "Parqueaderos";
            headers = ["ID", "Tipo", "Apartamento", "Torre", "Conjunto"];
            var qParkPdf = db.Parqueaderos.AsQueryable();
            if (!string.IsNullOrEmpty(tipoPar)) qParkPdf = qParkPdf.Where(p => p.Tipo == tipoPar);
            if (apartamentoIdPar.HasValue && apartamentoIdPar.Value != 0) qParkPdf = qParkPdf.Where(p => p.ApartamentoId == apartamentoIdPar.Value);
            var parks = await (from p in qParkPdf join a in db.Apartamentos on p.ApartamentoId equals a.ApartamentoId into pa from a in pa.DefaultIfEmpty()
                               join t in db.Torres on p.TorreId equals t.TorreId into pt from t in pt.DefaultIfEmpty()
                               join c in db.Conjuntos on p.ConjuntoId equals (long?)c.ConjuntoId into pc from c in pc.DefaultIfEmpty()
                               select new { p.ParqueaderoId, p.Tipo, Apto = a != null ? a.Numero : "N/A", Torre = t != null ? t.Nombre : "N/A", Conjunto = c != null ? c.Nombre : "N/A" }).ToListAsync();
            filas = parks.Select(x => new[] { x.ParqueaderoId.ToString(), x.Tipo, x.Apto, x.Torre, x.Conjunto }).ToList();
            break;
        case "visitantes":
            titulo = "Parqueaderos Visitantes";
            headers = ["ID", "Tipo", "Placa", "Ocupado", "Apartamento", "Ingreso", "Salida"];
            var qVis = db.ParqueaderosVisitantes.AsQueryable();
            if (inicio.HasValue) qVis = qVis.Where(v => v.FechaHoraIngreso >= inicio.Value);
            if (fin.HasValue) qVis = qVis.Where(v => v.FechaHoraIngreso <= fin.Value);
            if (!string.IsNullOrEmpty(tipoVis)) qVis = qVis.Where(v => v.Tipo == tipoVis);
            if (conjuntoVis.HasValue && conjuntoVis.Value != 0) qVis = qVis.Where(v => v.ConjuntoId == conjuntoVis.Value);
            if (ocupadoVis == "si") qVis = qVis.Where(v => v.Ocupado);
            else if (ocupadoVis == "no") qVis = qVis.Where(v => !v.Ocupado);
            var vists = await (from v in qVis join a in db.Apartamentos on v.ApartamentoId equals a.ApartamentoId into va from a in va.DefaultIfEmpty()
                               select new { v.ParqueaderoVisitanteId, v.Tipo, v.Placa, Ocu = v.Ocupado ? "Sí" : "No", Apto = a != null ? a.Numero : "N/A", v.FechaHoraIngreso, v.FechaHoraSalida }).ToListAsync();
            filas = vists.Select(x => new[] { x.ParqueaderoVisitanteId.ToString(), x.Tipo, x.Placa, x.Ocu, x.Apto, x.FechaHoraIngreso?.ToString("dd/MM/yyyy HH:mm") ?? "", x.FechaHoraSalida?.ToString("dd/MM/yyyy HH:mm") ?? "" }).ToList();
            break;
        case "cuartosutil":
            titulo = "Cuartos Útiles";
            headers = ["ID", "Número", "Estado", "Apartamento", "Torre", "Conjunto"];
            var qCUPdf = db.CuartosUtil.AsQueryable();
            if (!string.IsNullOrEmpty(estadoCU)) qCUPdf = qCUPdf.Where(c => c.Estado == estadoCU);
            if (conjuntoCU.HasValue && conjuntoCU.Value != 0) qCUPdf = qCUPdf.Where(c => c.ConjuntoId == conjuntoCU.Value);
            var cus = await (from c in qCUPdf join a in db.Apartamentos on c.ApartamentoId equals a.ApartamentoId into ca from a in ca.DefaultIfEmpty()
                             join t in db.Torres on c.TorreId equals (long?)t.TorreId into ct from t in ct.DefaultIfEmpty()
                             join co in db.Conjuntos on c.ConjuntoId equals (long?)co.ConjuntoId into cc from co in cc.DefaultIfEmpty()
                             select new { c.CuartoUtilId, c.CuartoUtilNumero, c.Estado, Apto = a != null ? a.Numero : "N/A", Torre = t != null ? t.Nombre : "N/A", Conjunto = co != null ? co.Nombre : "N/A" }).ToListAsync();
            filas = cus.Select(x => new[] { x.CuartoUtilId.ToString(), x.CuartoUtilNumero, x.Estado, x.Apto, x.Torre, x.Conjunto }).ToList();
            break;
        default:
            return Results.BadRequest("Entidad no soportada para PDF.");
    }

    var pdfBytes = GenerarPdf(titulo, headers, filas, inicio, fin);
    return Results.File(pdfBytes, "application/pdf", $"Reporte_{entidad}.pdf");
});

byte[] GenerarPdf(string titulo, string[] headers, List<string[]> filas, DateTime? inicio, DateTime? fin)
{
    return Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(1.5f, Unit.Centimetre);
            page.DefaultTextStyle(x => x.FontSize(9));

            page.Header().Column(col =>
            {
                col.Item().Text(titulo).FontSize(16).Bold().FontColor(Colors.Indigo.Darken2);
                if (inicio.HasValue || fin.HasValue)
                    col.Item().Text($"Período: {(inicio.HasValue ? inicio.Value.ToString("dd/MM/yyyy") : "inicio")} — {(fin.HasValue ? fin.Value.ToString("dd/MM/yyyy") : "fin")}").FontSize(9).FontColor(Colors.Grey.Darken1);
                col.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Medium);
                col.Item().PaddingTop(4).LineHorizontal(1).LineColor(Colors.Indigo.Lighten2);
            });

            page.Content().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(cols => { foreach (var _ in headers) cols.RelativeColumn(); });

                table.Header(h =>
                {
                    foreach (var header in headers)
                        h.Cell().Background(Colors.Indigo.Darken1).Padding(5).Text(header).FontColor(Colors.White).Bold().FontSize(8);
                });

                bool alt = false;
                foreach (var fila in filas)
                {
                    alt = !alt;
                    var bg = alt ? Colors.Grey.Lighten4 : Colors.White;
                    foreach (var celda in fila)
                        table.Cell().Background(bg).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(celda ?? "").FontSize(8);
                }
            });

            page.Footer().AlignRight().Text(x =>
            {
                x.Span("Página ").FontSize(8).FontColor(Colors.Grey.Medium);
                x.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                x.Span(" de ").FontSize(8).FontColor(Colors.Grey.Medium);
                x.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
            });
        });
    }).GeneratePdf();
}

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
