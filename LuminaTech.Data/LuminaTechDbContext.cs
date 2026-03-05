using Microsoft.EntityFrameworkCore;
using LuminaTech.Data.Entities;

namespace LuminaTech.Data;

public class LuminaTechDbContext : DbContext
{
    public LuminaTechDbContext(DbContextOptions<LuminaTechDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();
    public DbSet<ContactSubmission> ContactSubmissions => Set<ContactSubmission>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.GoogleId).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Price).HasColumnType("decimal(10,2)");
        });

        // Wishlist — composite key
        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(w => new { w.UserId, w.ProductId });

            entity.HasOne(w => w.User)
                  .WithMany(u => u.Wishlists)
                  .HasForeignKey(w => w.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(w => w.Product)
                  .WithMany(p => p.Wishlists)
                  .HasForeignKey(w => w.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data — sample products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "LuminaCore X1", Description = "Next-gen quantum processor with neural-link architecture. 128 cores, 5nm fabrication, AI-accelerated compute.", Price = 2999.99m, Category = "Hardware", ImageUrl = "https://res.cloudinary.com/dhgwfh6x0/image/upload/v1772658073/luminatech/product1.png", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Product { Id = 2, Name = "NeuroSync Pro", Description = "AI-powered neural interface for seamless human-machine interaction. Brain-computer integration redefined.", Price = 1499.99m, Category = "Hardware", ImageUrl = "https://res.cloudinary.com/dhgwfh6x0/image/upload/v1772658076/luminatech/product2.png", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Product { Id = 3, Name = "CloudVault Enterprise", Description = "Military-grade encrypted cloud storage with zero-knowledge architecture. Infinite scalability.", Price = 499.99m, Category = "Software", ImageUrl = "https://res.cloudinary.com/dhgwfh6x0/image/upload/v1772658080/luminatech/product3.png", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Product { Id = 4, Name = "QuantumShield", Description = "Post-quantum cryptography suite. Future-proof your security with lattice-based encryption algorithms.", Price = 799.99m, Category = "Software", ImageUrl = "https://res.cloudinary.com/dhgwfh6x0/image/upload/v1772658083/luminatech/product4.png", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Product { Id = 5, Name = "HoloLens Ultra", Description = "Immersive AR/VR headset with 16K resolution per eye. Spatial computing at its finest.", Price = 3499.99m, Category = "Hardware", ImageUrl = "https://res.cloudinary.com/dhgwfh6x0/image/upload/v1772658086/luminatech/product5.png", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Product { Id = 6, Name = "LuminaTech Consulting", Description = "End-to-end digital transformation consulting. Strategy, architecture, and implementation by experts.", Price = 15000.00m, Category = "Service", ImageUrl = "https://res.cloudinary.com/dhgwfh6x0/image/upload/v1772658089/luminatech/product6.png", IsActive = true, CreatedAt = DateTime.UtcNow }
        );

        // Seed users
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, GoogleId = "admin-seed", Email = "admin@luminatech.com", FullName = "LuminaTech Admin", Role = "Admin", IsActive = true, CreatedAt = DateTime.UtcNow },
            new User { Id = 2, GoogleId = "demo-seed", Email = "demo@luminatech.com", FullName = "Demo User", Role = "User", IsActive = true, CreatedAt = DateTime.UtcNow },
            new User { Id = 3, GoogleId = "superadmin-seed", Email = "superadmin@luminatech.com", FullName = "Super Admin", Role = "Admin", IsActive = true, CreatedAt = DateTime.UtcNow }
        );
    }
}
