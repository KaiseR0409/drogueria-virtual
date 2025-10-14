using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrogueriaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceDetailFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProveedorProducto_Producto_IdProducto",
                table: "ProveedorProducto");

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
                name: "Rut",
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
                name: "Rut",
                table: "Proveedor",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
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

            migrationBuilder.AddColumn<string>(
                name: "VendedorAsignado",
                table: "Orden",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orden_IdProveedor",
                table: "Orden",
                column: "IdProveedor");

            migrationBuilder.AddForeignKey(
                name: "FK_Orden_Proveedor_IdProveedor",
                table: "Orden",
                column: "IdProveedor",
                principalTable: "Proveedor",
                principalColumn: "IdProveedor",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProveedorProducto_Producto_IdProducto",
                table: "ProveedorProducto",
                column: "IdProducto",
                principalTable: "Producto",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orden_Proveedor_IdProveedor",
                table: "Orden");

            migrationBuilder.DropForeignKey(
                name: "FK_ProveedorProducto_Producto_IdProducto",
                table: "ProveedorProducto");

            migrationBuilder.DropIndex(
                name: "IX_Orden_IdProveedor",
                table: "Orden");

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
                name: "Rut",
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
                name: "Rut",
                table: "Proveedor");

            migrationBuilder.DropColumn(
                name: "Telefono",
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
                name: "VendedorAsignado",
                table: "Orden");

            migrationBuilder.AlterColumn<decimal>(
                name: "Descuento",
                table: "Orden",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProveedorProducto_Producto_IdProducto",
                table: "ProveedorProducto",
                column: "IdProducto",
                principalTable: "Producto",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
