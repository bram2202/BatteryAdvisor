using BatteryAdvisor.Core.Contracts.Services;
using BatteryAdvisor.HA.Contracts.Services;
using BatteryAdvisor.HA.Contracts.Helpers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BatteryAdvisor.Core.ApplicationOptions;
using BatteryAdvisor.Core.Models.Response;

namespace BatteryAdvisor.HA.Services;

public class WebSocketAuthenticationService : IWebSocketAuthenticationService
{
    private readonly IWebSocketService _webSocketService;
    private readonly ApplicationOptions _options;
    private readonly ILogger<WebSocketAuthenticationService> _logger;
    private readonly IWebSocketMessageHelper _messageHelper;

    public WebSocketAuthenticationService(
        IWebSocketService webSocketService,
        IOptions<ApplicationOptions> options,
        ILogger<WebSocketAuthenticationService> logger,
        IWebSocketMessageHelper messageHelper
    )
    {
        _webSocketService = webSocketService;
        _options = options.Value;
        _logger = logger;
        _messageHelper = messageHelper;
    }

    public async Task AuthenticateAsync(
        string accessToken,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Home Assistant websocket authentication.");
        var authMessage = _messageHelper.BuildAuthMessage(accessToken);
        await this._webSocketService.SendAsync(authMessage, cancellationToken);
        await WaitForSuccessfulAuthAsync(cancellationToken);
    }

    /// <summary>
    /// Waits for a successful authentication response from Home Assistant after sending the authentication message.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    private async Task WaitForSuccessfulAuthAsync(
        CancellationToken cancellationToken)
    {

        var timeout = TimeSpan.FromSeconds(_options.HomeAssistant.WebSocketResponseTimeoutSeconds);
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        while (true)
        {
            var response = await _webSocketService.ReceiveAsync(linkedCts.Token);

            var authenticationResponse = JsonSerializer.Deserialize<AuthenticationResponse>(response);

            if (string.IsNullOrWhiteSpace(authenticationResponse?.type))
            {
                continue;
            }

            var responseType = authenticationResponse.type;

            if (string.Equals(responseType, "auth_ok", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Home Assistant websocket authentication successful.");
                return;
            }

            if (string.Equals(responseType, "auth_invalid", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Home Assistant websocket authentication failed: {Message}", authenticationResponse.message ?? "auth_invalid");
                throw new InvalidOperationException(
                    $"Home Assistant websocket auth failed: {authenticationResponse.message ?? "auth_invalid"}");
            }
        }
    }
}