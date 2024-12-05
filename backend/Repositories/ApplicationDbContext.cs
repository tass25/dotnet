using Microsoft.EntityFrameworkCore;  // Required for EF Core methods
using Microsoft.Extensions.Configuration;  // Required for IConfiguration
using backend.Models;  // Make sure to reference your models namespace
using System.IO;  // For reading appsettings.json

public class ApplicationDbContext : DbContext
{
    // Define DbSet for each of your models (tables)
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductSize> ProductSizes { get; set; }
    public DbSet<User> Users { get; set; }

    // Configure the DbContext to use MySQL
       private readonly string _connectionString;

    // Constructor to inject the connection string via IConfiguration
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _connectionString = configuration.GetConnectionString("UserAppCon");  // Access the connection string from appsettings.json
    }

    // Configure the DbContext to use MySQL
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString)); // Use the connection string
        }
    }

    // Configure relationships and other settings if needed
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>()
        .HasKey(u => u.UserID); 

        // Example: Setting up relationships

    // Setting up Order relationships
    modelBuilder.Entity<Order>()
        .HasOne<User>() // Define the relationship
        .WithMany()
        .HasForeignKey(o => o.UserID)
        .OnDelete(DeleteBehavior.Cascade);

    // Adding default value for DateTime
    modelBuilder.Entity<Order>()
        .Property(o => o.DateTime)
        .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<OrderItem>()
            .HasOne<Order>()
            .WithMany()
            .HasForeignKey(oi => oi.OrderID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne<ProductSize>()
            .WithMany()
            .HasForeignKey(oi => oi.ProductSizeID)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProductSize>()
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(ps => ps.ProductID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
