using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MinshpWebApp.Dal.Entities;

public partial class MinshpDatabaseContext : DbContext
{
    public MinshpDatabaseContext()
    {
    }

    public MinshpDatabaseContext(DbContextOptions<MinshpDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductFeature> ProductFeatures { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Video> Videos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=MINSHP_Database;Trusted_Connection=True; TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BillingAddress)
                .HasMaxLength(50)
                .HasColumnName("Billing_address");
            entity.Property(e => e.DeliveryAddress).HasColumnName("Delivery_address");
            entity.Property(e => e.FirstName).HasColumnName("First_name");
            entity.Property(e => e.LastName).HasColumnName("Last_name");
            entity.Property(e => e.PhoneNumber).HasColumnName("Phone_number");
        });

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.ToTable("Feature");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.ToTable("Image");

            entity.Property(e => e.IdProduct).HasColumnName("Id_product");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Images)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("FK_Image_Product");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.OrderNumber).HasColumnName("Order_number");

            entity.HasOne(d => d.IdCustomerNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdCustomer)
                .HasConstraintName("FK_Order_Customer");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("FK_Order_Product");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<ProductFeature>(entity =>
        {
            entity.ToTable("Product_feature");

            entity.Property(e => e.IdFeature).HasColumnName("Id_feature");
            entity.Property(e => e.IdProduct).HasColumnName("Id_product");

            entity.HasOne(d => d.IdFeatureNavigation).WithMany(p => p.ProductFeatures)
                .HasForeignKey(d => d.IdFeature)
                .HasConstraintName("FK_Product_feature_Feature");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.ProductFeatures)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("FK_Product_feature_Product");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.ToTable("Promotion");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.IdProduct).HasColumnName("Id_product");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Promotions)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("FK_Promotion_Product");
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.ToTable("Video");

            entity.Property(e => e.IdProduct).HasColumnName("Id_product");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Videos)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("FK_Video_Product");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
