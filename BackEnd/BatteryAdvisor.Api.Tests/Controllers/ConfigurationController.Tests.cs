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
    public async Task GetConfiguration_ReturnsOk_WithServiceResult()
    {
        // Arrange
        var configurations = new List<ConfigurationReadModel>
        {
            new()
            {
                Name = ConfigurationKeys.HAUrl,
                Value = "https://example.com"
            },
            new()
            {
                Name = ConfigurationKeys.HAToken,
                Value = "token"
            }
        };

        var configurationServiceMock = new Mock<IConfigurationService>();
        configurationServiceMock
            .Setup(s => s.GetAllConfigurationsAsync())
            .ReturnsAsync(configurations);

        var controller = new ConfigurationController(configurationServiceMock.Object);

        // Act
        var actionResult = await controller.GetConfiguration();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var response = Assert.IsAssignableFrom<IEnumerable<ConfigurationReadModel>>(okResult.Value);

        Assert.Equal(2, response.Count());
        Assert.Contains(response, c => c.Name == ConfigurationKeys.HAUrl && c.Value == "https://example.com");
        Assert.Contains(response, c => c.Name == ConfigurationKeys.HAToken && c.Value == "token");
    }

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
    public async Task AddConfiguration_ReturnsOk_WhenAddSucceeds()
    {
        // Arrange
        var createModel = new ConfigurationCreateModel
        {
            Name = ConfigurationKeys.HAUrl,
            Value = "https://example.com"
        };

        var configurationServiceMock = new Mock<IConfigurationService>();
        var controller = new ConfigurationController(configurationServiceMock.Object);

        // Act
        var actionResult = await controller.AddConfiguration(createModel);

        // Assert
        Assert.IsType<OkResult>(actionResult);
        configurationServiceMock.Verify(
            s => s.AddAsync(createModel),
            Times.Once);
    }

    [Fact]
    public async Task AddConfiguration_ReturnsBadRequest_WhenServiceThrowsArgumentException()
    {
        // Arrange
        var createModel = new ConfigurationCreateModel
        {
            Name = ConfigurationKeys.HAUrl,
            Value = string.Empty
        };

        var configurationServiceMock = new Mock<IConfigurationService>();
        configurationServiceMock
            .Setup(s => s.AddAsync(createModel))
            .ThrowsAsync(new ArgumentException("Configuration value cannot be empty."));

        var controller = new ConfigurationController(configurationServiceMock.Object);

        // Act
        var actionResult = await controller.AddConfiguration(createModel);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Configuration value cannot be empty.", badRequestResult.Value);
    }

    [Fact]
    public async Task AddConfiguration_ReturnsConflict_WhenServiceThrowsInvalidOperationException()
    {
        // Arrange
        var createModel = new ConfigurationCreateModel
        {
            Name = ConfigurationKeys.HAUrl,
            Value = "https://example.com"
        };

        var configurationServiceMock = new Mock<IConfigurationService>();
        configurationServiceMock
            .Setup(s => s.AddAsync(createModel))
            .ThrowsAsync(new InvalidOperationException("Configuration already exists."));

        var controller = new ConfigurationController(configurationServiceMock.Object);

        // Act
        var actionResult = await controller.AddConfiguration(createModel);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(actionResult);
        Assert.Equal("Configuration already exists.", conflictResult.Value);
    }

    [Fact]
    public async Task UpdateConfiguration_ReturnsOk_WhenUpdateSucceeds()
    {
        // Arrange
        var createModel = new ConfigurationCreateModel
        {
            Name = ConfigurationKeys.HAUrl,
            Value = "https://updated.example.com"
        };

        var configurationServiceMock = new Mock<IConfigurationService>();
        var controller = new ConfigurationController(configurationServiceMock.Object);

        // Act
        var actionResult = await controller.UpdateConfiguration(createModel);

        // Assert
        Assert.IsType<OkResult>(actionResult);
        configurationServiceMock.Verify(
            s => s.UpdateConfigurationAsync(createModel),
            Times.Once);
    }

    [Fact]
    public async Task UpdateConfiguration_ReturnsBadRequest_WhenServiceThrowsArgumentException()
    {
        // Arrange
        var createModel = new ConfigurationCreateModel
        {
            Name = ConfigurationKeys.HAUrl,
            Value = string.Empty
        };

        var configurationServiceMock = new Mock<IConfigurationService>();
        configurationServiceMock
            .Setup(s => s.UpdateConfigurationAsync(createModel))
            .ThrowsAsync(new ArgumentException("Configuration value cannot be empty."));

        var controller = new ConfigurationController(configurationServiceMock.Object);

        // Act
        var actionResult = await controller.UpdateConfiguration(createModel);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        Assert.Equal("Configuration value cannot be empty.", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateConfiguration_ReturnsNotFoundOrConflict_WhenServiceThrowsInvalidOperationException()
    {
        // Arrange
        var createModel = new ConfigurationCreateModel
        {
            Name = ConfigurationKeys.HAUrl,
            Value = "https://example.com"
        };

        var configurationServiceMock = new Mock<IConfigurationService>();
        configurationServiceMock
            .Setup(s => s.UpdateConfigurationAsync(createModel))
            .ThrowsAsync(new InvalidOperationException("Configuration does not exist."));

        var controller = new ConfigurationController(configurationServiceMock.Object);

        // Act
        var actionResult = await controller.UpdateConfiguration(createModel);

        // Assert
        var statusCodeResult = Assert.IsAssignableFrom<ObjectResult>(actionResult);
        Assert.True(statusCodeResult.StatusCode is 404 or 409);
        Assert.Equal("Configuration does not exist.", statusCodeResult.Value);
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
