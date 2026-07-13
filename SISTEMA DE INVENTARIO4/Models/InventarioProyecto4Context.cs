using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SISTEMA_DE_INVENTARIO4.Models;

public partial class InventarioProyecto4Context : DbContext
{
    public InventarioProyecto4Context()
    {
    }

    public InventarioProyecto4Context(DbContextOptions<InventarioProyecto4Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Clientes__3214EC07800F5BC8");

            entity.HasIndex(e => e.Cedula, "UQ__Clientes__B4ADFE38C267BF85").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Cedula)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.IdCodigoF).HasName("PK__Factura__DAEDA809436BE153");

            entity.ToTable("Factura");

            entity.Property(e => e.IdCodigoF)
                .ValueGeneratedNever()
                .HasColumnName("Id_CodigoF");
            entity.Property(e => e.CantidadCompra).HasColumnName("Cantidad_Compra");
            entity.Property(e => e.IdCliente).HasColumnName("Id_Cliente");
            entity.Property(e => e.IdProducto).HasColumnName("Id_Producto");
            entity.Property(e => e.Precio).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdCodigoP).HasName("PK__Producto__DAEDA837FF22603A");

            entity.Property(e => e.IdCodigoP)
                .ValueGeneratedNever()
                .HasColumnName("Id_CodigoP");
            entity.Property(e => e.Iva)
                .HasDefaultValue(16.00m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("IVA");
            entity.Property(e => e.Marca)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NombreProducto)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("Nombre_producto");
            entity.Property(e => e.PrecioCompra)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Precio_Compra");
            entity.Property(e => e.PrecioVenta)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Precio_Venta");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC0799A2AB3C");

            entity.HasIndex(e => e.Username, "UQ__Usuarios__536C85E4A334A140").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Usuarios__A9D105341E15F598").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Rol)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
