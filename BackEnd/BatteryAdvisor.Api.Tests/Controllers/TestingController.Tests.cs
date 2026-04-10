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
        var mockApiClient = new Mock<IApiClient>();
        mockApiClient.Setup(c => c.GetData()).Returns(Task.CompletedTask);
        
        var controller = new TestingController(mockApiClient.Object);

        // Act
        var result = await controller.Run();

        // Assert
        mockApiClient.Verify(c => c.GetData(), Times.Once);
        Assert.NotNull(result);
    }
}
