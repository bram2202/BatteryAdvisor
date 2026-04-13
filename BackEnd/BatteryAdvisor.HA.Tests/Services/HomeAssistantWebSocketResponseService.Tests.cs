using BatteryAdvisor.Core.Models.HomeAssistant;
using BatteryAdvisor.Core.Services;
using BatteryAdvisor.HA.Services;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net.WebSockets;

namespace BatteryAdvisor.HA.Tests;

public class HomeAssistantWebSocketResponseServiceTests
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
        var helper = new HomeAssistantWebSocketResponseService(
            webSocketService,
            NullLogger<HomeAssistantWebSocketResponseService>.Instance);

        var result = await helper
            .ReceiveForMessageIdAsync<StaticIdModel[]>(1, CancellationToken.None);

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

    [Fact]
    public async Task ReceiveForMessageIdAsync_ParsesNestedResultPropertyToStatisticsDuringPeriodArray()
    {
        var response = """
        {
            "id": 11,
            "type": "result",
            "success": true,
            "result": {
                "sensor.p1_meter_energie_export": [
                    {
                        "start": 1769900400000,
                        "end": 1772319600000,
                        "sum": 19.057999999999993,
                        "change": 19.057999999999993
                    },
                    {
                        "start": 1772319600000,
                        "end": 1774994400000,
                        "sum": 177.59000000000015,
                        "change": 158.53200000000015
                    }
                ]
            }
        }
        """;

        var webSocketService = new FakeWebSocketService(response);
        var helper = new HomeAssistantWebSocketResponseService(
            webSocketService,
            NullLogger<HomeAssistantWebSocketResponseService>.Instance);

        var result = await helper.ReceiveForMessageIdAsync<StatisticsDuringPeriodModel[]>(
            11,
            CancellationToken.None,
            "sensor.p1_meter_energie_export");

        Assert.NotNull(result);
        Assert.Equal(2, result.Length);
        Assert.Equal(1769900400000, result[0].Start);
        Assert.Equal(1772319600000, result[0].End);
        Assert.Equal(19.057999999999993, result[0].Sum);
        Assert.Equal(19.057999999999993, result[0].Change);
    }

    [Fact]
    public async Task ReceiveForMessageIdAsync_ReturnsEmptyArray_WhenNestedResultPropertyMissingAndResultObjectIsEmpty()
    {
        var response = """
        {
            "id": 4,
            "type": "result",
            "success": true,
            "result": {}
        }
        """;

        var webSocketService = new FakeWebSocketService(response);
        var helper = new HomeAssistantWebSocketResponseService(
            webSocketService,
            NullLogger<HomeAssistantWebSocketResponseService>.Instance);

        var result = await helper.ReceiveForMessageIdAsync<StatisticsDuringPeriodModel[]>(
            4,
            CancellationToken.None,
            "sensor.p1_meter_energie_export");

        Assert.NotNull(result);
        Assert.Empty(result);
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
