using BatteryAdvisor.Core.Contracts.Enums;
using BatteryAdvisor.Core.Contracts.Models;
using BatteryAdvisor.Core.Database;
using BatteryAdvisor.Core.Models.Database;
using BatteryAdvisor.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace BatteryAdvisor.Core.Tests.Services;

public class ConfigurationServiceTests
{
    private static BatteryAdvisorContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<BatteryAdvisorContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new BatteryAdvisorContext(options);
    }

    private static ConfigurationService CreateService(BatteryAdvisorContext context)
    {
        var databaseService = new DatabaseService(context);
        return new ConfigurationService(databaseService, context);
    }

    [Fact]
    public async Task GetConfigurationAsync_WhenConfigurationExists_ReturnsReadModel()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);

        context.Configurations.Add(new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            Name = ConfigurationKeys.HomeAssistantUrl,
            Value = "https://example.com"
        });
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetConfigurationAsync(ConfigurationKeys.HomeAssistantUrl);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ConfigurationKeys.HomeAssistantUrl, result.Name);
        Assert.Equal("https://example.com", result.Value);
    }

    [Fact]
    public async Task GetConfigurationAsync_WhenConfigurationDoesNotExist_ReturnsNull()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);

        // Act
        var result = await service.GetConfigurationAsync(ConfigurationKeys.HomeAssistantUrl);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetConfigurationAsync_WithInvalidKey_ThrowsArgumentException()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.GetConfigurationAsync((ConfigurationKeys)999));
        Assert.Contains("invalid configuration key", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdateConfigurationAsync_WhenConfigurationExists_UpdatesValue()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);

        context.Configurations.Add(new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            Name = ConfigurationKeys.HomeAssistantUrl,
            Value = "https://old.example.com"
        });
        await context.SaveChangesAsync();

        var updateModel = new ConfigurationCreateModel
        {
            Name = ConfigurationKeys.HomeAssistantUrl,
            Value = "  https://new.example.com  "
        };

        // Act
        await service.UpdateConfigurationAsync(updateModel);

        // Assert
        var result = await context.Configurations.SingleAsync(x => x.Name == ConfigurationKeys.HomeAssistantUrl);
        Assert.Equal("https://new.example.com", result.Value);
    }

    [Fact]
    public async Task UpdateConfigurationAsync_WithInvalidName_ThrowsArgumentException()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);
        var updateModel = new ConfigurationCreateModel
        {
            Name = (ConfigurationKeys)999,
            Value = "some-value"
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateConfigurationAsync(updateModel));
        Assert.Contains("invalid configuration name", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdateConfigurationAsync_WhenConfigurationDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);
        var updateModel = new ConfigurationCreateModel
        {
            Name = ConfigurationKeys.HomeAssistantToken,
            Value = "token-value"
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateConfigurationAsync(updateModel));
        Assert.Contains("does not exist", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetAllConfigurationsAsync_WhenNoConfigurationsExist_ReturnsEmptyCollection()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);

        // Act
        var result = await service.GetAllConfigurationsAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllConfigurationsAsync_WhenConfigurationsExist_ReturnsAll()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);

        context.Configurations.AddRange(
            new BatteryAdvisor.Core.Models.Database.ConfigurationModel { Id = Guid.NewGuid(), Name = ConfigurationKeys.HomeAssistantUrl, Value = "https://example.com" },
            new BatteryAdvisor.Core.Models.Database.ConfigurationModel { Id = Guid.NewGuid(), Name = ConfigurationKeys.HomeAssistantToken, Value = "token123" }
        );
        await context.SaveChangesAsync();

        // Act
        var result = (await service.GetAllConfigurationsAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Name == ConfigurationKeys.HomeAssistantUrl && r.Value == "https://example.com");
        Assert.Contains(result, r => r.Name == ConfigurationKeys.HomeAssistantToken && r.Value == "token123");
    }

    [Fact]
    public async Task GetAllConfigurationsAsync_WhenMaskSensitiveValuesIsTrue_MasksToken()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);

        context.Configurations.AddRange(
            new BatteryAdvisor.Core.Models.Database.ConfigurationModel { Id = Guid.NewGuid(), Name = ConfigurationKeys.HomeAssistantUrl, Value = "https://example.com" },
            new BatteryAdvisor.Core.Models.Database.ConfigurationModel { Id = Guid.NewGuid(), Name = ConfigurationKeys.HomeAssistantToken, Value = "token123" }
        );
        await context.SaveChangesAsync();

        // Act
        var result = (await service.GetAllConfigurationsAsync(maskSensitiveValues: true)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Name == ConfigurationKeys.HomeAssistantUrl && r.Value == "https://example.com");
        Assert.Contains(result, r => r.Name == ConfigurationKeys.HomeAssistantToken && r.Value == "****");
    }

    [Fact]
    public async Task DeleteConfigurationAsync_WhenConfigurationExists_DeletesIt()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);

        context.Configurations.Add(new BatteryAdvisor.Core.Models.Database.ConfigurationModel
        {
            Id = Guid.NewGuid(),
            Name = ConfigurationKeys.HomeAssistantUrl,
            Value = "https://example.com"
        });
        await context.SaveChangesAsync();

        // Act
        await service.DeleteConfigurationAsync(ConfigurationKeys.HomeAssistantUrl);

        // Assert
        var result = await context.Configurations.SingleOrDefaultAsync(x => x.Name == ConfigurationKeys.HomeAssistantUrl);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteConfigurationAsync_WhenConfigurationDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteConfigurationAsync(ConfigurationKeys.HomeAssistantUrl));
        Assert.Contains("does not exist", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeleteConfigurationAsync_WithInvalidKey_ThrowsArgumentException()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.DeleteConfigurationAsync((ConfigurationKeys)999));
        Assert.Contains("invalid configuration key", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
