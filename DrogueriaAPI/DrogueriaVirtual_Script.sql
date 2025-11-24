IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Producto] (
    [IdProducto] int NOT NULL IDENTITY,
    [Marca] nvarchar(max) NOT NULL,
    [CodigoBarras] nvarchar(12) NOT NULL,
    [NombreProducto] nvarchar(max) NOT NULL,
    [PrincipioActivo] nvarchar(max) NOT NULL,
    [Concentracion] nvarchar(max) NOT NULL,
    [FormaFarmaceutica] nvarchar(max) NOT NULL,
    [PresentacionComercial] nvarchar(max) NOT NULL,
    [LaboratorioFabricante] nvarchar(max) NOT NULL,
    [RegistroSanitario] nvarchar(max) NOT NULL,
    [FechaVencimiento] datetime2 NULL,
    [CondicionesAlmacenamiento] nvarchar(max) NOT NULL,
    [ImagenUrl] nvarchar(max) NULL,
    CONSTRAINT [PK_Producto] PRIMARY KEY ([IdProducto])
);

CREATE TABLE [Usuario] (
    [IdUsuario] int NOT NULL IDENTITY,
    [NombreUsuario] nvarchar(max) NOT NULL,
    [Usuario] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NULL,
    [TipoUsuario] nvarchar(max) NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaActualizacion] datetime2 NOT NULL,
    [TipoEstablecimiento] nvarchar(max) NOT NULL,
    [Direccion1] nvarchar(max) NOT NULL,
    [Direccion2] nvarchar(max) NULL,
    [Direccion3] nvarchar(max) NULL,
    [EstadoUsuario] nvarchar(max) NOT NULL,
    [Correo] nvarchar(max) NOT NULL,
    [Telefono] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Usuario] PRIMARY KEY ([IdUsuario])
);

CREATE TABLE [Proveedores] (
    [IdProveedor] int NOT NULL,
    [NombreProveedor] nvarchar(max) NULL,
    CONSTRAINT [PK_Proveedores] PRIMARY KEY ([IdProveedor]),
    CONSTRAINT [FK_Proveedores_Usuario_IdProveedor] FOREIGN KEY ([IdProveedor]) REFERENCES [Usuario] ([IdUsuario]) ON DELETE NO ACTION
);

CREATE TABLE [Ordenes] (
    [IdOrden] int NOT NULL IDENTITY,
    [DireccionEnvioCompleta] nvarchar(500) NULL,
    [IdUsuario] int NOT NULL,
    [IdProveedor] int NOT NULL,
    [FechaOrden] datetime2 NOT NULL,
    [EstadoOrden] nvarchar(max) NOT NULL,
    [MontoTotal] decimal(18,2) NOT NULL,
    [NumeroOrdenCompra] nvarchar(max) NULL,
    [NumeroFactura] nvarchar(max) NOT NULL,
    [TipoComprobante] nvarchar(max) NOT NULL,
    [FechaFactura] datetime2 NULL,
    [MetodoPago] nvarchar(max) NOT NULL,
    [Moneda] nvarchar(max) NOT NULL,
    [Impuestos] decimal(18,2) NOT NULL,
    [Descuento] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Ordenes] PRIMARY KEY ([IdOrden]),
    CONSTRAINT [FK_Ordenes_Proveedores_IdProveedor] FOREIGN KEY ([IdProveedor]) REFERENCES [Proveedores] ([IdProveedor]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Ordenes_Usuario_IdUsuario] FOREIGN KEY ([IdUsuario]) REFERENCES [Usuario] ([IdUsuario]) ON DELETE NO ACTION
);

CREATE TABLE [ProveedorProductos] (
    [IdProveedor] int NOT NULL,
    [IdProducto] int NOT NULL,
    [Precio] decimal(18,2) NOT NULL,
    [Stock] int NOT NULL,
    [ProveedorIdProveedor] int NULL,
    [ProductoIdProducto] int NULL,
    CONSTRAINT [PK_ProveedorProductos] PRIMARY KEY ([IdProveedor], [IdProducto]),
    CONSTRAINT [FK_ProveedorProductos_Producto_ProductoIdProducto] FOREIGN KEY ([ProductoIdProducto]) REFERENCES [Producto] ([IdProducto]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProveedorProductos_Proveedores_ProveedorIdProveedor] FOREIGN KEY ([ProveedorIdProveedor]) REFERENCES [Proveedores] ([IdProveedor]) ON DELETE NO ACTION
);

