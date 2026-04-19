using BatteryAdvisor.Core.Contracts.Enums;
using BatteryAdvisor.Core.Contracts.Models;
using BatteryAdvisor.Core.Contracts.Services;
using BatteryAdvisor.Core.Database;
using BatteryAdvisor.Core.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace BatteryAdvisor.Core.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly IDatabaseService _databaseService;
    private readonly BatteryAdvisorContext _context;

    public ConfigurationService(IDatabaseService databaseService, BatteryAdvisorContext context)
    {
        _databaseService = databaseService;
        _context = context;
    }

    public async Task AddAsync(ConfigurationCreateModel configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.Value))
        {
            throw new ArgumentException("Configuration value cannot be empty.", nameof(configuration));
        }

        var alreadyExists = await _context.Configurations
            .AnyAsync(x => x.Name == configuration.Name);

        if (alreadyExists)
        {
            throw new InvalidOperationException($"Configuration with name '{configuration.Name}' already exists.");
        }

        var entity = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            Name = configuration.Name,
            Value = configuration.Value.Trim()
        };

        await _databaseService.AddAsync(entity);
    }

    public async Task<ConfigurationReadModel?> GetConfigurationAsync(ConfigurationKeys key)
    {
        var configuration = await _context.Configurations
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Name == key);

        if (configuration is null)
        {
            return null;
        }

        return new ConfigurationReadModel
        {
            Name = configuration.Name,
            Value = configuration.Value
        };
    }
}
