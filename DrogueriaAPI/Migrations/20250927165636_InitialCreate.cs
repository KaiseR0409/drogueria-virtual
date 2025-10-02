using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrogueriaAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreProducto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrincipioActivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Concentracion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormaFarmaceutica = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PresentacionComercial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LaboratorioFabricante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistroSanitario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CondicionesAlmacenamiento = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.IdProducto);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    fechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    fechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TipoEstablecimiento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    direccion1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    direccion2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    direccion3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Proveedor",
                columns: table => new
                {
                    IdProveedor = table.Column<int>(type: "int", nullable: false),
                    NombreProveedor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedor", x => x.IdProveedor);
                    table.ForeignKey(
                        name: "FK_Proveedor_Usuario_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Proveedor");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
