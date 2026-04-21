using BatteryAdvisor.Core.Contracts.Enums;
using BatteryAdvisor.Core.Contracts.Models;

namespace BatteryAdvisor.Core.Contracts.Services;

public interface IConfigurationService
{
    /// <summary>
    /// Adds a new configuration after applying configuration-specific business rules.
    /// </summary>
    /// <param name="configuration">Configuration payload to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(ConfigurationCreateModel configuration);

    /// <summary>
    /// Retrieves a configuration by its key.
    /// </summary>
    /// <param name="key">The key of the configuration to retrieve.</param>
    /// <returns>The configuration model if found; otherwise, null.</returns>
    Task<ConfigurationReadModel?> GetConfigurationAsync(ConfigurationKeys key);
}
