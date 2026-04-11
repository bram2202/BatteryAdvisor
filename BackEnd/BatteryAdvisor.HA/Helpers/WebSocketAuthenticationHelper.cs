using BatteryAdvisor.Core.Models.HomeAssistant;
using BatteryAdvisor.Core.Services;
using System.Text.Json;

namespace BatteryAdvisor.HA.Helpers;

public static class WebSocketAuthenticationHelper
{
    public static async Task AuthenticateAsync(
        IWebSocketService webSocketService,
        string accessToken,
        CancellationToken cancellationToken)
    {
        var authMessage = WebSocketMessageHelper.BuildAuthMessage(accessToken);
        await webSocketService.SendAsync(authMessage, cancellationToken);
        await WaitForSuccessfulAuthAsync(webSocketService, cancellationToken);
    }

    /// <summary>
    /// Waits for a successful authentication response from Home Assistant after sending the authentication message.
    /// </summary>
    /// <param name="webSocketService">The WebSocket service used to receive messages.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when authentication fails.</exception>
    private static async Task WaitForSuccessfulAuthAsync(
        IWebSocketService webSocketService,
        CancellationToken cancellationToken)
    {
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        while (true)
        {
            var response = await webSocketService.ReceiveAsync(linkedCts.Token);

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