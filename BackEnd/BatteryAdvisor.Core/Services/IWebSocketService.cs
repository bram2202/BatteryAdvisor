using System.Net.WebSockets;

namespace BatteryAdvisor.Core.Services;

public interface IWebSocketService
{
    Task<ClientWebSocket> GetOrConnectAsync(string url, CancellationToken cancellationToken);

    Task CloseAsync(CancellationToken cancellationToken);

}