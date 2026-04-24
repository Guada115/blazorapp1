using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Client.Models
{
    [Table("rol", Schema = "dbo")]
    public class Rol
    {
        [Key]
        [Column("rolid")]
        public long RolId { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }

    [Table("usuario", Schema = "dbo")]
    public class Usuario
    {
        [Key]
        [Column("usuarioid")]
        public long UsuarioId { get; set; }

        [Column("rolid")]
        public int RolId { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("documento")]
        public string Documento { get; set; } = string.Empty;

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("telefono")]
        public string Telefono { get; set; } = string.Empty;

        [NotMapped]
        public bool Activo { get; set; } = true;

        [Column("fechacreacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Column("Clave")]
        public string Clave { get; set; } = string.Empty;

        [Column("espropietario")]
        public bool? EsPropietario { get; set; }

        [Column("apartamentoid")]
        public int? ApartamentoId { get; set; }

        [Column("torreid")]
        public int? TorreId { get; set; }

        public override string ToString() => $"{Nombre} {Email} {Documento} {Telefono}";
    }

    [Table("conjunto", Schema = "dbo")]
    public class Conjunto
    {
        [Key]
        [Column("conjuntoid")]
        public long ConjuntoId { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("direccion")]
        public string Direccion { get; set; } = string.Empty;

        [Column("ciudad")]
        public string Ciudad { get; set; } = string.Empty;

        [Column("nit")]
        public string Nit { get; set; } = string.Empty;

        [Column("telefono")]
        public string Telefono { get; set; } = string.Empty;
    }

    [Table("torre", Schema = "dbo")]
    public class Torre
    {
        [Key]
        [Column("torreid")]
        public long TorreId { get; set; }

        [Column("conjuntoid")]
        public int ConjuntoId { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }

    [Table("apartamentos", Schema = "dbo")]
    public class Apartamento
    {
        [Key]
        [Column("apartamentoid")]
        public long ApartamentoId { get; set; }

        [Column("torreid")]
        public int TorreId { get; set; }

        [Column("numero")]
        public string Numero { get; set; } = string.Empty;

        [Column("tipo")]
        public string Tipo { get; set; } = string.Empty;

        [Column("conjuntoid")]
        public int? ConjuntoId { get; set; }
    }

    [Table("residenteunidad", Schema = "dbo")]
    public class ResidenteUnidad
    {
        [Key]
        [Column("residenteunidadid")]
        public long ResidenteUnidadId { get; set; }

        [Column("usuarioid")]
        public int UsuarioId { get; set; }

        [Column("torreid")]
        public int TorreId { get; set; }

        [Column("espropietario")]
        public bool EsPropietario { get; set; }
    }

    [Table("bitacoravigilancia", Schema = "dbo")]
    public class BitacoraVigilancia
    {
        [Key]
        [Column("bitacoraid")]
        public long BitacoraId { get; set; }

        [Column("vigilanteid")]
        public int VigilanteId { get; set; }

        [Column("fechahora")]
        public DateTime FechaHora { get; set; }

        [Column("observacion")]
        public string Observacion { get; set; } = string.Empty;
    }

    [Table("ingreso", Schema = "dbo")]
    public class Ingreso
    {
        [Key]
        [Column("ingresoid")]
        public long IngresoId { get; set; }

        [Column("tipo")]
        public string Tipo { get; set; } = string.Empty;

        [Column("nombrepersona")]
        public string NombrePersona { get; set; } = string.Empty;

        [Column("documento")]
        public string Documento { get; set; } = string.Empty;

        [Column("fechahoraingreso")]
        public DateTime FechaHoraIngreso { get; set; }

        [Column("fechahorasalida")]
        public DateTime? FechaHoraSalida { get; set; }

        [Column("usuarioid")]
        public int? UsuarioId { get; set; }

        [Column("torreid")]
        public int TorreId { get; set; }

        [Column("apartamentoid")]
        public int? ApartamentoId { get; set; }

        [Column("observaciones")]
        public string? Observaciones { get; set; }
    }

    [Table("zonacomun", Schema = "dbo")]
    public class ZonaComun
    {
        [Key]
        [Column("zonacomunid")]
        public long ZonaComunId { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("requierepago")]
        public bool RequierePago { get; set; }

        [Column("valorhora")]
        public int? ValorHora { get; set; }
    }

    [Table("tipomantenimiento", Schema = "dbo")]
    public class TipoMantenimiento
    {
        [Key]
        [Column("tipomantenimientoid")]
        public long TipoMantenimientoId { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }

    [Table("mantenimiento", Schema = "dbo")]
    public class Mantenimiento
    {
        [Key]
        [Column("mantenimientoid")]
        public long MantenimientoId { get; set; }

        [Column("tipomantenimientoid")]
        public int TipoMantenimientoId { get; set; }

        [Column("fecha")]
        public DateOnly Fecha { get; set; }

        [Column("proveedor")]
        public string Provider { get; set; } = string.Empty;

        [Column("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [Column("costo")]
        public decimal Costo { get; set; }

        [Column("zonacomunid")]
        public int ZonaComunId { get; set; }
    }

    [Table("parqueadero", Schema = "dbo")]
    public class Parqueadero
    {
        [Key]
        [Column("parqueaderoid")]
        public long ParqueaderoId { get; set; }

        [Column("tipo")]
        public string Tipo { get; set; } = string.Empty;

        [Column("numero")]
        public string Numero { get; set; } = string.Empty;

        [Column("torreid")]
        public int TorreId { get; set; }
    }

    [Table("parqueaderovisitante", Schema = "dbo")]
    public class ParqueaderoVisitante
    {
        [Key]
        [Column("parqueaderovisitanteid")]
        public int ParqueaderoVisitanteId { get; set; }

        [Column("tipo")]
        public string Tipo { get; set; } = string.Empty;

        [Column("ocupado")]
        public bool Ocupado { get; set; }

        [Column("placa")]
        public string Placa { get; set; } = string.Empty;

        [Column("apartamentoid")]
        public long? ApartamentoId { get; set; }

        [Column("fechahoraingreso")]
        public DateTime? FechaHoraIngreso { get; set; }

        [Column("fechahorasalida")]
        public DateTime? FechaHoraSalida { get; set; }
    }

    [Table("reserva", Schema = "dbo")]
    public class Reserva
    {
        [Key]
        [Column("reservaid")]
        public long ReservaId { get; set; }

        [Column("zonacomunid")]
        public int ZonaComunId { get; set; }

        [Column("usuarioid")]
        public int UsuarioId { get; set; }

        [Column("fecha")]
        public DateOnly Fecha { get; set; }

        [Column("horainicio")]
        public TimeOnly HoraInicio { get; set; }

        [Column("horafin")]
        public TimeOnly HoraFin { get; set; }

        [Column("estado")]
        public string Estado { get; set; } = string.Empty;

        [Column("totalcobrar")]
        public decimal? TotalCobrar { get; set; }
    }
}
