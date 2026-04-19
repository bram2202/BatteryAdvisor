using BatteryAdvisor.Core.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace BatteryAdvisor.Core.Database;

public class BatteryAdvisorContext : DbContext
{
    public DbSet<ConfigurationModel> Configurations { get; set; }

    public BatteryAdvisorContext(DbContextOptions<BatteryAdvisorContext> options) : base(options)
    {
    }
}