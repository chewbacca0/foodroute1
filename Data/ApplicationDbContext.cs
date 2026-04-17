using Microsoft.EntityFrameworkCore;
using FoodRoute.Models;

namespace FoodRoute.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<FoodItem> FoodItems { get; set; }
    public DbSet<UserSelection> UserSelections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(500);
        });

        modelBuilder.Entity<FoodItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ImageUrl).IsRequired();
            entity.Property(e => e.Tags).HasMaxLength(500);
            entity.Property(e => e.MealType).HasMaxLength(50);
            
            entity.HasOne(e => e.Restaurant)
                .WithMany(r => r.FoodItems)
                .HasForeignKey(e => e.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserSelection>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SessionId).IsRequired().HasMaxLength(100);
            
            entity.HasOne(e => e.FoodItem)
                .WithMany()
                .HasForeignKey(e => e.FoodItemId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
