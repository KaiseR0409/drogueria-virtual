using Microsoft.EntityFrameworkCore;
using DrogueriaAPI.Models;

namespace DrogueriaAPI.Data
{
        public class DrogueriaDbContext : DbContext
        {
            public DrogueriaDbContext(DbContextOptions<DrogueriaDbContext> options) : base(options)
            {
            }
            public DbSet<Producto> Productos { get; set; }
            public DbSet<Usuarios> Usuarios { get; set; }
            public DbSet<Proveedor> Proveedor { get; set; }
            public DbSet<Orden> Orden { get; set; } 
            public DbSet<ItemOrden> ItemOrden { get; set; } 
            public DbSet<ProveedorProducto> ProveedorProducto { get; set; } 
            public DbSet<Orden> Ordenes { get; set; }
            public DbSet<ItemOrden> ItemsOrden { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // --- INICIO DE LA CONFIGURACIÓN ---

                // Configura la entidad Usuario para su clave primaria
                modelBuilder.Entity<Usuarios>(entity =>
                {
                    entity.HasKey(e => e.IdUsuario); // Le decimos a EF que IdUsuario es la PK
                });

                // Configura la entidad Proveedor
                modelBuilder.Entity<Proveedor>(entity =>
                {
                    entity.HasKey(e => e.IdProveedor); // Le decimos a EF que IdProveedor es la PK
                });

                // Configuración de la relación uno a uno entre Usuario y Proveedor
                modelBuilder.Entity<Usuarios>()
                    .HasOne(usuario => usuario.Proveedor)       // Un Usuario tiene (opcionalmente) un Proveedor
                    .WithOne(proveedor => proveedor.Usuario)      // Un Proveedor está asociado a un único Usuario
                    .HasForeignKey<Proveedor>(proveedor => proveedor.IdProveedor); // La clave foránea está en la tabla Proveedor y es la columna IdProveedor

            

                // Configuración de la CLAVE COMPUESTA para la tabla de unión ProveedorProducto
                modelBuilder.Entity<ProveedorProducto>()
                    .HasKey(pp => new { pp.IdProveedor, pp.IdProducto }); // Las dos columnas juntas son la clave.

                // Configuración de las Relaciones (que dependen de la clave compuesta):
                modelBuilder.Entity<ProveedorProducto>()
                    .HasOne(pp => pp.Proveedor)
                    .WithMany()
                    .HasForeignKey(pp => pp.IdProveedor);

                modelBuilder.Entity<ProveedorProducto>()
                    .HasOne(pp => pp.Producto)
                    .WithMany()
                    .HasForeignKey(pp => pp.IdProducto);

                modelBuilder.Entity<ItemOrden>()
                    .HasOne(io => io.Producto)
                    .WithMany()
                    .HasForeignKey(io => io.IdProducto);

                modelBuilder.Entity<ItemOrden>()
                    .HasOne(io => io.Orden)
                    .WithMany(o => o.Items)
                    .HasForeignKey(io => io.IdOrden);
                modelBuilder.Entity<ItemOrden>()
                    .Property(io => io.PrecioUnitario)
                    .HasPrecision(18, 2);

                modelBuilder.Entity<Orden>()
                    .Property(o => o.MontoTotal)
                    .HasPrecision(18, 2);

                modelBuilder.Entity<ProveedorProducto>()
                    .Property(pp => pp.Precio)
                    .HasPrecision(18, 2);

                modelBuilder.Entity<ItemOrden>()
                    .HasOne(i => i.Orden)
                    .WithMany(o => o.Items)
                    .HasForeignKey(i => i.IdOrden);

                modelBuilder.Entity<ItemOrden>()
                    .HasOne(i => i.Producto)
                    .WithMany()
                    .HasForeignKey(i => i.IdProducto);
            // --- FIN DE LA CONFIGURACIÓN ---

        }
    }

}
