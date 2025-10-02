using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrogueriaAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProductoIdProductoFromItemOrden : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemOrden_Orden_OrdenIdOrden",
                table: "ItemOrden");

            migrationBuilder.DropIndex(
                name: "IX_ItemOrden_OrdenIdOrden",
                table: "ItemOrden");

            migrationBuilder.DropColumn(
                name: "OrdenIdOrden",
                table: "ItemOrden");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOrden_IdOrden",
                table: "ItemOrden",
                column: "IdOrden");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOrden_IdProducto",
                table: "ItemOrden",
                column: "IdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrden_Orden_IdOrden",
                table: "ItemOrden",
                column: "IdOrden",
                principalTable: "Orden",
                principalColumn: "IdOrden",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrden_Producto_IdProducto",
                table: "ItemOrden",
                column: "IdProducto",
                principalTable: "Producto",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemOrden_Orden_IdOrden",
                table: "ItemOrden");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemOrden_Producto_IdProducto",
                table: "ItemOrden");

            migrationBuilder.DropIndex(
                name: "IX_ItemOrden_IdOrden",
                table: "ItemOrden");

            migrationBuilder.DropIndex(
                name: "IX_ItemOrden_IdProducto",
                table: "ItemOrden");

            migrationBuilder.AddColumn<int>(
                name: "OrdenIdOrden",
                table: "ItemOrden",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemOrden_OrdenIdOrden",
                table: "ItemOrden",
                column: "OrdenIdOrden");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemOrden_Orden_OrdenIdOrden",
                table: "ItemOrden",
                column: "OrdenIdOrden",
                principalTable: "Orden",
                principalColumn: "IdOrden");
        }
    }
}
