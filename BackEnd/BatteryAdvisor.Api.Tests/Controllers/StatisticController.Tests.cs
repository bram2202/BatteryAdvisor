using BatteryAdvisor.Api.Controllers;
using BatteryAdvisor.Core.Models.DTO;
using BatteryAdvisor.Core.Models.HomeAssistant;
using BatteryAdvisor.HA.Clients;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BatteryAdvisor.Api.Tests.Controllers;

public class StatisticControllerTests
{
    [Fact]
    public async Task GetStatisticIds_ReturnsOkWithStatisticIds_WhenWebSocketClientReturnsStatisticIds()
    {
        // Arrange
        var statisticIds = new[]
        {
            new StaticIdModel
            {
                statistic_id = "sensor.dsmr_energy_consumed_tariff_1",
                display_unit_of_measurement = "kWh"
            },
            new StaticIdModel
            {
                statistic_id = "sensor.dsmr_energy_consumed_tariff_2",
                display_unit_of_measurement = "kWh"
            }
        };

        var webSocketClientMock = new Mock<IWebSocketClient>();
        webSocketClientMock
            .Setup(c => c.GetStatisticIds())
            .ReturnsAsync(statisticIds);

        var controller = new StatisticController(webSocketClientMock.Object);

        // Act
        var actionResult = await controller.GetStatisticIds();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var response = Assert.IsAssignableFrom<StaticIdModel[]>(okResult.Value);

        Assert.Equal(2, response.Length);
        Assert.Equal("sensor.dsmr_energy_consumed_tariff_1", response[0].statistic_id);
        Assert.Equal("sensor.dsmr_energy_consumed_tariff_2", response[1].statistic_id);

        webSocketClientMock.Verify(c => c.GetStatisticIds(), Times.Once);
    }

    [Fact]
    public async Task GetStatisticsDuringPeriod_ReturnsOkWithMappedDtos_WhenWebSocketClientReturnsStatistics()
    {
        // Arrange
        var request = new StatisticsDuringPeriodRequestDTO
        {
            Id = "sensor.p1_meter_energie_export",
            Start = "2026-01-01T00:00:00Z",
            End = "2026-01-02T00:00:00Z"
        };

        var models = new[]
        {
            new StatisticsDuringPeriodModel
            {
                Start = 1767225600000,
                End = 1767232800000,
                Sum = 10.5,
                Change = 1.25
            },
            new StatisticsDuringPeriodModel
            {
                Start = 1767232800000,
                End = 1767240000000,
                Sum = 12.0,
                Change = 1.5
            }
        };

        var webSocketClientMock = new Mock<IWebSocketClient>();
        webSocketClientMock
            .Setup(c => c.GetStatisticsDuringPeriod(request.Id, request.Start, request.End))
            .ReturnsAsync(models);

        var controller = new StatisticController(webSocketClientMock.Object);

        // Act
        var actionResult = await controller.GetStatisticsDuringPeriod(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var response = Assert.IsAssignableFrom<List<StatisticsDuringPeriodResponseDTO>>(okResult.Value);

        Assert.Equal(2, response.Count);
        Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(models[0].Start).UtcDateTime, response[0].StartDateTimeUtc);
        Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(models[0].End).UtcDateTime, response[0].EndDateTimeUtc);
        Assert.Equal(models[0].Sum, response[0].Sum);
        Assert.Equal(models[0].Change, response[0].Change);

        Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(models[1].Start).UtcDateTime, response[1].StartDateTimeUtc);
        Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(models[1].End).UtcDateTime, response[1].EndDateTimeUtc);
        Assert.Equal(models[1].Sum, response[1].Sum);
        Assert.Equal(models[1].Change, response[1].Change);

        webSocketClientMock.Verify(
            c => c.GetStatisticsDuringPeriod(request.Id, request.Start, request.End),
            Times.Once);
    }

    [Fact]
    public async Task Get_ReturnsOkWithEmptyList_WhenWebSocketClientReturnsNoStatistics()
    {
        // Arrange
        var request = new StatisticsDuringPeriodRequestDTO
        {
            Id = "sensor.p1_meter_energie_export",
            Start = "2026-01-01T00:00:00Z",
            End = "2026-01-02T00:00:00Z"
        };

        var webSocketClientMock = new Mock<IWebSocketClient>();
        webSocketClientMock
            .Setup(c => c.GetStatisticsDuringPeriod(request.Id, request.Start, request.End))
            .ReturnsAsync(Array.Empty<StatisticsDuringPeriodModel>());

        var controller = new StatisticController(webSocketClientMock.Object);

        // Act
        var actionResult = await controller.GetStatisticsDuringPeriod(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var response = Assert.IsAssignableFrom<List<StatisticsDuringPeriodResponseDTO>>(okResult.Value);

        Assert.Empty(response);
        webSocketClientMock.Verify(
            c => c.GetStatisticsDuringPeriod(request.Id, request.Start, request.End),
            Times.Once);
    }
}
