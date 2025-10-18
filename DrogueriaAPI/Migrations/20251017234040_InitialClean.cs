using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrogueriaAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemsOrden_Ordenes_IdOrden",
                table: "ItemsOrden");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemsOrden_Producto_ProductoIdProducto",
                table: "ItemsOrden");

            migrationBuilder.DropForeignKey(
                name: "FK_Ordenes_Proveedores_IdProveedor",
                table: "Ordenes");

            migrationBuilder.DropForeignKey(
                name: "FK_Ordenes_Usuario_IdUsuario",
                table: "Ordenes");

            migrationBuilder.DropForeignKey(
                name: "FK_ProveedorProductos_Producto_ProductoIdProducto",
                table: "ProveedorProductos");

            migrationBuilder.DropForeignKey(
                name: "FK_ProveedorProductos_Proveedores_ProveedorIdProveedor",
                table: "ProveedorProductos");

            migrationBuilder.DropIndex(
                name: "IX_ProveedorProductos_ProveedorIdProveedor",
                table: "ProveedorProductos");

            migrationBuilder.DropColumn(
                name: "ProveedorIdProveedor",
                table: "ProveedorProductos");

            migrationBuilder.AlterColumn<int>(
                name: "ProductoIdProducto",
                table: "ItemsOrden",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorProductos_IdProducto",
                table: "ProveedorProductos",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsOrden_IdProducto",
                table: "ItemsOrden",
                column: "IdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsOrden_Ordenes_IdOrden",
                table: "ItemsOrden",
                column: "IdOrden",
                principalTable: "Ordenes",
                principalColumn: "IdOrden",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsOrden_Producto_IdProducto",
                table: "ItemsOrden",
                column: "IdProducto",
                principalTable: "Producto",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsOrden_Producto_ProductoIdProducto",
                table: "ItemsOrden",
                column: "ProductoIdProducto",
                principalTable: "Producto",
                principalColumn: "IdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_Ordenes_Proveedores_IdProveedor",
                table: "Ordenes",
                column: "IdProveedor",
                principalTable: "Proveedores",
                principalColumn: "IdProveedor",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ordenes_Usuario_IdUsuario",
                table: "Ordenes",
                column: "IdUsuario",
                principalTable: "Usuario",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProveedorProductos_Producto_IdProducto",
                table: "ProveedorProductos",
                column: "IdProducto",
                principalTable: "Producto",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProveedorProductos_Producto_ProductoIdProducto",
                table: "ProveedorProductos",
                column: "ProductoIdProducto",
                principalTable: "Producto",
                principalColumn: "IdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_ProveedorProductos_Proveedores_IdProveedor",
                table: "ProveedorProductos",
                column: "IdProveedor",
                principalTable: "Proveedores",
                principalColumn: "IdProveedor",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemsOrden_Ordenes_IdOrden",
                table: "ItemsOrden");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemsOrden_Producto_IdProducto",
                table: "ItemsOrden");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemsOrden_Producto_ProductoIdProducto",
                table: "ItemsOrden");

            migrationBuilder.DropForeignKey(
                name: "FK_Ordenes_Proveedores_IdProveedor",
                table: "Ordenes");

            migrationBuilder.DropForeignKey(
                name: "FK_Ordenes_Usuario_IdUsuario",
                table: "Ordenes");

            migrationBuilder.DropForeignKey(
                name: "FK_ProveedorProductos_Producto_IdProducto",
                table: "ProveedorProductos");

            migrationBuilder.DropForeignKey(
                name: "FK_ProveedorProductos_Producto_ProductoIdProducto",
                table: "ProveedorProductos");

            migrationBuilder.DropForeignKey(
                name: "FK_ProveedorProductos_Proveedores_IdProveedor",
                table: "ProveedorProductos");

            migrationBuilder.DropIndex(
                name: "IX_ProveedorProductos_IdProducto",
                table: "ProveedorProductos");

            migrationBuilder.DropIndex(
                name: "IX_ItemsOrden_IdProducto",
                table: "ItemsOrden");

            migrationBuilder.AddColumn<int>(
                name: "ProveedorIdProveedor",
                table: "ProveedorProductos",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductoIdProducto",
                table: "ItemsOrden",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorProductos_ProveedorIdProveedor",
                table: "ProveedorProductos",
                column: "ProveedorIdProveedor");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsOrden_Ordenes_IdOrden",
                table: "ItemsOrden",
                column: "IdOrden",
                principalTable: "Ordenes",
                principalColumn: "IdOrden",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemsOrden_Producto_ProductoIdProducto",
                table: "ItemsOrden",
                column: "ProductoIdProducto",
                principalTable: "Producto",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ordenes_Proveedores_IdProveedor",
                table: "Ordenes",
                column: "IdProveedor",
                principalTable: "Proveedores",
                principalColumn: "IdProveedor",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ordenes_Usuario_IdUsuario",
                table: "Ordenes",
                column: "IdUsuario",
                principalTable: "Usuario",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProveedorProductos_Producto_ProductoIdProducto",
                table: "ProveedorProductos",
                column: "ProductoIdProducto",
                principalTable: "Producto",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProveedorProductos_Proveedores_ProveedorIdProveedor",
                table: "ProveedorProductos",
                column: "ProveedorIdProveedor",
                principalTable: "Proveedores",
                principalColumn: "IdProveedor",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
