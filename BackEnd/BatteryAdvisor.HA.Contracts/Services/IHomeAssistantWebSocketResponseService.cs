namespace BatteryAdvisor.HA.Contracts.Services;

public interface IHomeAssistantWebSocketResponseService
{
    /// <summary>
    /// Listens for WebSocket messages from Home Assistant and returns the result of the message with the specified message ID, deserialized to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to which the "result" property of the WebSocket response should be deserialized.</typeparam>
    /// <param name="messageId">The message ID to match in the WebSocket responses.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <param name="resultPropertyName">
    /// Optional property name inside the "result" object to deserialize instead of deserializing the full "result" object.
    /// </param>
    /// <returns>The deserialized result of the WebSocket response with the matching message ID.</returns>
    Task<T> ReceiveForMessageIdAsync<T>(
        int messageId,
        CancellationToken cancellationToken,
        string? resultPropertyName = null);
}
