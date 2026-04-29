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

    /// <summary>
    /// Retrieves all configurations.
    /// </summary>
    /// <returns>A list of all configuration models.</returns>
    Task<IEnumerable<ConfigurationReadModel>> GetAllConfigurationsAsync();

    /// <summary>
    /// Updates an existing configuration after applying configuration-specific business rules.
    /// </summary>
    /// <param name="configuration">The configuration model to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateConfigurationAsync(ConfigurationCreateModel configuration);

    /// <summary>
    /// Deletes a configuration by its key.
    /// </summary>
    /// <param name="key">The key of the configuration to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteConfigurationAsync(ConfigurationKeys key);
}
