using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.IdentityAPI.DAL.Models;

public partial class EcommerceContext : DbContext
{
    public EcommerceContext()
    {
    }

    public EcommerceContext(DbContextOptions<EcommerceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=PAVLE-PC\\SQLSERVER2016;Initial Catalog=ECommerce;TrustServerCertificate=True;User ID=sa;Password=1234");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.IdCart);

            entity.ToTable("Cart");

            entity.Property(e => e.DateCartCreated).HasColumnType("date");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cart_User");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.IdCartItem);

            entity.ToTable("CartItem");

            entity.Property(e => e.CustomRequest)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.DateItemAdded).HasColumnType("date");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 4)");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItem_Product");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.ToTable("Log");

            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ActionDateTime).HasColumnType("date");
            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct);

            entity.ToTable("Product");

            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Tchanged)
                .HasColumnType("date")
                .HasColumnName("TChanged");
            entity.Property(e => e.Uchanged).HasColumnName("UChanged");

            entity.HasOne(d => d.IdProductTypeNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.IdProductType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_ProductType");

            entity.HasOne(d => d.UchangedNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Uchanged)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_User");
        });

        modelBuilder.Entity<ProductType>(entity =>
        {
            entity.HasKey(e => e.IdProductType);

            entity.ToTable("ProductType");

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Tchanged)
                .HasColumnType("date")
                .HasColumnName("TChanged");
            entity.Property(e => e.Uchanged).HasColumnName("UChanged");

            entity.HasOne(d => d.UchangedNavigation).WithMany(p => p.ProductTypes)
                .HasForeignKey(d => d.Uchanged)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductType_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole);

            entity.ToTable("Role");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Tchanged)
                .HasColumnType("date")
                .HasColumnName("TChanged");
            entity.Property(e => e.Uchanged).HasColumnName("UChanged");

            entity.HasOne(d => d.UchangedNavigation).WithMany(p => p.Roles)
                .HasForeignKey(d => d.Uchanged)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Role_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser);

            entity.ToTable("User");

            entity.HasIndex(e => e.Username, "UQ_User").IsUnique();

            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.CountryAlpha3Code)
                .HasMaxLength(3)
                .IsUnicode(false);
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(320)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.Postcode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Tchanged)
                .HasColumnType("date")
                .HasColumnName("TChanged");
            entity.Property(e => e.Uchanged).HasColumnName("UChanged");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.IdUserRole);

            entity.ToTable("UserRole");

            entity.Property(e => e.Tcreated)
                .HasColumnType("date")
                .HasColumnName("TCreated");
            entity.Property(e => e.Ucreated).HasColumnName("UCreated");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_Role");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserRoleIdUserNavigations)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_User");

            entity.HasOne(d => d.UcreatedNavigation).WithMany(p => p.UserRoleUcreatedNavigations)
                .HasForeignKey(d => d.Ucreated)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_User2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
