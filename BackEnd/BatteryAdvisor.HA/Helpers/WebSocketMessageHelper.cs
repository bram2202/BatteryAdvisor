using System.Text.Json;
using BatteryAdvisor.Core.Models.HomeAssistant;

namespace BatteryAdvisor.HA.Helpers;

public class WebSocketMessageHelper : IWebSocketMessageHelper
{

    public string BuildAuthMessage(string accessToken)
    {
        var authMsg = new AuthenticationRequest
        {
            type = "auth",
            access_token = accessToken
        };

        return JsonSerializer.Serialize(authMsg);
    }

    public string BuildListStatisticIdsMessage(int id)
    {
        var message = JsonSerializer.Serialize(new
        {
            id = id,
            type = "recorder/list_statistic_ids",
            statistic_type = "sum"
        });
        return message;
    }

    public string BuildGetStatisticDuringPeriodMessage(int id, string statisticId, string startTime, string endTime)
    {
        var message = JsonSerializer.Serialize(new
        {
            id = id,
            type = "recorder/statistics_during_period",
            statistic_ids = new[] { statisticId },
            start_time = startTime,
            end_time = endTime,
            period = "month", // TODO: make this configurable
            types = new[] { "sum", "change" }
        });
        return message;
    }
}