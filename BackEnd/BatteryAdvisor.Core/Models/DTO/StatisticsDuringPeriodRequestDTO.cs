namespace BatteryAdvisor.Core.Models.DTO;

public class StatisticsDuringPeriodRequestDTO
{
    public required string Id { get; set; }
    public required string Start { get; set; }
    public required string End { get; set; }
}
