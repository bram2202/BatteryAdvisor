using BatteryAdvisor.Core.Models.DTO;
using BatteryAdvisor.HA.Contracts.Clients;
using BatteryAdvisor.HA.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace BatteryAdvisor.HA.Services;

public class StatisticsService : IStatisticsService
{

    private readonly IWebSocketClient _webSocketClient;
    private readonly ILogger<StatisticsService> _logger;
    public StatisticsService(IWebSocketClient webSocketClient, ILogger<StatisticsService> logger)
    {
        _webSocketClient = webSocketClient;
        _logger = logger;
    }

    public async Task<StatisticEntityDto[]> GetStatisticEntities()
    {
        // Get all the statistic IDs first
        var statisticIds = await _webSocketClient.GetStatisticIds();

        // Then get all the entities
        var entities = await _webSocketClient.GetEntities();

        if (statisticIds is null || entities is null)
        {
            throw new InvalidOperationException("Failed to retrieve statistic IDs or entities from Home Assistant.");
        }

        // filter out the entities that are not in the statistic IDs
        var statisticEntities = entities
            .Where(e => statisticIds.Any(s => s.StatisticId == e.EntityId))
            .Select(e =>
            {
                var matchingStatistic = statisticIds.First(s => s.StatisticId == e.EntityId);

                if (matchingStatistic is null)
                {
                    _logger.LogDebug("No matching statistic found for entity ID {EntityId}", e.EntityId);
                    return null;
                }

                return new StatisticEntityDto
                {
                    EntityId = e.EntityId,
                    FriendlyName = e.Attributes?.FriendlyName ?? "",
                    UnitOfMeasurement = e.Attributes?.UnitOfMeasurement ?? "",
                    DisplayUnitOfMeasurement = matchingStatistic?.DisplayUnitOfMeasurement ?? "",
                    State = e.State ?? "",
                    Icon = e.Attributes?.Icon ?? "",
                };
            })
            .OfType<StatisticEntityDto>()
            .ToArray();


        return statisticEntities;
    }
}