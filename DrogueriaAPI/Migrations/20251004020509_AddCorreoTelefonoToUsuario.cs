using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DrogueriaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCorreoTelefonoToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "fechaCreacion",
                table: "Usuario",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "fechaActualizacion",
                table: "Usuario",
                newName: "FechaActualizacion");

            migrationBuilder.RenameColumn(
                name: "direccion3",
                table: "Usuario",
                newName: "Direccion3");

            migrationBuilder.RenameColumn(
                name: "direccion2",
                table: "Usuario",
                newName: "Direccion2");

            migrationBuilder.RenameColumn(
                name: "direccion1",
                table: "Usuario",
                newName: "Direccion1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Usuario",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Usuario",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Direccion3",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Direccion2",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Correo",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "NumeroOrdenCompra",
                table: "Orden",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Correo",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Usuario");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "Usuario",
                newName: "fechaCreacion");

            migrationBuilder.RenameColumn(
                name: "FechaActualizacion",
                table: "Usuario",
                newName: "fechaActualizacion");

            migrationBuilder.RenameColumn(
                name: "Direccion3",
                table: "Usuario",
                newName: "direccion3");

            migrationBuilder.RenameColumn(
                name: "Direccion2",
                table: "Usuario",
                newName: "direccion2");

            migrationBuilder.RenameColumn(
                name: "Direccion1",
                table: "Usuario",
                newName: "direccion1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "fechaCreacion",
                table: "Usuario",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "fechaActualizacion",
                table: "Usuario",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "direccion3",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "direccion2",
                table: "Usuario",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NumeroOrdenCompra",
                table: "Orden",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
