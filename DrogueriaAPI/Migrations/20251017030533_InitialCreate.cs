using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrogueriaAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Marca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoBarras = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    NombreProducto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrincipioActivo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Concentracion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormaFarmaceutica = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PresentacionComercial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LaboratorioFabricante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistroSanitario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CondicionesAlmacenamiento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.IdProducto);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoEstablecimiento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Direccion3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    IdProveedor = table.Column<int>(type: "int", nullable: false),
                    NombreProveedor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.IdProveedor);
                    table.ForeignKey(
                        name: "FK_Proveedores_Usuario_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ordenes",
                columns: table => new
                {
                    IdOrden = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DireccionEnvioCompleta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdProveedor = table.Column<int>(type: "int", nullable: false),
                    FechaOrden = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadoOrden = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MontoTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NumeroOrdenCompra = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroFactura = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoComprobante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaFactura = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MetodoPago = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Moneda = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Impuestos = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ordenes", x => x.IdOrden);
                    table.ForeignKey(
                        name: "FK_Ordenes_Proveedores_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedores",
                        principalColumn: "IdProveedor",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ordenes_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProveedorProductos",
                columns: table => new
                {
                    IdProveedor = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    ProveedorIdProveedor = table.Column<int>(type: "int", nullable: true),
                    ProductoIdProducto = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProveedorProductos", x => new { x.IdProveedor, x.IdProducto });
                    table.ForeignKey(
                        name: "FK_ProveedorProductos_Producto_ProductoIdProducto",
                        column: x => x.ProductoIdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProveedorProductos_Proveedores_ProveedorIdProveedor",
                        column: x => x.ProveedorIdProveedor,
                        principalTable: "Proveedores",
                        principalColumn: "IdProveedor",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemsOrden",
                columns: table => new
                {
                    IdItemOrden = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrden = table.Column<int>(type: "int", nullable: false),
                    IdProducto = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Impuesto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProductoIdProducto = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsOrden", x => x.IdItemOrden);
                    table.ForeignKey(
                        name: "FK_ItemsOrden_Ordenes_IdOrden",
                        column: x => x.IdOrden,
                        principalTable: "Ordenes",
                        principalColumn: "IdOrden",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemsOrden_Producto_ProductoIdProducto",
                        column: x => x.ProductoIdProducto,
                        principalTable: "Producto",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemsOrden_IdOrden",
                table: "ItemsOrden",
                column: "IdOrden");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsOrden_ProductoIdProducto",
                table: "ItemsOrden",
                column: "ProductoIdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_IdProveedor",
                table: "Ordenes",
                column: "IdProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_IdUsuario",
                table: "Ordenes",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_CodigoBarras",
                table: "Producto",
                column: "CodigoBarras",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorProductos_ProductoIdProducto",
                table: "ProveedorProductos",
                column: "ProductoIdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_ProveedorProductos_ProveedorIdProveedor",
                table: "ProveedorProductos",
                column: "ProveedorIdProveedor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemsOrden");

            migrationBuilder.DropTable(
                name: "ProveedorProductos");

            migrationBuilder.DropTable(
                name: "Ordenes");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
