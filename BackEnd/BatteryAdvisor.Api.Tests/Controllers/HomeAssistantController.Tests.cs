using BatteryAdvisor.Api.Controllers;
using BatteryAdvisor.HA.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BatteryAdvisor.Api.Tests.Controllers;

public class HomeAssistantControllerTests
{
    [Fact]
    public async Task TestConnection_ReturnsOk_WithServiceResult()
    {
        // Arrange
        var homeAssistantServiceMock = new Mock<IHomeAssistantRestService>();
        homeAssistantServiceMock
            .Setup(s => s.TestConnectionAsync("https://ha.local", "abc123"))
            .ReturnsAsync(true);

        var controller = new HomeAssistantController(homeAssistantServiceMock.Object);

        // Act
        var actionResult = await controller.TestConnection("https://ha.local", "abc123");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var response = Assert.IsType<bool>(okResult.Value);
        Assert.True(response);
    }

    [Fact]
    public async Task TestConnection_ReturnsBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var homeAssistantServiceMock = new Mock<IHomeAssistantRestService>();
        homeAssistantServiceMock
            .Setup(s => s.TestConnectionAsync("https://ha.local", "abc123"))
            .ThrowsAsync(new InvalidOperationException("Boom"));

        var controller = new HomeAssistantController(homeAssistantServiceMock.Object);

        // Act
        var actionResult = await controller.TestConnection("https://ha.local", "abc123");

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Boom", badRequestResult.Value);
    }
}
