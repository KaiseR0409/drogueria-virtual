using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrogueriaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRutRazonSocial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orden_Usuario_UsuarioIdUsuario",
                table: "Orden");

            migrationBuilder.DropIndex(
                name: "IX_Orden_UsuarioIdUsuario",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "Rut",
                table: "Proveedor");

            migrationBuilder.DropColumn(
                name: "FechaVencimiento",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "Iva",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "UsuarioIdUsuario",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "VendedorAsignado",
                table: "Orden");

            migrationBuilder.AlterColumn<string>(
                name: "Rut",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RazonSocial",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "Descuento",
                table: "Orden",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RazonSocial",
                table: "Usuario");

            migrationBuilder.AlterColumn<string>(
                name: "Rut",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Rut",
                table: "Proveedor",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Descuento",
                table: "Orden",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaVencimiento",
                table: "Orden",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Iva",
                table: "Orden",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "Orden",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdUsuario",
                table: "Orden",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VendedorAsignado",
                table: "Orden",
                type: "nvarchar(max)",
                nullable: true);

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
    }
}
