using System.Text.Json.Serialization;

namespace BatteryAdvisor.Core.Models.HomeAssistant;

public class StaticIdModel
{
    [JsonPropertyName("statistic_id")]
    public required string StatisticId { get; set; }

    [JsonPropertyName("display_unit_of_measurement")]
    public string? DisplayUnitOfMeasurement { get; set; }

    [JsonPropertyName("has_mean")]
    public bool? HasMean { get; set; }

    [JsonPropertyName("mean_type")]
    public int? MeanType { get; set; }

    [JsonPropertyName("has_sum")]
    public bool? HasSum { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("statistics_unit_of_measurement")]
    public string? StatisticsUnitOfMeasurement { get; set; }

    [JsonPropertyName("unit_class")]
    public string? UnitClass { get; set; }
}

