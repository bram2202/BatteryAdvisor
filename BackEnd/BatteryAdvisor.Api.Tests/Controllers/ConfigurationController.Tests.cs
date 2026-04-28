using BatteryAdvisor.Api.Controllers;
using BatteryAdvisor.Core.Contracts.Enums;
using BatteryAdvisor.Core.Contracts.Models;
using BatteryAdvisor.Core.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BatteryAdvisor.Api.Tests.Controllers;

public class ConfigurationControllerTests
{
    [Fact]
    public async Task GetConfigurationByKey_ReturnsBadRequest_WhenKeyIsUndefinedEnumValue()
    {
        // Arrange
        var configurationServiceMock = new Mock<IConfigurationService>();
        var controller = new ConfigurationController(configurationServiceMock.Object);

        // Act
        var actionResult = await controller.GetConfigurationByKey("2");

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Invalid configuration key: 2", badRequestResult.Value);
        configurationServiceMock.Verify(
            s => s.GetConfigurationAsync(It.IsAny<ConfigurationKeys>()),
            Times.Never);
    }

    [Fact]
    public async Task GetConfigurationByKey_ReturnsBadRequest_WhenKeyIsNotParsable()
    {
        // Arrange
        var configurationServiceMock = new Mock<IConfigurationService>();
        var controller = new ConfigurationController(configurationServiceMock.Object);

        // Act
        var actionResult = await controller.GetConfigurationByKey("not-a-key");

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Invalid configuration key: not-a-key", badRequestResult.Value);
        configurationServiceMock.Verify(
            s => s.GetConfigurationAsync(It.IsAny<ConfigurationKeys>()),
            Times.Never);
    }

    [Fact]
    public async Task GetConfigurationByKey_ReturnsNotFound_WhenKeyDoesNotExist()
    {
        // Arrange
        const string key = "HAUrl";

        var configurationServiceMock = new Mock<IConfigurationService>();
        configurationServiceMock
            .Setup(s => s.GetConfigurationAsync(ConfigurationKeys.HAUrl))
            .ReturnsAsync((ConfigurationReadModel?)null);

        var controller = new ConfigurationController(configurationServiceMock.Object);

        // Act
        var actionResult = await controller.GetConfigurationByKey(key);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        Assert.Equal("Configuration with key 'HAUrl' not found.", notFoundResult.Value);
    }

    [Fact]
    public async Task GetConfigurationByKey_ReturnsOk_WhenKeyExists()
    {
        // Arrange
        var model = new ConfigurationReadModel
        {
            Name = ConfigurationKeys.HAUrl,
            Value = "https://example.com"
        };

        var configurationServiceMock = new Mock<IConfigurationService>();
        configurationServiceMock
            .Setup(s => s.GetConfigurationAsync(ConfigurationKeys.HAUrl))
            .ReturnsAsync(model);

        var controller = new ConfigurationController(configurationServiceMock.Object);

        // Act
        var actionResult = await controller.GetConfigurationByKey("HAUrl");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var response = Assert.IsType<ConfigurationReadModel>(okResult.Value);

        Assert.Equal(ConfigurationKeys.HAUrl, response.Name);
        Assert.Equal("https://example.com", response.Value);
    }

    [Fact]
    public async Task DeleteConfiguration_ReturnsBadRequest_WhenKeyIsNotParsable()
    {
        // Arrange
        var configurationServiceMock = new Mock<IConfigurationService>();
        var controller = new ConfigurationController(configurationServiceMock.Object);

        // Act
        var actionResult = await controller.DeleteConfiguration("not-a-key");

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Invalid configuration key: not-a-key", badRequestResult.Value);
        configurationServiceMock.Verify(
            s => s.DeleteConfigurationAsync(It.IsAny<ConfigurationKeys>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteConfiguration_ReturnsNotFound_WhenConfigurationDoesNotExist()
    {
        // Arrange
        var configurationServiceMock = new Mock<IConfigurationService>();
        configurationServiceMock
            .Setup(s => s.DeleteConfigurationAsync(ConfigurationKeys.HAToken))
            .ThrowsAsync(new InvalidOperationException("Configuration with key 'HAToken' does not exist."));

        var controller = new ConfigurationController(configurationServiceMock.Object);

        // Act
        var actionResult = await controller.DeleteConfiguration("HAToken");

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        Assert.Equal("Configuration with key 'HAToken' does not exist.", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteConfiguration_ReturnsOk_WhenDeleteSucceeds()
    {
        // Arrange
        var configurationServiceMock = new Mock<IConfigurationService>();
        var controller = new ConfigurationController(configurationServiceMock.Object);

        // Act
        var actionResult = await controller.DeleteConfiguration("HAUrl");

        // Assert
        Assert.IsType<OkResult>(actionResult);
        configurationServiceMock.Verify(
            s => s.DeleteConfigurationAsync(ConfigurationKeys.HAUrl),
            Times.Once);
    }
}
