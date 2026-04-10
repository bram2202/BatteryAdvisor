using BatteryAdvisor.Core.ApplicationOptions;
using BatteryAdvisor.Core.Models.HomeAssistant;
using BatteryAdvisor.Core.Services;
using Microsoft.Extensions.Options;
using System.Net.WebSockets;

namespace BatteryAdvisor.HA.Clients;

public class WebSocketClient : IWebSocketClient
{
    private readonly IWebSocketService _webSocketService = new WebSocketService();
    private readonly ApplicationOptions _options;

    private ClientWebSocket? _socket;

    public WebSocketClient(IOptions<ApplicationOptions> options)
    {
        _options = options.Value;
    }

    public async Task<StaticIdModel[]> GetStatisticIds()
    {
        await EnsureConnectedAsync(CancellationToken.None);

        // TODO: Implement logic the send msg and wait for response
        // {
        //   "id": 3,
        //   "type": "recorder/list_statistic_ids",
        //   "statistic_type": "sum"
        // }
        // ID is incremental

        return [];
    }

    private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
    {
        var websocketUrl = BuildHomeAssistantWebSocketUrl(_options.HomeAssistant.Url);
        _socket = await _webSocketService.GetOrConnectAsync(websocketUrl, cancellationToken);
    }

    private static string BuildHomeAssistantWebSocketUrl(string homeAssistantUrl)
    {
        var baseUri = new Uri(homeAssistantUrl, UriKind.Absolute);
        var builder = new UriBuilder(baseUri)
        {
            Scheme = baseUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) ? "wss" : "ws",
            Port = baseUri.Port,
            Path = "api/websocket"
        };

        return builder.Uri.ToString();
    }
}