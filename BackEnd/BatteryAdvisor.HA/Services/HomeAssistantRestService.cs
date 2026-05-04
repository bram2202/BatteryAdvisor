using BatteryAdvisor.Core.Contracts.Services;
using BatteryAdvisor.HA.Contracts.Services;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BatteryAdvisor.HA.Services;

public class HomeAssistantRestService : IHomeAssistantRestService
{
    private readonly IHttpClientService _httpClientService;
    private readonly ILogger<HomeAssistantRestService> _logger;

    public HomeAssistantRestService(IHttpClientService httpClientService, ILogger<HomeAssistantRestService> logger)
    {
        _httpClientService = httpClientService;
        _logger = logger;
    }

    public Task<bool> TestConnectionAsync(string url, string token)
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
            return Task.FromResult(true);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == HttpStatusCode.BadRequest || ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning(ex, "Failed to connect to Home Assistant API. Status code: {StatusCode}. Message: {Message}", ex.StatusCode, ex.Message);
            }

            throw;
        }
    }


}