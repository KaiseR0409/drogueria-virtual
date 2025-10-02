using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrogueriaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddItemOrdenNavegaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orden_Proveedor_ProveedorIdProveedor",
                table: "Orden");

            migrationBuilder.DropForeignKey(
                name: "FK_Orden_Usuario_UsuarioIdUsuario",
                table: "Orden");

            migrationBuilder.DropIndex(
                name: "IX_Orden_ProveedorIdProveedor",
                table: "Orden");

            migrationBuilder.DropIndex(
                name: "IX_Orden_UsuarioIdUsuario",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "ProveedorIdProveedor",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "UsuarioIdUsuario",
                table: "Orden");

            migrationBuilder.AlterColumn<string>(
                name: "EstadoOrden",
                table: "Orden",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Descuento",
                table: "Orden",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFactura",
                table: "Orden",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Impuestos",
                table: "Orden",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MetodoPago",
                table: "Orden",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Moneda",
                table: "Orden",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NumeroFactura",
                table: "Orden",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NumeroOrdenCompra",
                table: "Orden",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoComprobante",
                table: "Orden",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Descuento",
                table: "ItemOrden",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Impuesto",
                table: "ItemOrden",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "ItemOrden",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descuento",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "FechaFactura",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "Impuestos",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "MetodoPago",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "Moneda",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "NumeroFactura",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "NumeroOrdenCompra",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "TipoComprobante",
                table: "Orden");

            migrationBuilder.DropColumn(
                name: "Descuento",
                table: "ItemOrden");

            migrationBuilder.DropColumn(
                name: "Impuesto",
                table: "ItemOrden");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "ItemOrden");

            migrationBuilder.AlterColumn<string>(
                name: "EstadoOrden",
                table: "Orden",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ProveedorIdProveedor",
                table: "Orden",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdUsuario",
                table: "Orden",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orden_ProveedorIdProveedor",
                table: "Orden",
                column: "ProveedorIdProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_Orden_UsuarioIdUsuario",
                table: "Orden",
                column: "UsuarioIdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Orden_Proveedor_ProveedorIdProveedor",
                table: "Orden",
                column: "ProveedorIdProveedor",
                principalTable: "Proveedor",
                principalColumn: "IdProveedor");

            migrationBuilder.AddForeignKey(
                name: "FK_Orden_Usuario_UsuarioIdUsuario",
                table: "Orden",
                column: "UsuarioIdUsuario",
                principalTable: "Usuario",
                principalColumn: "IdUsuario");
        }
    }
}
