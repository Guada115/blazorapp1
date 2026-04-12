using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApp1.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "apartamentos",
                schema: "dbo",
                columns: table => new
                {
                    unidadid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    torreid = table.Column<int>(type: "int", nullable: false),
                    numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    area = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_apartamentos", x => x.unidadid);
                });

            migrationBuilder.CreateTable(
                name: "bitacoravigilancia",
                schema: "dbo",
                columns: table => new
                {
                    bitacoraid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vigilanteid = table.Column<int>(type: "int", nullable: false),
                    fechahora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    observacion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bitacoravigilancia", x => x.bitacoraid);
                });

            migrationBuilder.CreateTable(
                name: "conjunto",
                schema: "dbo",
                columns: table => new
                {
                    conjuntoid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ciudad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conjunto", x => x.conjuntoid);
                });

            migrationBuilder.CreateTable(
                name: "ingreso",
                schema: "dbo",
                columns: table => new
                {
                    ingresoid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nombrepersona = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    documento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fechahoraingreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fechahorasalida = table.Column<DateTime>(type: "datetime2", nullable: true),
                    usuarioid = table.Column<int>(type: "int", nullable: true),
                    unidadid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingreso", x => x.ingresoid);
                });

            migrationBuilder.CreateTable(
                name: "mantenimiento",
                schema: "dbo",
                columns: table => new
                {
                    mantenimientoid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipomantenimientoid = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    proveedor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    costo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    zonacomunid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mantenimiento", x => x.mantenimientoid);
                });

            migrationBuilder.CreateTable(
                name: "parqueadero",
                schema: "dbo",
                columns: table => new
                {
                    parqueaderoid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    unidadid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parqueadero", x => x.parqueaderoid);
                });

            migrationBuilder.CreateTable(
                name: "parqueaderovisitante",
                schema: "dbo",
                columns: table => new
                {
                    parqueaderovisitanteid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    parqueaderoid = table.Column<int>(type: "int", nullable: false),
                    placa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fechahoraingreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fechahorasalida = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ingresoid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parqueaderovisitante", x => x.parqueaderovisitanteid);
                });

            migrationBuilder.CreateTable(
                name: "reserva",
                schema: "dbo",
                columns: table => new
                {
                    reservaid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    zonacomunid = table.Column<int>(type: "int", nullable: false),
                    usuarioid = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    horainicio = table.Column<TimeOnly>(type: "time", nullable: false),
                    horafin = table.Column<TimeOnly>(type: "time", nullable: false),
                    estado = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reserva", x => x.reservaid);
                });

            migrationBuilder.CreateTable(
                name: "residenteunidad",
                schema: "dbo",
                columns: table => new
                {
                    residenteunidadid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    usuarioid = table.Column<int>(type: "int", nullable: false),
                    unidadid = table.Column<int>(type: "int", nullable: false),
                    espropietario = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_residenteunidad", x => x.residenteunidadid);
                });

            migrationBuilder.CreateTable(
                name: "rol",
                schema: "dbo",
                columns: table => new
                {
                    rolid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rol", x => x.rolid);
                });

            migrationBuilder.CreateTable(
                name: "tipomantenimiento",
                schema: "dbo",
                columns: table => new
                {
                    tipomantenimientoid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipomantenimiento", x => x.tipomantenimientoid);
                });

            migrationBuilder.CreateTable(
                name: "torre",
                schema: "dbo",
                columns: table => new
                {
                    torreid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    conjuntoid = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_torre", x => x.torreid);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                schema: "dbo",
                columns: table => new
                {
                    usuarioid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rolid = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    documento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false),
                    fechacreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Clave = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.usuarioid);
                });

            migrationBuilder.CreateTable(
                name: "zonacomun",
                schema: "dbo",
                columns: table => new
                {
                    zonacomunid = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    requierepago = table.Column<bool>(type: "bit", nullable: false),
                    valorhora = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zonacomun", x => x.zonacomunid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "apartamentos",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "bitacoravigilancia",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "conjunto",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ingreso",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "mantenimiento",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "parqueadero",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "parqueaderovisitante",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "reserva",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "residenteunidad",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "rol",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "tipomantenimiento",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "torre",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "usuario",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "zonacomun",
                schema: "dbo");
        }
    }
}
