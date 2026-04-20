using Microsoft.EntityFrameworkCore;
using BlazorApp1.Client.Models;

namespace BlazorApp1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Conjunto> Conjuntos { get; set; }
        public DbSet<Torre> Torres { get; set; }
        public DbSet<Apartamento> Apartamentos { get; set; }
        public DbSet<ResidenteUnidad> ResidentesUnidades { get; set; }
        public DbSet<BitacoraVigilancia> BitacorasVigilancia { get; set; }
        public DbSet<Ingreso> Ingresos { get; set; }
        public DbSet<ZonaComun> ZonasComunes { get; set; }
        public DbSet<TipoMantenimiento> TiposMantenimiento { get; set; }
        public DbSet<Mantenimiento> Mantenimientos { get; set; }
        public DbSet<Parqueadero> Parqueaderos { get; set; }
        public DbSet<ParqueaderoVisitante> ParqueaderosVisitantes { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // EF Core 7+ trigger configuration
            modelBuilder.Entity<Reserva>().ToTable(tb => tb.HasTrigger("trg_reserva_calcular_total"));
        }
    }
}
