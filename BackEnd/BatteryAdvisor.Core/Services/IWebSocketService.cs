using System.Net.WebSockets;

namespace BatteryAdvisor.Core.Services;

public interface IWebSocketService
{
    /// <summary>
    /// Gets an open WebSocket connection to the specified URL, 
    /// or establishes a new one if necessary.
    /// </summary>
    /// <param name="url">The WebSocket URL to connect to.</param>
    /// <param name="cancellationToken">A token to cancel the connection attempt.</param>
    /// <returns>An open ClientWebSocket connected to the specified URL.</returns>
    Task<ClientWebSocket> GetOrConnectAsync(string url, CancellationToken cancellationToken);

    /// <summary>
    /// Closes the WebSocket connection if it is open.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the close operation.</param
    Task CloseAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Sends a message over the WebSocket connection.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="cancellationToken">A token to cancel the send operation.</param
    Task SendAsync(string message, CancellationToken cancellationToken);

    /// <summary> Receives a message from the WebSocket connection.
    /// </summary> <param name="cancellationToken">A token to cancel the receive operation.</param>
    /// <returns>A message received from the WebSocket connection.</returns>
    Task<string> ReceiveAsync(CancellationToken cancellationToken);

}