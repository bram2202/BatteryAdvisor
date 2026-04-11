using BatteryAdvisor.Core.Models.HomeAssistant;
using BatteryAdvisor.Core.Services;
using BatteryAdvisor.HA.Helpers;
using System.Net.WebSockets;

namespace BatteryAdvisor.HA.Tests;

public class HomeAssistantWebSocketResponseHelperTests
{
    [Fact]
    public async Task ReceiveForMessageIdAsync_ParsesResultArrayToStaticIdModelArray()
    {
        var response = """
        {
            "id": 1,
            "type": "result",
            "success": true,
            "result": [
                {
                    "statistic_id": "sensor.dsmr_energy_consumed_tariff_1",
                    "display_unit_of_measurement": "kWh",
                    "has_mean": false,
                    "mean_type": 0,
                    "has_sum": true,
                    "name": null,
                    "source": "recorder",
                    "statistics_unit_of_measurement": "kWh",
                    "unit_class": "energy"
                },
                {
                    "statistic_id": "sensor.dsmr_energy_consumed_tariff_2",
                    "display_unit_of_measurement": "kWh",
                    "has_mean": false,
                    "mean_type": 0,
                    "has_sum": true,
                    "name": null,
                    "source": "recorder",
                    "statistics_unit_of_measurement": "kWh",
                    "unit_class": "energy"
                }
            ]
        }
        """;

        var webSocketService = new FakeWebSocketService(response);

        var result = await HomeAssistantWebSocketResponseHelper
            .ReceiveForMessageIdAsync<StaticIdModel[]>(webSocketService, 1, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Length);

        Assert.Equal("sensor.dsmr_energy_consumed_tariff_1", result[0].statistic_id);
        Assert.Equal("kWh", result[0].display_unit_of_measurement);
        Assert.False(result[0].has_mean);
        Assert.Equal(0, result[0].mean_type);
        Assert.True(result[0].has_sum);
        Assert.Null(result[0].name);
        Assert.Equal("recorder", result[0].source);
        Assert.Equal("kWh", result[0].statistics_unit_of_measurement);
        Assert.Equal("energy", result[0].unit_class);

        Assert.Equal("sensor.dsmr_energy_consumed_tariff_2", result[1].statistic_id);
    }

    private sealed class FakeWebSocketService(string response) : IWebSocketService
    {
        public Task<ClientWebSocket> GetOrConnectAsync(string url, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task CloseAsync(CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task SendAsync(string message, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task<string> ReceiveAsync(CancellationToken cancellationToken)
            => Task.FromResult(response);
    }
}
