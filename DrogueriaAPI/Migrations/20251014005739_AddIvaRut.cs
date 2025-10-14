using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrogueriaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIvaRut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comuna",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "Giro",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "Direccion",
                table: "Proveedor");

            migrationBuilder.DropColumn(
                name: "Giro",
                table: "Proveedor");

            migrationBuilder.DropColumn(
                name: "InfoPago",
                table: "Proveedor");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Proveedor");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdUsuario",
                table: "Orden",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orden_UsuarioIdUsuario",
                table: "Orden",
                column: "UsuarioIdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Orden_Usuario_UsuarioIdUsuario",
                table: "Orden",
                column: "UsuarioIdUsuario",
                principalTable: "Usuario",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orden_Usuario_UsuarioIdUsuario",
                table: "Orden");

            migrationBuilder.DropIndex(
                name: "IX_Orden_UsuarioIdUsuario",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "UsuarioIdUsuario",
                table: "Orden");

            migrationBuilder.AddColumn<string>(
                name: "Comuna",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Giro",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Direccion",
                table: "Proveedor",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Giro",
                table: "Proveedor",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InfoPago",
                table: "Proveedor",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Proveedor",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
