using System.Text.Json;
using BatteryAdvisor.Core.Models.HomeAssistant;

namespace BatteryAdvisor.HA.Helpers;

public static class WebSocketMessageHelper
{

    /// <summary>
    /// Builds the authentication message to be sent to Home Assistant for WebSocket authentication using the provided access token.
    /// </summary>
    /// <param name="accessToken"></param>
    /// <returns></returns>
    public static string BuildAuthMessage(string accessToken)
    {
        var authMsg = new AuthenticationRequest
        {
            type = "auth",
            access_token = accessToken
        };

        return JsonSerializer.Serialize(authMsg);
    }

    /// <summary>    
    /// Builds the message to request the list of statistic IDs from Home Assistant via the WebSocket API, using the specified message ID.
    /// </summary>   
    /// <param name="id">The message ID to include in the request message.</param>
    public static string BuildListStatisticIdsMessage(int id)
    {
        var message = JsonSerializer.Serialize(new
        {
            id = id,
            type = "recorder/list_statistic_ids",
            statistic_type = "sum"
        });
        return message;
    }
}