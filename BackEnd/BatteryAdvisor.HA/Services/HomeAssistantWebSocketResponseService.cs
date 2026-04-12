using BatteryAdvisor.Core.Models.HomeAssistant;
using BatteryAdvisor.Core.Services;
using System.Text.Json;

namespace BatteryAdvisor.HA.Services;

public class HomeAssistantWebSocketResponseService : IHomeAssistantWebSocketResponseService
{
    private readonly IWebSocketService _webSocketService;

    public HomeAssistantWebSocketResponseService(IWebSocketService webSocketService)
    {
        _webSocketService = webSocketService;
    }

    public async Task<T> ReceiveForMessageIdAsync<T>(
        int messageId,
        CancellationToken cancellationToken)
    {
        while (true)
        {
            var message = await _webSocketService.ReceiveAsync(cancellationToken);

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

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

}