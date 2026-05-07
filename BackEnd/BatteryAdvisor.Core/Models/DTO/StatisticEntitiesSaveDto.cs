namespace BatteryAdvisor.Core.Models.DTO;

public class StatisticEntitiesSaveDto
{
    public required string[] PvEntities { get; set; }
    public required string[] PowerProductionEntities { get; set; }
    public required string[] PowerConsumptionEntities { get; set; }
}
