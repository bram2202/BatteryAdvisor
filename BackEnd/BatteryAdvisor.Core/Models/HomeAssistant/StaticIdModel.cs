namespace BatteryAdvisor.Core.Models.HomeAssistant;

public class StaticIdModel
{
    public string? statistic_id { get; set; }
    public string? display_unit_of_measurement { get; set; }
    public bool? has_mean { get; set; }
    public int? mean_type { get; set; }
    public bool? has_sum { get; set; }
    public string? name { get; set; }
    public string? source { get; set; }
    public string? statistics_unit_of_measurement { get; set; }
    public string? unit_class { get; set; }
}

