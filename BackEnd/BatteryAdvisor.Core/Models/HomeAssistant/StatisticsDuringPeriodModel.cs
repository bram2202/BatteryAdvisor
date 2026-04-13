using System.Text.Json.Serialization;

namespace BatteryAdvisor.Core.Models.HomeAssistant;

public class StatisticsDuringPeriodModel
{
    [JsonPropertyName("start")]
    public long Start { get; set; }

    [JsonPropertyName("end")]
    public long End { get; set; }

    [JsonPropertyName("sum")]
    public double Sum { get; set; }

    [JsonPropertyName("change")]
    public double Change { get; set; }

}

