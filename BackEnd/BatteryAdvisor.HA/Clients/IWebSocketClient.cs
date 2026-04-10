using BatteryAdvisor.Core.Models.HomeAssistant;

namespace BatteryAdvisor.HA.Clients;

public interface IWebSocketClient
{
   Task<StaticIdModel[]> GetStatisticIds();
}