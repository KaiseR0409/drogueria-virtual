using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrogueriaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddImagenUrlToProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemOrden_Orden_OrdenIdOrden",
                table: "ItemOrden");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOrden_Producto_ProductoIdProducto",
                table: "ItemOrden");

            migrationBuilder.AddColumn<int>(
                name: "ProductoIdProducto",
                table: "ProveedorProducto",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Producto",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductoIdProducto",
                table: "ItemOrden",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OrdenIdOrden",
                table: "ItemOrden",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorProducto_ProductoIdProducto",
                table: "ProveedorProducto",
                column: "ProductoIdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrden_Orden_OrdenIdOrden",
                table: "ItemOrden",
                column: "OrdenIdOrden",
                principalTable: "Orden",
                principalColumn: "IdOrden");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrden_Producto_ProductoIdProducto",
                table: "ItemOrden",
                column: "ProductoIdProducto",
                principalTable: "Producto",
                principalColumn: "IdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_ProveedorProducto_Producto_ProductoIdProducto",
                table: "ProveedorProducto",
                column: "ProductoIdProducto",
                principalTable: "Producto",
                principalColumn: "IdProducto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemOrden_Orden_OrdenIdOrden",
                table: "ItemOrden");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOrden_Producto_ProductoIdProducto",
                table: "ItemOrden");

            migrationBuilder.DropForeignKey(
                name: "FK_ProveedorProducto_Producto_ProductoIdProducto",
                table: "ProveedorProducto");

            migrationBuilder.DropIndex(
                name: "IX_ProveedorProducto_ProductoIdProducto",
                table: "ProveedorProducto");

            migrationBuilder.DropColumn(
                name: "ProductoIdProducto",
                table: "ProveedorProducto");

            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Producto");

            migrationBuilder.AlterColumn<int>(
                name: "ProductoIdProducto",
                table: "ItemOrden",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrdenIdOrden",
                table: "ItemOrden",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrden_Orden_OrdenIdOrden",
                table: "ItemOrden",
                column: "OrdenIdOrden",
                principalTable: "Orden",
                principalColumn: "IdOrden",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrden_Producto_ProductoIdProducto",
                table: "ItemOrden",
                column: "ProductoIdProducto",
                principalTable: "Producto",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
