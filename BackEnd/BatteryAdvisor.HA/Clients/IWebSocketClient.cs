using BatteryAdvisor.Core.Models.HomeAssistant;

namespace BatteryAdvisor.HA.Clients;

public interface IWebSocketClient
{
   /// <summary>
   /// Get the list of statistic IDs from Home Assistant via the WebSocket API.
   /// </summary>
   /// <returns>An array of <see cref="StaticIdModel"/> representing the statistic IDs.</returns>
   Task<StaticIdModel[]> GetStatisticIds();
}