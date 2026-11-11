using Microsoft.EntityFrameworkCore;
using StrategyMate.Models;

namespace StrategyMate.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<IndustryGroup> IndustryGroups { get; set; }
    public DbSet<IndustrySubGroup> IndustrySubGroups { get; set; }
    public DbSet<Symbol> Symbols { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IndustryGroup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.GroupName).IsUnique();
        });

        modelBuilder.Entity<IndustrySubGroup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SubGroupName).IsUnique();

            entity.HasOne(e => e.Group)
                  .WithMany(g => g.SubGroups)
                  .HasForeignKey(e => e.GroupId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Symbol>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.InstrumentCode).IsUnique();
            entity.HasIndex(e => e.SymbolName).IsUnique();
            entity.HasIndex(e => e.Isin).IsUnique();

            entity.HasOne(e => e.Group)
                  .WithMany(g => g.Symbols)
                  .HasForeignKey(e => e.GroupId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SubGroup)
                  .WithMany(sg => sg.Symbols)
                  .HasForeignKey(e => e.SubGroupId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}