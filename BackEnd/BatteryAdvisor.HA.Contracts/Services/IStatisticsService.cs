using BatteryAdvisor.Core.Models.DTO;

namespace BatteryAdvisor.HA.Contracts.Services;

public interface IStatisticsService
{
    /// <summary>
    /// Get a list of all statistic entities in Home Assistant, 
    /// along with their friendly name, unit of measurement, display unit of measurement, state and icon.
    /// </summary>
    /// <returns></returns>
    Task<StatisticEntityDto[]> GetStatisticEntities();

}