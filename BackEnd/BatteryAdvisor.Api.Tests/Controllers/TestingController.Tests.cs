using BatteryAdvisor.Api.Controllers;
using BatteryAdvisor.HA.Clients;
using Moq;

namespace BatteryAdvisor.Api.Tests;

public class TestingControllerTests
{
    [Fact]
    public async Task Run_CallsApiClientGetData_ReturnsOk()
    {
        // Arrange
        var mockRestClient = new Mock<IRestClient>();
        mockRestClient.Setup(c => c.GetData()).Returns(Task.CompletedTask);

        var mockWebSocketClient = new Mock<IWebSocketClient>();

        var controller = new TestingController(mockRestClient.Object, mockWebSocketClient.Object);

        // Act
        var result = await controller.Run();

        // Assert
        mockRestClient.Verify(c => c.GetData(), Times.Once);
        Assert.NotNull(result);
    }
}
