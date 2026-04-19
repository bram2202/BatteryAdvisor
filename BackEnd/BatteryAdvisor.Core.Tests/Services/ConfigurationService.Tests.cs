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
    public async Task AddAsync_WithValidConfiguration_AddsSuccessfully()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);
        var config = new ConfigurationCreateModel
        {
            Name = ConfigurationKeys.HAUrl,
            Value = "https://example.com"
        };

        // Act
        await service.AddAsync(config);

        // Assert
        var result = await context.Configurations.SingleOrDefaultAsync(x => x.Name == ConfigurationKeys.HAUrl);
        Assert.NotNull(result);
        Assert.Equal("https://example.com", result.Value);
    }

    [Fact]
    public async Task AddAsync_WithEmptyValue_ThrowsArgumentException()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);
        var config = new ConfigurationCreateModel
        {
            Name = ConfigurationKeys.HAUrl,
            Value = ""
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.AddAsync(config));
        Assert.Contains("value cannot be empty", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AddAsync_WithWhitespaceValue_ThrowsArgumentException()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);
        var config = new ConfigurationCreateModel
        {
            Name = ConfigurationKeys.HAUrl,
            Value = "   "
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => service.AddAsync(config));
        Assert.Contains("value cannot be empty", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AddAsync_WithDuplicateName_ThrowsInvalidOperationException()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);

        var config1 = new ConfigurationCreateModel
        {
            Name = ConfigurationKeys.HAUrl,
            Value = "https://example.com"
        };

        var config2 = new ConfigurationCreateModel
        {
            Name = ConfigurationKeys.HAUrl,
            Value = "https://other.com"
        };

        // Act & Assert
        await service.AddAsync(config1);
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddAsync(config2));
        Assert.Contains("already exists", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AddAsync_TrimsWhitespaceFromValue()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);
        var config = new ConfigurationCreateModel
        {
            Name = ConfigurationKeys.HAToken,
            Value = "  token123  "
        };

        // Act
        await service.AddAsync(config);

        // Assert
        var result = await context.Configurations.SingleAsync(x => x.Name == ConfigurationKeys.HAToken);
        Assert.Equal("token123", result.Value);
    }

    [Fact]
    public async Task AddAsync_AssignsNewGuid()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);
        var config = new ConfigurationCreateModel
        {
            Name = ConfigurationKeys.HAUrl,
            Value = "https://example.com"
        };

        // Act
        await service.AddAsync(config);

        // Assert
        var result = await context.Configurations.SingleAsync(x => x.Name == ConfigurationKeys.HAUrl);
        Assert.NotEqual(Guid.Empty, result.Id);
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
            Name = ConfigurationKeys.HAUrl,
            Value = "https://example.com"
        });
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetConfigurationAsync(ConfigurationKeys.HAUrl);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ConfigurationKeys.HAUrl, result.Name);
        Assert.Equal("https://example.com", result.Value);
    }

    [Fact]
    public async Task GetConfigurationAsync_WhenConfigurationDoesNotExist_ReturnsNull()
    {
        // Arrange
        await using var context = CreateDbContext();
        var service = CreateService(context);

        // Act
        var result = await service.GetConfigurationAsync(ConfigurationKeys.HAUrl);

        // Assert
        Assert.Null(result);
    }
}
