using BatteryAdvisor.Core.Models.HomeAssistant;
using BatteryAdvisor.Core.Services;
using BatteryAdvisor.HA.Services;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BatteryAdvisor.Core.ApplicationOptions;

namespace BatteryAdvisor.HA.Helpers;

public class WebSocketAuthenticationService : IWebSocketAuthenticationService
{
    private readonly IWebSocketService _webSocketService;
    private readonly ApplicationOptions _options;

    private readonly ILogger<WebSocketAuthenticationService> _logger;

    public WebSocketAuthenticationService(
        IWebSocketService webSocketService,
        IOptions<ApplicationOptions> options,
        ILogger<WebSocketAuthenticationService> logger
    )
    {
        _webSocketService = webSocketService;
        _options = options.Value;
        _logger = logger;

    }

    public async Task AuthenticateAsync(
        string accessToken,
        CancellationToken cancellationToken)
    {
        var authMessage = WebSocketMessageHelper.BuildAuthMessage(accessToken);
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
                return;
            }

            if (string.Equals(responseType, "auth_invalid", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Home Assistant websocket auth failed: {authenticationResponse.message ?? "auth_invalid"}");
            }
        }
    }
}