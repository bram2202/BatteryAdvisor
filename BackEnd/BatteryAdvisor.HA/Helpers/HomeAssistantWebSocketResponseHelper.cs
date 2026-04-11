using BatteryAdvisor.Core.Models.HomeAssistant;
using BatteryAdvisor.Core.Services;
using System.Text.Json;

namespace BatteryAdvisor.HA.Helpers;

public static class HomeAssistantWebSocketResponseHelper
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Listens for WebSocket messages from Home Assistant and returns the result of the message with the specified message ID, deserialized to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to which the "result" property of the WebSocket response should be deserialized.</typeparam>
    /// <param name="webSocketService">The WebSocket service used to receive messages.</param>
    /// <param name="messageId">The message ID to match in the WebSocket responses.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The deserialized result of the WebSocket response with the matching message ID.</returns>
    public static async Task<T> ReceiveForMessageIdAsync<T>(
        IWebSocketService webSocketService,
        int messageId,
        CancellationToken cancellationToken)
    {
        while (true)
        {
            var message = await webSocketService.ReceiveAsync(cancellationToken);

            using var document = JsonDocument.Parse(message);

            if (!document.RootElement.TryGetProperty("id", out var idElement))
            {
                continue;
            }

            if (idElement.ValueKind != JsonValueKind.Number)
            {
                continue;
            }

            if (idElement.TryGetInt32(out var responseMessageId) && responseMessageId == messageId)
            {
                try
                {
                    var parsedMessage = JsonSerializer.Deserialize<WebSocketResponseModel>(
                        message,
                        SerializerOptions);

                    if (parsedMessage is null)
                    {
                        throw new InvalidOperationException(
                            "Failed to deserialize websocket message to WebSocketResponseModel.");
                    }

                    if (parsedMessage.Result.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
                    {
                        throw new InvalidOperationException(
                            $"Websocket message result was empty for {typeof(T).Name}.");
                    }

                    var result = parsedMessage.Result.Deserialize<T>(SerializerOptions);

                    if (result is null)
                    {
                        throw new InvalidOperationException(
                            $"Failed to deserialize websocket message result to {typeof(T).Name}.");
                    }

                    return result;
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to deserialize websocket message result to {typeof(T).Name}.",
                        ex);
                }
            }
        }
    }
}