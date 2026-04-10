using BatteryAdvisor.Core.Services;
using BatteryAdvisor.Core.ApplicationOptions;
using Microsoft.Extensions.Options;

namespace BatteryAdvisor.HA.Clients;

public class RestClient : IRestClient
{
    private readonly IHttpClientService _httpClientService;
    private readonly ApplicationOptions _options;

    public RestClient(IHttpClientService httpClientService,
        IOptions<ApplicationOptions> options)
    {
        _httpClientService = httpClientService;
        _options = options.Value;
    }
    
    public async Task GetData()
    {
        // !! For testing purposes only !!
        var headers = new Dictionary<string, string>
        {
            ["Authorization"] = $"Bearer {_options.HomeAssistant.Token}"
        };

        var url = $"{_options.HomeAssistant.Url}/api/states/sensor.battery_level";

        var result = await _httpClientService.GetAsync<string>(url, headers);
        Console.WriteLine(result);
    }
}