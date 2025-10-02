using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrogueriaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderAndInventoryEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orden",
                columns: table => new
                {
                    IdOrden = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdProveedor = table.Column<int>(type: "int", nullable: false),
                    FechaOrden = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadoOrden = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MontoTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UsuarioIdUsuario = table.Column<int>(type: "int", nullable: true),
                    ProveedorIdProveedor = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orden", x => x.IdOrden);
                    table.ForeignKey(
                        name: "FK_Orden_Proveedor_ProveedorIdProveedor",
                        column: x => x.ProveedorIdProveedor,
                        principalTable: "Proveedor",
                        principalColumn: "IdProveedor");
                    table.ForeignKey(
                        name: "FK_Orden_Usuario_UsuarioIdUsuario",
                        column: x => x.UsuarioIdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "ProveedorProducto",
                columns: table => new
                {
                    IdProveedor = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProveedorProducto", x => new { x.IdProveedor, x.IdProducto });
                    table.ForeignKey(
                        name: "FK_ProveedorProducto_Producto_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProveedorProducto_Proveedor_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedor",
                        principalColumn: "IdProveedor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemOrden",
                columns: table => new
                {
                    IdItemOrden = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrden = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrdenIdOrden = table.Column<int>(type: "int", nullable: false),
                    ProductoIdProducto = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemOrden", x => x.IdItemOrden);
                    table.ForeignKey(
                        name: "FK_ItemOrden_Orden_OrdenIdOrden",
                        column: x => x.OrdenIdOrden,
                        principalTable: "Orden",
                        principalColumn: "IdOrden",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemOrden_Producto_ProductoIdProducto",
                        column: x => x.ProductoIdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemOrden_OrdenIdOrden",
                table: "ItemOrden",
                column: "OrdenIdOrden");

            migrationBuilder.CreateIndex(
                name: "IX_ItemOrden_ProductoIdProducto",
                table: "ItemOrden",
                column: "ProductoIdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Orden_ProveedorIdProveedor",
                table: "Orden",
                column: "ProveedorIdProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_Orden_UsuarioIdUsuario",
                table: "Orden",
                column: "UsuarioIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorProducto_IdProducto",
                table: "ProveedorProducto",
                column: "IdProducto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemOrden");

            migrationBuilder.DropTable(
                name: "ProveedorProducto");

            migrationBuilder.DropTable(
                name: "Orden");
        }
    }
}
