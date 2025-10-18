using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrogueriaAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixProveedorUsuarioRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProveedorIdProveedor",
                table: "ProveedorProductos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdUsuario",
                table: "Proveedores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorProductos_ProveedorIdProveedor",
                table: "ProveedorProductos",
                column: "ProveedorIdProveedor");

            migrationBuilder.AddForeignKey(
                name: "FK_ProveedorProductos_Proveedores_ProveedorIdProveedor",
                table: "ProveedorProductos",
                column: "ProveedorIdProveedor",
                principalTable: "Proveedores",
                principalColumn: "IdProveedor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProveedorProductos_Proveedores_ProveedorIdProveedor",
                table: "ProveedorProductos");

            migrationBuilder.DropIndex(
                name: "IX_ProveedorProductos_ProveedorIdProveedor",
                table: "ProveedorProductos");

            migrationBuilder.DropColumn(
                name: "ProveedorIdProveedor",
                table: "ProveedorProductos");

            migrationBuilder.DropColumn(
                name: "IdUsuario",
                table: "Proveedores");
        }
    }
}
