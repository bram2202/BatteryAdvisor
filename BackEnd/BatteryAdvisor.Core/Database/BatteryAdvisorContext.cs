using BatteryAdvisor.Core.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace BatteryAdvisor.Core.Database;

public class BatteryAdvisorContext : DbContext
{
    public DbSet<ConfigurationModel> Configurations { get; set; } = null!;

    public BatteryAdvisorContext(DbContextOptions<BatteryAdvisorContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConfigurationModel>()
            .HasIndex(x => x.Name)
            .IsUnique();
    }
}