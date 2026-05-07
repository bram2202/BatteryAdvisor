using System.Net.WebSockets;
using BatteryAdvisor.Core.Contracts.Enums;
using BatteryAdvisor.Core.Contracts.Models;
using BatteryAdvisor.Core.Models.HomeAssistant;
using BatteryAdvisor.Core.Contracts.Services;
using BatteryAdvisor.HA.Clients;
using BatteryAdvisor.HA.Contracts.Services;
using BatteryAdvisor.HA.Helpers;
using Microsoft.Extensions.Logging.Abstractions;

namespace BatteryAdvisor.HA.Tests.Clients;

public class WebSocketClientTests
{
    [Fact]
    public async Task GetStatisticIds_ConnectsAuthenticatesAndReturnsModels()
    {
        var webSocketService = new FakeWebSocketService();
        var responseService = new FakeResponseService(new[] { new StaticIdModel { StatisticId = "sensor.test" } });
        var authenticationService = new FakeAuthenticationService();

        var client = CreateClient(webSocketService, responseService, authenticationService, "https://ha.local");

        var result = await client.GetStatisticIds();

        Assert.Single(result);
        Assert.Equal("sensor.test", result[0].StatisticId);

        Assert.Equal(1, authenticationService.CallCount);
        Assert.Equal("token-123", authenticationService.LastToken);
        Assert.Equal(1, webSocketService.SendCallCount);

        Assert.Contains("\"type\":\"recorder/list_statistic_ids\"", webSocketService.LastSentMessage);
        Assert.Equal(1, responseService.CallCount);
    }

    [Fact]
    public async Task GetStatisticIds_WhenCalledTwice_AuthenticatesOnlyOnce()
    {
        var webSocketService = new FakeWebSocketService();
        var responseService = new FakeResponseService(Array.Empty<StaticIdModel>());
        var authenticationService = new FakeAuthenticationService();

        var client = CreateClient(webSocketService, responseService, authenticationService, "https://ha.local");

        await client.GetStatisticIds();
        await client.GetStatisticIds();

        Assert.Equal(1, authenticationService.CallCount);
        Assert.Equal(2, webSocketService.SendCallCount);
        Assert.Equal(2, responseService.CallCount);
    }

    [Fact]
    public async Task GetStatisticIds_UsesWebSocketUrlBasedOnConfiguredHttpUrl()
    {
        var webSocketService = new FakeWebSocketService();
        var responseService = new FakeResponseService(Array.Empty<StaticIdModel>());
        var authenticationService = new FakeAuthenticationService();

        var client = CreateClient(webSocketService, responseService, authenticationService, "http://localhost:8123");

        await client.GetStatisticIds();

        Assert.StartsWith("ws://localhost:8123", webSocketService.LastConnectUrl);
        Assert.EndsWith("/api/websocket", webSocketService.LastConnectUrl);
    }

    private static WebSocketClient CreateClient(
        FakeWebSocketService webSocketService,
        FakeResponseService responseService,
        FakeAuthenticationService authenticationService,
        string homeAssistantUrl)
    {
        var configurationService = new FakeConfigurationService(homeAssistantUrl, "token-123");

        return new WebSocketClient(
            webSocketService,
            responseService,
            authenticationService,
            NullLogger<WebSocketClient>.Instance,
            new WebSocketMessageHelper(),
            configurationService);
    }

    private sealed class FakeWebSocketService : IWebSocketService
    {
        public string LastConnectUrl { get; private set; } = string.Empty;
        public string LastSentMessage { get; private set; } = string.Empty;
        public int SendCallCount { get; private set; }

        public bool IsConnected => true;

        public Task<ClientWebSocket> GetOrConnectAsync(string url, CancellationToken cancellationToken)
        {
            LastConnectUrl = url;
            return Task.FromResult(new ClientWebSocket());
        }

        public Task CloseAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task SendAsync(string message, CancellationToken cancellationToken)
        {
            LastSentMessage = message;
            SendCallCount++;
            return Task.CompletedTask;
        }

        public Task<string> ReceiveAsync(CancellationToken cancellationToken)
            => Task.FromResult(string.Empty);
    }

    private sealed class FakeResponseService : IHomeAssistantWebSocketResponseService
    {
        private readonly StaticIdModel[] _result;

        public int CallCount { get; private set; }

        public FakeResponseService(StaticIdModel[] result)
        {
            _result = result;
        }

        public Task<T> ReceiveForMessageIdAsync<T>(int messageId, CancellationToken cancellationToken, string? resultPropertyName = null)
        {
            CallCount++;
            return Task.FromResult((T)(object)_result);
        }
    }

    private sealed class FakeAuthenticationService : IWebSocketAuthenticationService
    {
        public int CallCount { get; private set; }
        public string LastToken { get; private set; } = string.Empty;
        public bool IsAuthenticated { get; private set; }

        public Task AuthenticateAsync(string accessToken, CancellationToken cancellationToken)
        {
            CallCount++;
            LastToken = accessToken;
            IsAuthenticated = true;
            return Task.CompletedTask;
        }

        public void Reset() => IsAuthenticated = false;
    }

    private sealed class FakeConfigurationService : IConfigurationService
    {
        private readonly string _url;
        private readonly string _token;

        public FakeConfigurationService(string url, string token)
        {
            _url = url;
            _token = token;
        }

        public Task<ConfigurationReadModel?> GetConfigurationAsync(ConfigurationKeys key)
        {
            var value = key switch
            {
                ConfigurationKeys.HomeAssistantUrl => _url,
                ConfigurationKeys.HomeAssistantToken => _token,
                _ => null
            };

            var result = value is not null
                ? new ConfigurationReadModel { Name = key, Value = value }
                : null;

            return Task.FromResult<ConfigurationReadModel?>(result);
        }

        public Task AddAsync(ConfigurationCreateModel configuration) => Task.CompletedTask;
        public Task AddOrUpdateAsync(ConfigurationCreateModel configuration) => Task.CompletedTask;
        public Task<IEnumerable<ConfigurationReadModel>> GetAllConfigurationsAsync() => Task.FromResult(Enumerable.Empty<ConfigurationReadModel>());
        public Task UpdateConfigurationAsync(ConfigurationCreateModel configuration) => Task.CompletedTask;
        public Task DeleteConfigurationAsync(ConfigurationKeys key) => Task.CompletedTask;
    }
}
