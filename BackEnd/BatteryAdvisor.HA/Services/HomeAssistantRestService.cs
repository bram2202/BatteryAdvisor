using BatteryAdvisor.Core.Contracts.Enums;
using BatteryAdvisor.Core.Contracts.Models;
using BatteryAdvisor.Core.Contracts.Services;
using BatteryAdvisor.HA.Contracts.Services;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BatteryAdvisor.HA.Services;

public class HomeAssistantRestService : IHomeAssistantRestService
{
    private readonly IHttpClientService _httpClientService;
    private readonly IConfigurationService _configurationService;
    private readonly ILogger<HomeAssistantRestService> _logger;

    public HomeAssistantRestService(IHttpClientService httpClientService, IConfigurationService configurationService, ILogger<HomeAssistantRestService> logger)
    {
        _httpClientService = httpClientService;
        _configurationService = configurationService;
        _logger = logger;
    }

    public async Task<bool> TestConnectionAsync(string url, string token)
    {

        var path = $"{url}/api/";

        // Set the authorization header with the provided token
        var headers = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {token}" }
        };

        try
        {
            _ = _httpClientService.GetAsync<object>(path, headers).GetAwaiter().GetResult();

            // If the request is successful, we can assume the connection details are valid. We return true to indicate a successful connection.
            await StoreConnectionDetailsAsync(url, token);

            return true;
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == HttpStatusCode.BadRequest || ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning(ex, "Failed to connect to Home Assistant API. Status code: {StatusCode}. Message: {Message}", ex.StatusCode, ex.Message);
            }

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while testing connection to Home Assistant API. Message: {Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Stores the connection details in the database for future use. 
    /// </summary>
    /// <param name="url">The URL of the Home Assistant instance.</param>
    /// <param name="token">The access token for the Home Assistant instance.</param>
    /// <returns></returns>
    private async Task StoreConnectionDetailsAsync(string url, string token)
    {
        // Store the connection details in the database for future use
        await _configurationService.AddOrUpdateAsync(new ConfigurationCreateModel
        {
            Name = ConfigurationKeys.HomeAssistantUrl,
            Value = url
        });

        await _configurationService.AddOrUpdateAsync(new ConfigurationCreateModel
        {
            Name = ConfigurationKeys.HomeAssistantToken,
            Value = token
        });
    }


}