using BatteryAdvisor.Core.Models.HomeAssistant;
using BatteryAdvisor.Core.Contracts.Services;
using BatteryAdvisor.HA.Contracts.Services;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BatteryAdvisor.HA.Services;

public class HomeAssistantWebSocketResponseService : IHomeAssistantWebSocketResponseService
{
    private readonly IWebSocketService _webSocketService;
    private readonly ILogger<HomeAssistantWebSocketResponseService> _logger;

    public HomeAssistantWebSocketResponseService(
        IWebSocketService webSocketService,
        ILogger<HomeAssistantWebSocketResponseService> logger
    )
    {
        _webSocketService = webSocketService;
        _logger = logger;
    }

    public async Task<T> ReceiveForMessageIdAsync<T>(
        int messageId,
        CancellationToken cancellationToken,
        string? resultPropertyName = null)
    {
        while (true)
        {
            var message = await _webSocketService.ReceiveAsync(cancellationToken);

            if (!IsMatchingResponseMessage(message, messageId))
            {
                continue;
            }

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
                    // _logger.LogError(
                    //     "Websocket message result was empty for message ID {MessageId}: {Message}",
                    //     messageId,
                    //     message);
                    throw new InvalidOperationException(
                        $"Websocket message result was empty for {typeof(T).Name}, message ID {messageId}, message: {message}");
                }

                if (TryCreateEmptyArrayResult<T>(parsedMessage.Result, resultPropertyName, out var emptyResult))
                {
                    _logger.LogInformation(
                        "Received empty nested result for message ID {MessageId} and property {ResultPropertyName}; returning empty array.",
                        messageId,
                        resultPropertyName);

                    return emptyResult;
                }

                var resultElement = ResolveResultElement(parsedMessage.Result, resultPropertyName);
                var result = DeserializeResult<T>(resultElement);

                _logger.LogInformation("Received response for message ID {MessageId}: {Result}", messageId, result);

                return result;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization error for message ID {MessageId}: {Message}", messageId, message);
                throw new InvalidOperationException(
                    $"Failed to deserialize websocket message result to {typeof(T).Name}.",
                    ex);
            }
        }
    }

    private static bool IsMatchingResponseMessage(string message, int messageId)
    {
        using var document = JsonDocument.Parse(message);

        if (!document.RootElement.TryGetProperty("id", out var idElement))
        {
            return false;
        }

        return idElement.ValueKind == JsonValueKind.Number
            && idElement.TryGetInt32(out var responseMessageId)
            && responseMessageId == messageId;
    }

    private static JsonElement ResolveResultElement(JsonElement resultElement, string? resultPropertyName)
    {
        if (string.IsNullOrWhiteSpace(resultPropertyName))
        {
            return resultElement;
        }

        if (resultElement.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidOperationException(
                $"Websocket message result was not an object for nested property '{resultPropertyName}'.");
        }

        if (!resultElement.TryGetProperty(resultPropertyName, out var nestedResultElement))
        {
            throw new InvalidOperationException(
                $"Websocket message result did not contain property '{resultPropertyName}'.");
        }

        return nestedResultElement;
    }

    private static T DeserializeResult<T>(JsonElement resultElement)
    {
        var result = resultElement.Deserialize<T>(SerializerOptions);

        if (result is null)
        {
            throw new InvalidOperationException(
                $"Failed to deserialize websocket message result to {typeof(T).Name}.");
        }

        return result;
    }

    private static bool TryCreateEmptyArrayResult<T>(
        JsonElement resultElement,
        string? resultPropertyName,
        out T emptyResult)
    {
        emptyResult = default!;

        if (string.IsNullOrWhiteSpace(resultPropertyName))
        {
            return false;
        }

        if (resultElement.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        if (resultElement.TryGetProperty(resultPropertyName, out _))
        {
            return false;
        }

        if (resultElement.EnumerateObject().Any())
        {
            return false;
        }

        var type = typeof(T);
        if (!type.IsArray)
        {
            return false;
        }

        var elementType = type.GetElementType();
        if (elementType is null)
        {
            return false;
        }

        emptyResult = (T)(object)Array.CreateInstance(elementType, 0);
        return true;
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

}