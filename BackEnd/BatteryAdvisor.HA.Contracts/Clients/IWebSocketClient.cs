using BatteryAdvisor.Core.Models.HomeAssistant;

namespace BatteryAdvisor.HA.Contracts.Clients;

public interface IWebSocketClient
{
   /// <summary>
   /// Get the list of statistic IDs from Home Assistant via the WebSocket API.
   /// </summary>
   /// <returns>An array of <see cref="StaticIdModel"/> representing the statistic IDs.</returns>
   Task<StaticIdModel[]> GetStatisticIds();

   /// <summary>
   /// Get the list of entities from Home Assistant via the WebSocket API.
   /// </summary>
   Task<EntityModel[]> GetEntities();

   /// <summary>
   /// Get statistics for a specific statistic ID during a specified period from Home Assistant via the WebSocket API.
   /// </summary>
   /// <param name="statisticId">The statistic ID for which to retrieve statistics.</param>
   /// <param name="startTime">The start time of the period for which to retrieve statistics</param>
   /// <param name="endTime">The end time of the period for which to retrieve statistics</param>
   /// <returns>An array of <see cref="StatisticsDuringPeriodModel"/> representing the retrieved statistics.</returns>
   Task<StatisticsDuringPeriodModel[]> GetStatisticsDuringPeriod(string statisticId, string startTime, string endTime);
}
