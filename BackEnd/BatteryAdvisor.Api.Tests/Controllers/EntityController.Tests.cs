using BatteryAdvisor.Api.Controllers;
using BatteryAdvisor.Core.Models.DTO;
using BatteryAdvisor.HA.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BatteryAdvisor.Api.Tests.Controllers;

public class EntityControllerTests
{
    [Fact]
    public async Task GetStatisticEntities_ReturnsOkWithStatisticEntities_WhenServiceReturnsEntities()
    {
        // Arrange
        var statisticEntities = new[]
        {
            new StatisticEntityDto
            {
                EntityId = "sensor.energy_import",
                FriendlyName = "Energy Import",
                UnitOfMeasurement = "kWh",
                DisplayUnitOfMeasurement = "kWh",
                State = "123.45",
                Icon = "mdi:flash"
            },
            new StatisticEntityDto
            {
                EntityId = "sensor.energy_export",
                FriendlyName = "Energy Export",
                UnitOfMeasurement = "kWh",
                DisplayUnitOfMeasurement = "kWh",
                State = "67.89",
                Icon = "mdi:transmission-tower-export"
            }
        };

        var statisticsServiceMock = new Mock<IStatisticsService>();
        statisticsServiceMock
            .Setup(s => s.GetStatisticEntities())
            .ReturnsAsync(statisticEntities);

        var controller = new EntityController(statisticsServiceMock.Object);

        // Act
        var actionResult = await controller.GetStatisticEntities();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var response = Assert.IsAssignableFrom<StatisticEntityDto[]>(okResult.Value);

        Assert.Equal(2, response.Length);
        Assert.Equal("sensor.energy_import", response[0].EntityId);
        Assert.Equal("sensor.energy_export", response[1].EntityId);

        statisticsServiceMock.Verify(s => s.GetStatisticEntities(), Times.Once);
    }

    [Fact]
    public async Task GetStatisticEntities_ReturnsOkWithEmptyArray_WhenServiceReturnsNoEntities()
    {
        // Arrange
        var statisticsServiceMock = new Mock<IStatisticsService>();
        statisticsServiceMock
            .Setup(s => s.GetStatisticEntities())
            .ReturnsAsync(Array.Empty<StatisticEntityDto>());

        var controller = new EntityController(statisticsServiceMock.Object);

        // Act
        var actionResult = await controller.GetStatisticEntities();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var response = Assert.IsAssignableFrom<StatisticEntityDto[]>(okResult.Value);

        Assert.Empty(response);
        statisticsServiceMock.Verify(s => s.GetStatisticEntities(), Times.Once);
    }
}
