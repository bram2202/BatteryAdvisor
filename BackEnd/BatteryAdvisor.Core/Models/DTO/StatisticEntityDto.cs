namespace BatteryAdvisor.Core.Models.DTO;

public class StatisticEntityDto
{
    public required string EntityId { get; set; }
    public required string FriendlyName { get; set; }
    public required string UnitOfMeasurement { get; set; }
    public required string DisplayUnitOfMeasurement { get; set; }
    public required string State { get; set; }
    public required string Icon { get; set; }
}