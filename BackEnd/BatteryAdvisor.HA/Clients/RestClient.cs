using BatteryAdvisor.Core.Contracts.Enums;
using BatteryAdvisor.Core.Contracts.Services;
using BatteryAdvisor.HA.Contracts.Clients;
using Microsoft.Extensions.Options;

namespace BatteryAdvisor.HA.Clients;

public class RestClient : IRestClient
{
    private readonly IHttpClientService _httpClientService;
    private readonly IConfigurationService _configurationService;


    public RestClient(IHttpClientService httpClientService, IConfigurationService configurationService)
    {
        _httpClientService = httpClientService;
        _configurationService = configurationService;

    }

    public async Task GetData()
    {

        // Get token from configuration
        var tokenConfiguration = await _configurationService.GetConfigurationAsync(ConfigurationKeys.HomeAssistantToken);
        if (tokenConfiguration is null || string.IsNullOrWhiteSpace(tokenConfiguration.Value))
        {
            throw new InvalidOperationException("Home Assistant token is not configured. Please configure the token before attempting to connect.");
        }

        var urlConfiguration = await _configurationService.GetConfigurationAsync(ConfigurationKeys.HomeAssistantUrl);
        if (urlConfiguration is null || string.IsNullOrWhiteSpace(urlConfiguration.Value))
        {
            throw new InvalidOperationException("Home Assistant URL is not configured. Please configure the URL before attempting to connect.");
        }

        // !! For testing purposes only !!
        var headers = new Dictionary<string, string>
        {
            ["Authorization"] = $"Bearer {tokenConfiguration.Value}"
        };

        var url = $"{urlConfiguration.Value}/api/states/sensor.battery_level";

        var result = await _httpClientService.GetAsync<string>(url, headers);
        Console.WriteLine(result);
    }
}