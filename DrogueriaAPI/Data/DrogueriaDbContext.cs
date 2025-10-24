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
            public DbSet<Proveedor> Proveedores { get; set; }

            public DbSet<ProveedorProducto> ProveedorProductos { get; set; } 
            public DbSet<Orden> Ordenes { get; set; }
            public DbSet<ItemOrden> ItemsOrden { get; set; }
            public DbSet<Direccion> Direcciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Tablas ---
            modelBuilder.Entity<ItemOrden>().ToTable("ItemsOrden");

            // --- Producto ---
            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.CodigoBarras)
                .IsUnique();

            // --- Usuario ---
            modelBuilder.Entity<Usuarios>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);
            });

            // --- Proveedor ---
            modelBuilder.Entity<Proveedor>(entity =>
            {
                entity.HasKey(e => e.IdProveedor);
            });
            modelBuilder.Entity<Proveedor>()
                .HasOne(p => p.Usuario)
                .WithOne()
                .HasForeignKey<Proveedor>(p => p.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Usuarios>()
                .HasOne(u => u.Proveedor)
                .WithOne(p => p.Usuario)
                .HasForeignKey<Proveedor>(p => p.IdProveedor)
                .OnDelete(DeleteBehavior.Restrict); 


            modelBuilder.Entity<ProveedorProducto>()
                .HasKey(pp => new { pp.IdProveedor, pp.IdProducto });

            modelBuilder.Entity<ProveedorProducto>()
                .HasOne(pp => pp.Proveedor)
                .WithMany()
                .HasForeignKey(pp => pp.IdProveedor)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<ProveedorProducto>()
                .HasOne(pp => pp.Producto)
                .WithMany()
                .HasForeignKey(pp => pp.IdProducto)
                .OnDelete(DeleteBehavior.Restrict); 

            // --- ItemOrden ---
            modelBuilder.Entity<ItemOrden>()
                .HasOne(io => io.Producto)
                .WithMany()
                .HasForeignKey(io => io.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ItemOrden>()
                .HasOne(io => io.Orden)
                .WithMany(o => o.Items)
                .HasForeignKey(io => io.IdOrden)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ItemOrden>()
                .Property(io => io.PrecioUnitario)
                .HasPrecision(18, 2);

            // --- Orden ---
            modelBuilder.Entity<Orden>()
                .Property(o => o.MontoTotal)
                .HasPrecision(18, 2);

            // --- ProveedorProducto.Precio ---
            modelBuilder.Entity<ProveedorProducto>()
                .Property(pp => pp.Precio)
                .HasPrecision(18, 2);
            // --- Direccion ---
            modelBuilder.Entity<Usuarios>()
                .HasMany(u => u.Direcciones)
                .WithOne(d => d.Usuario)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Direccion>(entity =>
            {
                entity.Property(d => d.Region).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Comuna).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Calle).IsRequired().HasMaxLength(100);
                entity.Property(d => d.NumeroCalle).IsRequired().HasMaxLength(20);
                entity.Property(d => d.Etiqueta).HasMaxLength(50);
                entity.Property(d => d.Complemento).HasMaxLength(255);
                entity.Property(d => d.EsPrincipal).HasDefaultValue(false);
            });
        }





    }

}
