namespace BatteryAdvisor.HA.Services;

public interface IWebSocketAuthenticationService
{
    /// <summary>
    /// Sends the authentication message to Home Assistant over the WebSocket connection
    /// and waits for a successful authentication response. Throws an exception if authentication fails.
    /// </summary>
    /// <param name="accessToken">The access token used for authentication.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AuthenticateAsync(string accessToken, CancellationToken cancellationToken);
}