CREATE TABLE [ItemsOrden] (
    [IdItemOrden] int NOT NULL IDENTITY,
    [IdOrden] int NOT NULL,
    [IdProducto] int NOT NULL,
    [Cantidad] int NOT NULL,
    [PrecioUnitario] decimal(18,2) NOT NULL,
    [Subtotal] decimal(18,2) NOT NULL,
    [Impuesto] decimal(18,2) NOT NULL,
    [Descuento] decimal(18,2) NOT NULL,
    [ProductoIdProducto] int NOT NULL,
    CONSTRAINT [PK_ItemsOrden] PRIMARY KEY ([IdItemOrden]),
    CONSTRAINT [FK_ItemsOrden_Ordenes_IdOrden] FOREIGN KEY ([IdOrden]) REFERENCES [Ordenes] ([IdOrden]) ON DELETE CASCADE,
    CONSTRAINT [FK_ItemsOrden_Producto_ProductoIdProducto] FOREIGN KEY ([ProductoIdProducto]) REFERENCES [Producto] ([IdProducto]) ON DELETE NO ACTION
);

CREATE INDEX [IX_ItemsOrden_IdOrden] ON [ItemsOrden] ([IdOrden]);

CREATE INDEX [IX_ItemsOrden_ProductoIdProducto] ON [ItemsOrden] ([ProductoIdProducto]);

CREATE INDEX [IX_Ordenes_IdProveedor] ON [Ordenes] ([IdProveedor]);

CREATE INDEX [IX_Ordenes_IdUsuario] ON [Ordenes] ([IdUsuario]);

CREATE UNIQUE INDEX [IX_Producto_CodigoBarras] ON [Producto] ([CodigoBarras]);

CREATE INDEX [IX_ProveedorProductos_ProductoIdProducto] ON [ProveedorProductos] ([ProductoIdProducto]);

CREATE INDEX [IX_ProveedorProductos_ProveedorIdProveedor] ON [ProveedorProductos] ([ProveedorIdProveedor]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251017030533_InitialCreate', N'9.0.9');

ALTER TABLE [ItemsOrden] DROP CONSTRAINT [FK_ItemsOrden_Ordenes_IdOrden];

ALTER TABLE [ItemsOrden] DROP CONSTRAINT [FK_ItemsOrden_Producto_ProductoIdProducto];

ALTER TABLE [Ordenes] DROP CONSTRAINT [FK_Ordenes_Proveedores_IdProveedor];

ALTER TABLE [Ordenes] DROP CONSTRAINT [FK_Ordenes_Usuario_IdUsuario];

ALTER TABLE [ProveedorProductos] DROP CONSTRAINT [FK_ProveedorProductos_Producto_ProductoIdProducto];

ALTER TABLE [ProveedorProductos] DROP CONSTRAINT [FK_ProveedorProductos_Proveedores_ProveedorIdProveedor];

DROP INDEX [IX_ProveedorProductos_ProveedorIdProveedor] ON [ProveedorProductos];

DECLARE @var sysname;
SELECT @var = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProveedorProductos]') AND [c].[name] = N'ProveedorIdProveedor');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [ProveedorProductos] DROP CONSTRAINT [' + @var + '];');
ALTER TABLE [ProveedorProductos] DROP COLUMN [ProveedorIdProveedor];

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ItemsOrden]') AND [c].[name] = N'ProductoIdProducto');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ItemsOrden] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [ItemsOrden] ALTER COLUMN [ProductoIdProducto] int NULL;

CREATE INDEX [IX_ProveedorProductos_IdProducto] ON [ProveedorProductos] ([IdProducto]);

CREATE INDEX [IX_ItemsOrden_IdProducto] ON [ItemsOrden] ([IdProducto]);

ALTER TABLE [ItemsOrden] ADD CONSTRAINT [FK_ItemsOrden_Ordenes_IdOrden] FOREIGN KEY ([IdOrden]) REFERENCES [Ordenes] ([IdOrden]) ON DELETE NO ACTION;

ALTER TABLE [ItemsOrden] ADD CONSTRAINT [FK_ItemsOrden_Producto_IdProducto] FOREIGN KEY ([IdProducto]) REFERENCES [Producto] ([IdProducto]) ON DELETE NO ACTION;

ALTER TABLE [ItemsOrden] ADD CONSTRAINT [FK_ItemsOrden_Producto_ProductoIdProducto] FOREIGN KEY ([ProductoIdProducto]) REFERENCES [Producto] ([IdProducto]);

ALTER TABLE [Ordenes] ADD CONSTRAINT [FK_Ordenes_Proveedores_IdProveedor] FOREIGN KEY ([IdProveedor]) REFERENCES [Proveedores] ([IdProveedor]) ON DELETE CASCADE;

