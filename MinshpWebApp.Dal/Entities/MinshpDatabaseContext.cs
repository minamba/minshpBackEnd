using Microsoft.EntityFrameworkCore;
using MinshpConsolApp.Models;
using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class MinshpDatabaseContext : DbContext
{
    //public MinshpDatabaseContext()
    //{
    //}

    public MinshpDatabaseContext(DbContextOptions<MinshpDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<BillingAddress> BillingAddresses { get; set; }
    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<FeatureCategory> FeatureCategories { get; set; }

    public virtual DbSet<DeliveryAddress> DeliveryAddresses { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductFeature> ProductFeatures { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<PromotionCode> PromotionCodes { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Taxe> Taxes { get; set; }
    public virtual DbSet<Video> Videos { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=MINSHP_Database;Trusted_Connection=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.ToTable("Application");

            entity.Property(e => e.DisplayNewProductNumber).HasColumnName("Display_new_product_number");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_date");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_date");
        });


        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });



        modelBuilder.Entity<BillingAddress>(entity =>
        {
            entity.ToTable("Billing_address");

            entity.Property(e => e.ComplementaryAddress).HasColumnName("Complementary_address");
            entity.Property(e => e.IdCustomer).HasColumnName("Id_customer");
            entity.Property(e => e.PostalCode).HasColumnName("Postal_code");

            entity.HasOne(d => d.IdCustomerNavigation).WithMany(p => p.BillingAddresses)
                .HasForeignKey(d => d.IdCustomer)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Billing_address_Customer");
        });




        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");
            entity.Property(e => e.IdPromotionCode).HasColumnName("Id_promotion_code");
            entity.Property(e => e.IdTaxe).HasColumnName("Id_taxe");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.Property(e => e.BirthDate)
                .HasColumnType("datetime")
                .HasColumnName("Birth_date");
            entity.Property(e => e.Civilite).HasMaxLength(50);
            entity.Property(e => e.ClientNumber1)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasComputedColumnSql("('CL'+right(replicate('0',(9))+CONVERT([varchar](50),[ClientNumberInt]),(9)))", true)
                .HasColumnName("ClientNumber");
            entity.Property(e => e.ClientNumberInt).HasDefaultValueSql("(NEXT VALUE FOR [dbo].[ClientNumberSeq])");
            entity.Property(e => e.FirstName).HasColumnName("First_name");
            entity.Property(e => e.IdAspNetUser)
                .HasMaxLength(450)
                .HasColumnName("Id_asp_net_user");
            entity.Property(e => e.LastName).HasColumnName("Last_name");
            entity.Property(e => e.PhoneNumber).HasColumnName("Phone_number");

            entity.HasOne(d => d.IdAspNetUserNavigation).WithMany(p => p.Customers)
                .HasForeignKey(d => d.IdAspNetUser)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Customer_AspNetUsers");
        });


        modelBuilder.Entity<DeliveryAddress>(entity =>
        {
            entity.ToTable("Delivery_address");

            entity.Property(e => e.ComplementaryAddress).HasColumnName("Complementary_address");
            entity.Property(e => e.IdCustomer).HasColumnName("Id_customer");
            entity.Property(e => e.PostalCode).HasColumnName("Postal_code");
            entity.Property(e => e.FirstName).HasColumnName("First_name");
            entity.Property(e => e.LastName).HasColumnName("Last_name");
            entity.Property(e => e.Phone).HasColumnName("Phone");

            entity.HasOne(d => d.IdCustomerNavigation).WithMany(p => p.DeliveryAddresses)
                .HasForeignKey(d => d.IdCustomer)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Delivery_address_Customer");
        });



        modelBuilder.Entity<EfmigrationsHistoryAuth>(entity =>
        {
            entity.HasKey(e => e.MigrationId);

            entity.ToTable("__EFMigrationsHistory_Auth", "auth");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });

        modelBuilder.Entity<EfmigrationsHistoryBusiness>(entity =>
        {
            entity.HasKey(e => e.MigrationId);

            entity.ToTable("__EFMigrationsHistory_Business");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });


        modelBuilder.Entity<Feature>(entity =>
        {
            entity.ToTable("Feature");

            entity.Property(e => e.IdCategory).HasColumnName("Id_category");
            entity.Property(e => e.IdFeatureCategory).HasColumnName("Id_feature_category");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Features)
                .HasForeignKey(d => d.IdCategory)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Feature_Category");
        });

        modelBuilder.Entity<FeatureCategory>(entity =>
        {
            entity.ToTable("Feature_category");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.ToTable("Image");

            entity.Property(e => e.Id_product).HasColumnName("Id_product");
            entity.Property(e => e.IdCategory).HasColumnName("Id_category");
        });


        modelBuilder.Entity<Invoice>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Invoice");

            entity.HasIndex(e => e.InvoiceNumber1, "UX_Invoices_InvoiceNumber").IsUnique();

            entity.Property(e => e.CustomerId).HasColumnName("Customer_id");
            entity.Property(e => e.DateCreation)
                .HasColumnType("datetime")
                .HasColumnName("Date_creation");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.InvoiceNumber).HasColumnName("Invoice_number");
            entity.Property(e => e.InvoiceNumber1)
                .HasMaxLength(11)
                .IsUnicode(false)
                .HasComputedColumnSql("('FA'+right(replicate('0',(9))+CONVERT([varchar](20),[InvoiceNumberInt]),(9)))", true)
                .HasColumnName("InvoiceNumber");
            entity.Property(e => e.InvoiceNumberInt).HasDefaultValueSql("(NEXT VALUE FOR [dbo].[InvoiceNumberSeq])");
            entity.Property(e => e.OrderId).HasColumnName("Order_id");

            entity.HasOne(d => d.Customer).WithMany()
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Invoice_Customer");

            entity.HasOne(d => d.Order).WithMany()
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_Invoice_Order");
        });


        modelBuilder.Entity<OpenIddictApplication>(entity =>
        {
            entity.HasIndex(e => e.ClientId, "IX_OpenIddictApplications_ClientId")
                .IsUnique()
                .HasFilter("([ClientId] IS NOT NULL)");

            entity.Property(e => e.ApplicationType).HasMaxLength(50);
            entity.Property(e => e.ClientId).HasMaxLength(100);
            entity.Property(e => e.ClientType).HasMaxLength(50);
            entity.Property(e => e.ConcurrencyToken).HasMaxLength(50);
            entity.Property(e => e.ConsentType).HasMaxLength(50);
        });

        modelBuilder.Entity<OpenIddictAuthorization>(entity =>
        {
            entity.HasIndex(e => new { e.ApplicationId, e.Status, e.Subject, e.Type }, "IX_OpenIddictAuthorizations_ApplicationId_Status_Subject_Type");

            entity.Property(e => e.ConcurrencyToken).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Subject).HasMaxLength(400);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Application).WithMany(p => p.OpenIddictAuthorizations).HasForeignKey(d => d.ApplicationId);
        });

        modelBuilder.Entity<OpenIddictScope>(entity =>
        {
            entity.HasIndex(e => e.Name, "IX_OpenIddictScopes_Name")
                .IsUnique()
                .HasFilter("([Name] IS NOT NULL)");

            entity.Property(e => e.ConcurrencyToken).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<OpenIddictToken>(entity =>
        {
            entity.HasIndex(e => new { e.ApplicationId, e.Status, e.Subject, e.Type }, "IX_OpenIddictTokens_ApplicationId_Status_Subject_Type");

            entity.HasIndex(e => e.AuthorizationId, "IX_OpenIddictTokens_AuthorizationId");

            entity.HasIndex(e => e.ReferenceId, "IX_OpenIddictTokens_ReferenceId")
                .IsUnique()
                .HasFilter("([ReferenceId] IS NOT NULL)");

            entity.Property(e => e.ConcurrencyToken).HasMaxLength(50);
            entity.Property(e => e.ReferenceId).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Subject).HasMaxLength(400);
            entity.Property(e => e.Type).HasMaxLength(150);

            entity.HasOne(d => d.Application).WithMany(p => p.OpenIddictTokens).HasForeignKey(d => d.ApplicationId);

            entity.HasOne(d => d.Authorization).WithMany(p => p.OpenIddictTokens).HasForeignKey(d => d.AuthorizationId);
        });


        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.OrderNumber).HasColumnName("Order_number");
            entity.Property(e => e.PaymentMethod).HasColumnName("Payment_method");

            entity.HasOne(d => d.IdCustomerNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdCustomer)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Order_Customer");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Id_Category).HasColumnName("Id_Category");
            entity.Property(e => e.IdPromotionCode).HasColumnName("Id_promotion_code");
            entity.Property(e => e.CreationDate)
                .HasColumnType("datetime")
                .HasColumnName("Creation_Date");
            entity.Property(e => e.ModificationDate)
                .HasColumnType("datetime")
                .HasColumnName("Modification_Date");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Products)
                .HasForeignKey(d => d.Id_Category)
                .HasConstraintName("FK_Product_Category");
        });

        modelBuilder.Entity<ProductFeature>(entity =>
        {
            entity.ToTable("Product_feature");

            entity.Property(e => e.Id_feature).HasColumnName("Id_feature");
            entity.Property(e => e.IdProduct).HasColumnName("Id_product");

            entity.HasOne(d => d.IdFeatureNavigation).WithMany(p => p.ProductFeatures)
               .HasForeignKey(d => d.Id_feature)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("FK_Product_feature_Feature");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.ProductFeatures)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Product_feature_Product");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.ToTable("Promotion");

            entity.Property(e => e.DateCreation)
                .HasColumnType("datetime")
                .HasColumnName("Date_creation");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Id_product).HasColumnName("Id_product");
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Promotions)
                .HasForeignKey(d => d.Id_product)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Promotion_Product");
        });


        modelBuilder.Entity<PromotionCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Promo_code");

            entity.ToTable("Promotion_code");

            entity.Property(e => e.DateCreation)
                .HasColumnType("datetime")
                .HasColumnName("Date_creation");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_date");
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_date");
        });


        modelBuilder.Entity<Stock>(entity =>
        {
            entity.ToTable("Stock");

            entity.Property(e => e.IdProduct).HasColumnName("Id_product");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Stock_Product");
        });


        modelBuilder.Entity<Taxe>(entity =>
        {
            entity.ToTable("Taxe");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
        });


        modelBuilder.Entity<Video>(entity =>
        {
            entity.ToTable("Video");

            entity.Property(e => e.Id_product).HasColumnName("Id_product");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.Videos)
                .HasForeignKey(d => d.Id_product)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Video_Product");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
