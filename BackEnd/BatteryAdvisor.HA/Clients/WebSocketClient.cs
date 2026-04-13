using BatteryAdvisor.Core.ApplicationOptions;
using BatteryAdvisor.Core.Models.HomeAssistant;
using BatteryAdvisor.Core.Services;
using BatteryAdvisor.HA.Helpers;
using BatteryAdvisor.HA.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BatteryAdvisor.HA.Clients;

public class WebSocketClient : IWebSocketClient
{
    private readonly IWebSocketService _webSocketService;
    private readonly IHomeAssistantWebSocketResponseService _webSocketResponseService;
    private readonly IWebSocketAuthenticationService _webSocketAuthenticationService;

    private readonly ApplicationOptions _options;

    private readonly ILogger<WebSocketClient> _logger;

    private bool _isLoggedIn = false;
    private int _messageIdCounter = 1;

    public WebSocketClient(
        IWebSocketService webSocketService,
        IHomeAssistantWebSocketResponseService webSocketResponseService,
        IWebSocketAuthenticationService webSocketAuthenticationService,
        IOptions<ApplicationOptions> options,
        ILogger<WebSocketClient> logger)
    {
        _webSocketService = webSocketService;
        _webSocketResponseService = webSocketResponseService;
        _webSocketAuthenticationService = webSocketAuthenticationService;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<StaticIdModel[]> GetStatisticIds()
    {
        _logger.LogDebug("Starting statistics ID retrieval from Home Assistant.");

        // Step 1: Ensure we have a connected WebSocket
        await EnsureConnectedAsync(CancellationToken.None);

        // Step 2: Authenticate if not already authenticated
        if (!_isLoggedIn)
        {
            _logger.LogInformation("Authenticating WebSocket session with Home Assistant.");
            await _webSocketAuthenticationService.AuthenticateAsync(
                _options.HomeAssistant.Token,
                CancellationToken.None);

            _isLoggedIn = true;
        }

        // Step 3: Send the list statistic IDs request
        var messageId = _messageIdCounter++;
        _logger.LogDebug("Sending list statistic IDs request with message id {MessageId}.", messageId);
        var listStatisticIdsMessage = WebSocketMessageHelper.BuildListStatisticIdsMessage(messageId);
        await SendMessageAsync(listStatisticIdsMessage, CancellationToken.None);

        // Step 4 & 5: Wait for the response with the matching message ID and parse the result
        var models = await _webSocketResponseService.ReceiveForMessageIdAsync<StaticIdModel[]>(
            messageId,
            CancellationToken.None);

        _logger.LogInformation("Received {Count} statistic IDs from Home Assistant.", models.Length);

        return models;
    }

    public async Task<StatisticsDuringPeriodModel[]> GetStatisticsDuringPeriod(
        string statisticId,
        string startTime,
        string endTime
    )
    {
        _logger.LogDebug("Starting statistics retrieval for statistic ID {StatisticId} from {StartTime} to {EndTime}.", statisticId, startTime, endTime);

        // Step 1: Ensure we have a connected WebSocket
        await EnsureConnectedAsync(CancellationToken.None);

        // Step 2: Authenticate if not already authenticated
        if (!_isLoggedIn)
        {
            _logger.LogInformation("Authenticating WebSocket session with Home Assistant.");
            await _webSocketAuthenticationService.AuthenticateAsync(
                _options.HomeAssistant.Token,
                CancellationToken.None);

            _isLoggedIn = true;
        }

        // Step 3: Send the list statistic IDs request
        var messageId = _messageIdCounter++;
        _logger.LogDebug("Sending list statistic IDs request for statistic ID {StatisticId} with message id {MessageId}.", statisticId, messageId);


        var listStatisticIdsMessage = WebSocketMessageHelper.BuildGetStatisticDuringPeriodMessage(messageId, statisticId, startTime, endTime);
        await SendMessageAsync(listStatisticIdsMessage, CancellationToken.None);

        // Step 4 & 5: Wait for the response with the matching message ID and parse the result
        var models = await _webSocketResponseService.ReceiveForMessageIdAsync<StatisticsDuringPeriodModel[]>(
            messageId,
            CancellationToken.None,
            statisticId);

        _logger.LogInformation("Received {Count} statistics for statistic ID {StatisticId} from Home Assistant.", models.Length, statisticId);

        return models;
    }




    /// <summary>
    /// Sends a message through the WebSocket connection. 
    /// </summary>
    /// <param name="message">The message to send through the WebSocket.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    private async Task SendMessageAsync(string message, CancellationToken cancellationToken)
    {
        await _webSocketService.SendAsync(message, cancellationToken);
    }

    /// <summary>
    /// Ensures that there is an active WebSocket connection to Home Assistant. 
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
    {
        var websocketUrl = BuildHomeAssistantWebSocketUrl(_options.HomeAssistant.Url);

        _logger.LogDebug("Ensuring WebSocket connection to Home Assistant at {WebSocketUrl}.", websocketUrl);

        await _webSocketService.GetOrConnectAsync(websocketUrl, cancellationToken);
    }

    /// <summary>
    /// Builds the WebSocket URL for connecting to Home Assistant based on the provided base URL.   
    /// </summary>
    /// <param name="homeAssistantUrl">The base URL of the Home Assistant instance.</param>
    /// <returns>The WebSocket URL for connecting to Home Assistant.</returns>
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