ALTER TABLE [Ordenes] ADD CONSTRAINT [FK_Ordenes_Usuario_IdUsuario] FOREIGN KEY ([IdUsuario]) REFERENCES [Usuario] ([IdUsuario]) ON DELETE CASCADE;

ALTER TABLE [ProveedorProductos] ADD CONSTRAINT [FK_ProveedorProductos_Producto_IdProducto] FOREIGN KEY ([IdProducto]) REFERENCES [Producto] ([IdProducto]) ON DELETE NO ACTION;

ALTER TABLE [ProveedorProductos] ADD CONSTRAINT [FK_ProveedorProductos_Producto_ProductoIdProducto] FOREIGN KEY ([ProductoIdProducto]) REFERENCES [Producto] ([IdProducto]);

ALTER TABLE [ProveedorProductos] ADD CONSTRAINT [FK_ProveedorProductos_Proveedores_IdProveedor] FOREIGN KEY ([IdProveedor]) REFERENCES [Proveedores] ([IdProveedor]) ON DELETE NO ACTION;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251017234040_InitialClean', N'9.0.9');

ALTER TABLE [ProveedorProductos] ADD [ProveedorIdProveedor] int NULL;

ALTER TABLE [Proveedores] ADD [IdUsuario] int NOT NULL DEFAULT 0;

CREATE INDEX [IX_ProveedorProductos_ProveedorIdProveedor] ON [ProveedorProductos] ([ProveedorIdProveedor]);

ALTER TABLE [ProveedorProductos] ADD CONSTRAINT [FK_ProveedorProductos_Proveedores_ProveedorIdProveedor] FOREIGN KEY ([ProveedorIdProveedor]) REFERENCES [Proveedores] ([IdProveedor]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251017235518_FixProveedorUsuarioRelation', N'9.0.9');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251018210301_AjusteProveedorFactura', N'9.0.9');

ALTER TABLE [Proveedores] ADD [Ciudad] nvarchar(max) NULL;

ALTER TABLE [Proveedores] ADD [DireccionComercial] nvarchar(max) NULL;

ALTER TABLE [Proveedores] ADD [Giro] nvarchar(max) NULL;

ALTER TABLE [Proveedores] ADD [RUT] nvarchar(max) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251018212322_AgregarCamposProveedorAFactura', N'9.0.9');

ALTER TABLE [Producto] ADD [Clase] nvarchar(max) NOT NULL DEFAULT N'';

ALTER TABLE [Producto] ADD [Familia] nvarchar(max) NOT NULL DEFAULT N'';

ALTER TABLE [Producto] ADD [RegistroISP] int NOT NULL DEFAULT 0;

ALTER TABLE [Producto] ADD [ViaAdministracion] nvarchar(max) NOT NULL DEFAULT N'';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251022233636_AddCamposExtraAProducto', N'9.0.9');

CREATE TABLE [Direcciones] (
    [IdDireccion] int NOT NULL IDENTITY,
    [IdUsuario] int NOT NULL,
    [Etiqueta] nvarchar(50) NULL,
    [Region] nvarchar(100) NOT NULL,
    [Comuna] nvarchar(100) NOT NULL,
    [Calle] nvarchar(100) NOT NULL,
    [NumeroCalle] nvarchar(20) NOT NULL,
    [Complemento] nvarchar(255) NULL,
    [EsPrincipal] bit NOT NULL DEFAULT CAST(0 AS bit),
    CONSTRAINT [PK_Direcciones] PRIMARY KEY ([IdDireccion]),
    CONSTRAINT [FK_Direcciones_Usuario_IdUsuario] FOREIGN KEY ([IdUsuario]) REFERENCES [Usuario] ([IdUsuario]) ON DELETE CASCADE
);

CREATE INDEX [IX_Direcciones_IdUsuario] ON [Direcciones] ([IdUsuario]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251023233816_AddDireccionesRelacionUsuarios', N'9.0.9');

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuario]') AND [c].[name] = N'Direccion1');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Usuario] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Usuario] DROP COLUMN [Direccion1];

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuario]') AND [c].[name] = N'Direccion2');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Usuario] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [Usuario] DROP COLUMN [Direccion2];

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Usuario]') AND [c].[name] = N'Direccion3');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Usuario] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [Usuario] DROP COLUMN [Direccion3];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251024000541_AddDireccionesRelacionUsuariosDeteleDireccion123', N'9.0.9');

COMMIT;
GO

