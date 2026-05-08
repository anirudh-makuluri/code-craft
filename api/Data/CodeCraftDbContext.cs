using api.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class CodeCraftDbContext(DbContextOptions<CodeCraftDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Craft> Crafts => Set<Craft>();
    public DbSet<CraftLike> CraftLikes => Set<CraftLike>();
    public DbSet<CraftView> CraftViews => Set<CraftView>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Craft>()
            .HasIndex(c => c.CraftId)
            .IsUnique();

        modelBuilder.Entity<Craft>()
            .HasOne(c => c.CreatedByUser)
            .WithMany(u => u.Crafts)
            .HasForeignKey(c => c.CreatedByUsername)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CraftLike>()
            .HasIndex(l => new { l.CraftId, l.Username })
            .IsUnique();

        modelBuilder.Entity<CraftLike>()
            .HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.Username)
            .HasPrincipalKey(u => u.Username)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CraftView>()
            .HasIndex(v => v.CraftId);
    }
}
