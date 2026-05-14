using BatteryAdvisor.Core.Contracts.Enums;
using BatteryAdvisor.Core.Contracts.Models;
using BatteryAdvisor.Core.Contracts.Services;
using BatteryAdvisor.Core.Models.HomeAssistant;
using BatteryAdvisor.HA.Contracts.Clients;
using BatteryAdvisor.HA.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace BatteryAdvisor.HA.Tests.Services;

public class StatisticsServiceTests
{
    [Fact]
    public async Task GetStatisticEntities_ReturnsMappedEntities_WhenStatisticIdsMatchEntities()
    {
        // Arrange
        var statisticIds = new[]
        {
            new StaticIdModel
            {
                StatisticId = "sensor.energy_import",
                DisplayUnitOfMeasurement = "kWh"
            }
        };

        var entities = new[]
        {
            new EntityModel
            {
                EntityId = "sensor.energy_import",
                State = "123.4",
                Attributes = new AttributesModel
                {
                    FriendlyName = "Energy Import",
                    UnitOfMeasurement = "kWh",
                    Icon = "mdi:flash"
                }
            },
            new EntityModel
            {
                EntityId = "sensor.unrelated",
                State = "9",
                Attributes = new AttributesModel
                {
                    FriendlyName = "Unrelated"
                }
            }
        };

        var webSocketClient = new FakeWebSocketClient(statisticIds, entities);
        var service = new EntityService(webSocketClient, NullLogger<EntityService>.Instance, new FakeConfigurationService());

        // Act
        var result = await service.GetStatisticEntities();

        // Assert
        Assert.Single(result);
        Assert.Equal("sensor.energy_import", result[0].EntityId);
        Assert.Equal("Energy Import", result[0].FriendlyName);
        Assert.Equal("kWh", result[0].UnitOfMeasurement);
        Assert.Equal("kWh", result[0].DisplayUnitOfMeasurement);
        Assert.Equal("123.4", result[0].State);
        Assert.Equal("mdi:flash", result[0].Icon);
    }

    [Fact]
    public async Task GetStatisticEntities_UsesEmptyStringsForMissingOptionalFields()
    {
        // Arrange
        var statisticIds = new[]
        {
            new StaticIdModel
            {
                StatisticId = "sensor.energy_import",
                DisplayUnitOfMeasurement = null
            }
        };

        var entities = new[]
        {
            new EntityModel
            {
                EntityId = "sensor.energy_import",
                State = null,
                Attributes = null
            }
        };

        var webSocketClient = new FakeWebSocketClient(statisticIds, entities);
        var service = new EntityService(webSocketClient, NullLogger<EntityService>.Instance, new FakeConfigurationService());

        // Act
        var result = await service.GetStatisticEntities();

        // Assert
        Assert.Single(result);
        Assert.Equal(string.Empty, result[0].FriendlyName);
        Assert.Equal(string.Empty, result[0].UnitOfMeasurement);
        Assert.Equal(string.Empty, result[0].DisplayUnitOfMeasurement);
        Assert.Equal(string.Empty, result[0].State);
        Assert.Equal(string.Empty, result[0].Icon);
    }

    [Fact]
    public async Task GetStatisticEntities_ThrowsInvalidOperationException_WhenStatisticIdsIsNull()
    {
        // Arrange
        var webSocketClient = new FakeWebSocketClient(null, Array.Empty<EntityModel>());
        var service = new EntityService(webSocketClient, NullLogger<EntityService>.Instance, new FakeConfigurationService());

        // Act
        var action = async () => await service.GetStatisticEntities();

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }

    [Fact]
    public async Task GetStatisticEntities_ThrowsInvalidOperationException_WhenEntitiesIsNull()
    {
        // Arrange
        var webSocketClient = new FakeWebSocketClient(Array.Empty<StaticIdModel>(), null);
        var service = new EntityService(webSocketClient, NullLogger<EntityService>.Instance, new FakeConfigurationService());

        // Act
        var action = async () => await service.GetStatisticEntities();

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }

    private sealed class FakeWebSocketClient : IWebSocketClient
    {
        private readonly StaticIdModel[]? _statisticIds;
        private readonly EntityModel[]? _entities;

        public FakeWebSocketClient(StaticIdModel[]? statisticIds, EntityModel[]? entities)
        {
            _statisticIds = statisticIds;
            _entities = entities;
        }

        public Task<StaticIdModel[]> GetStatisticIds()
            => Task.FromResult(_statisticIds!);

        public Task<EntityModel[]> GetEntities()
            => Task.FromResult(_entities!);

        public Task<StatisticsDuringPeriodModel[]> GetStatisticsDuringPeriod(string statisticId, string startTime, string endTime)
            => throw new NotSupportedException();
    }

    private sealed class FakeConfigurationService : IConfigurationService
    {
        public Task AddOrUpdateAsync(ConfigurationCreateModel configuration) => Task.CompletedTask;
        public Task<ConfigurationReadModel?> GetConfigurationAsync(ConfigurationKeys key) => Task.FromResult<ConfigurationReadModel?>(null);
        public Task<IEnumerable<ConfigurationReadModel>> GetAllConfigurationsAsync(bool maskSensitiveValues = false) => Task.FromResult(Enumerable.Empty<ConfigurationReadModel>());
        public Task UpdateConfigurationAsync(ConfigurationCreateModel configuration) => Task.CompletedTask;
        public Task DeleteConfigurationAsync(ConfigurationKeys key) => Task.CompletedTask;
    }
}
