using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace azure.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Point> Points { get; set; }

    public virtual DbSet<Tovar> Tovars { get; set; }

    public virtual DbSet<User> Users { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
// #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//         => optionsBuilder.UseMySql("server=localhost;user=root;password=0305ramazan;database=azure", Microsoft.EntityFrameworkCore.ServerVersion.Parse("9.5.0-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("brands");

            entity.HasIndex(e => e.Title, "title_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title)
                .HasMaxLength(90)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("categories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title)
                .HasMaxLength(45)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.IdPoint, "point_idx");

            entity.HasIndex(e => e.IdStatus, "status_idx");

            entity.HasIndex(e => e.IdUser, "user_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DeliveryDate).HasColumnName("delivery_date");
            entity.Property(e => e.IdPoint).HasColumnName("id_point");
            entity.Property(e => e.IdStatus).HasColumnName("id_status");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.PaymentType)
                .HasColumnType("enum('При получении','Онлайн')")
                .HasColumnName("payment_type");
            entity.Property(e => e.TotalPrice).HasColumnName("total_price");

            entity.HasOne(d => d.IdPointNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdPoint)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("point");

            entity.HasOne(d => d.IdStatusNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdStatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("status");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("order_items");

            entity.HasIndex(e => e.IdOrder, "order_idx");

            entity.HasIndex(e => e.IdTovar, "tovat_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdOrder).HasColumnName("id_order");
            entity.Property(e => e.IdTovar).HasColumnName("id_tovar");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.IdOrderNavigation).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.IdOrder)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order");

            entity.HasOne(d => d.IdTovarNavigation).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.IdTovar)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tovat");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("order_statuses");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title)
                .HasMaxLength(45)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Point>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("points");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.City)
                .HasMaxLength(45)
                .HasColumnName("city");
            entity.Property(e => e.HomeNumber).HasColumnName("home_number");
            entity.Property(e => e.Street)
                .HasMaxLength(45)
                .HasColumnName("street");
        });

        modelBuilder.Entity<Tovar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tovars");

            entity.HasIndex(e => e.IdBrand, "brand_idx");

            entity.HasIndex(e => e.IdCategory, "cat_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Article)
                .HasMaxLength(16)
                .HasColumnName("article");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IdBrand).HasColumnName("id_brand");
            entity.Property(e => e.IdCategory).HasColumnName("id_category");
            entity.Property(e => e.Photo)
                .HasMaxLength(255)
                .HasColumnName("photo");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status)
                .HasColumnType("enum('Активен','Удален')")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(45)
                .HasColumnName("title");

            entity.HasOne(d => d.IdBrandNavigation).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.IdBrand)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("brand");

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.IdCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cat");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(45)
                .HasColumnName("email");
            entity.Property(e => e.Login)
                .HasMaxLength(45)
                .HasColumnName("login");
            entity.Property(e => e.Name)
                .HasMaxLength(45)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(16)
                .HasColumnName("password");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(45)
                .HasColumnName("patronymic");
            entity.Property(e => e.Status)
                .HasColumnType("enum('Активный','Неактивный')")
                .HasColumnName("status");
            entity.Property(e => e.Role)
                .HasColumnType("enum('Клиент','Администратор')")
                .HasColumnName("role");
            entity.Property(e => e.Surname)
                .HasMaxLength(45)
                .HasColumnName("surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}