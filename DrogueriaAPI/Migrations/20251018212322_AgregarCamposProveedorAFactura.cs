using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrogueriaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposProveedorAFactura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ciudad",
                table: "Proveedores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DireccionComercial",
                table: "Proveedores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Giro",
                table: "Proveedores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RUT",
                table: "Proveedores",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ciudad",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "DireccionComercial",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "Giro",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "RUT",
                table: "Proveedores");
        }
    }
}
