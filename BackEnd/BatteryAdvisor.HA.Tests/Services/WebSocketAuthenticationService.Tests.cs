using BatteryAdvisor.Core.Services;
using BatteryAdvisor.Core.ApplicationOptions;
using BatteryAdvisor.HA.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Net.WebSockets;
using System.Text.Json;

namespace BatteryAdvisor.HA.Tests.Services;

public class WebSocketAuthenticationServiceTests
{
    [Fact]
    public async Task AuthenticateAsync_WithValidToken_SuccessfullyAuthenticates()
    {
        // Arrange
        var accessToken = "test_access_token";
        var authOkResponse = JsonSerializer.Serialize(new { type = "auth_ok" });
        var webSocketService = new FakeWebSocketService(authOkResponse);
        var authService = CreateAuthService(webSocketService, webSocketResponseTimeoutSeconds: 5);

        // Act
        await authService.AuthenticateAsync(accessToken, CancellationToken.None);

        // Assert
        Assert.Equal($"{{\"type\":\"auth\",\"access_token\":\"{accessToken}\"}}", webSocketService.LastSentMessage);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidToken_ThrowsInvalidOperationException()
    {
        // Arrange
        var accessToken = "invalid_token";
        var authInvalidResponse = JsonSerializer.Serialize(new { type = "auth_invalid", message = "Invalid token" });
        var webSocketService = new FakeWebSocketService(authInvalidResponse);
        var authService = CreateAuthService(webSocketService, webSocketResponseTimeoutSeconds: 5);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => authService.AuthenticateAsync(accessToken, CancellationToken.None));

        Assert.Contains("Home Assistant websocket auth failed", exception.Message);
        Assert.Contains("Invalid token", exception.Message);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidTokenNoMessage_ThrowsInvalidOperationExceptionWithDefaultMessage()
    {
        // Arrange
        var accessToken = "invalid_token";
        var authInvalidResponse = JsonSerializer.Serialize(new { type = "auth_invalid" });
        var webSocketService = new FakeWebSocketService(authInvalidResponse);
        var authService = CreateAuthService(webSocketService, webSocketResponseTimeoutSeconds: 5);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => authService.AuthenticateAsync(accessToken, CancellationToken.None));

        Assert.Contains("Home Assistant websocket auth failed", exception.Message);
        Assert.Contains("auth_invalid", exception.Message);
    }

    [Fact]
    public async Task AuthenticateAsync_SkipsMessagesWithNullType_WaitsForAuthResponse()
    {
        // Arrange
        var accessToken = "test_token";
        var responses = new[]
        {
            JsonSerializer.Serialize(new { type = (string?)null }),
            JsonSerializer.Serialize(new { ha_version = "2024.1.0" }),
            JsonSerializer.Serialize(new { type = "auth_ok" })
        };

        var webSocketService = new MultiResponseFakeWebSocketService(responses);
        var authService = CreateAuthService(webSocketService, webSocketResponseTimeoutSeconds: 5);

        // Act
        await authService.AuthenticateAsync(accessToken, CancellationToken.None);

        // Assert - should complete without throwing
        Assert.Equal(3, webSocketService.ReceiveCallCount);
    }

    [Fact]
    public async Task AuthenticateAsync_WithCancellationToken_RespectsCancellation()
    {
        // Arrange
        var accessToken = "test_token";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var webSocketService = new DelayedFakeWebSocketService();
        var authService = CreateAuthService(webSocketService, webSocketResponseTimeoutSeconds: 5);

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(
            () => authService.AuthenticateAsync(accessToken, cts.Token));
    }

    [Fact]
    public async Task AuthenticateAsync_Timeout_ThrowsOperationCanceledException()
    {
        // Arrange
        var accessToken = "test_token";
        var webSocketService = new NeverRespondingFakeWebSocketService();
        var authService = CreateAuthService(webSocketService, webSocketResponseTimeoutSeconds: 1);

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(
            () => authService.AuthenticateAsync(accessToken, CancellationToken.None));
    }

    /// <summary>
    /// Helper method to create a WebSocketAuthenticationService with the specified fake IWebSocketService and timeout configuration.
    /// </summary>
    private static WebSocketAuthenticationService CreateAuthService(
        IWebSocketService webSocketService,
        int webSocketResponseTimeoutSeconds)
    {
        var options = Options.Create(new ApplicationOptions
        {
            HomeAssistant = new HomeAssistantOptions
            {
                WebSocketResponseTimeoutSeconds = webSocketResponseTimeoutSeconds
            }
        });

        return new WebSocketAuthenticationService(
            webSocketService,
            options,
            NullLogger<WebSocketAuthenticationService>.Instance);
    }

    /// <summary>
    /// Fake IWebSocketService that returns a single response
    /// </summary>
    private sealed class FakeWebSocketService : IWebSocketService
    {
        private readonly string _response;
        public string? LastSentMessage { get; private set; }

        public FakeWebSocketService(string response)
        {
            _response = response;
        }

        public Task<ClientWebSocket> GetOrConnectAsync(string url, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task CloseAsync(CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task SendAsync(string message, CancellationToken cancellationToken)
        {
            LastSentMessage = message;
            return Task.CompletedTask;
        }

        public Task<string> ReceiveAsync(CancellationToken cancellationToken)
            => Task.FromResult(_response);
    }

    /// <summary>
    /// Fake IWebSocketService that returns multiple responses sequentially
    /// </summary>
    private sealed class MultiResponseFakeWebSocketService : IWebSocketService
    {
        private readonly string[] _responses;
        private int _currentIndex = 0;
        public int ReceiveCallCount { get; private set; }

        public MultiResponseFakeWebSocketService(string[] responses)
        {
            _responses = responses;
        }

        public Task<ClientWebSocket> GetOrConnectAsync(string url, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task CloseAsync(CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task SendAsync(string message, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task<string> ReceiveAsync(CancellationToken cancellationToken)
        {
            ReceiveCallCount++;
            var response = _responses[_currentIndex];
            _currentIndex = (_currentIndex + 1) % _responses.Length;
            return Task.FromResult(response);
        }
    }

    /// <summary>
    /// Fake IWebSocketService that delays before responding (tests timeout)
    /// </summary>
    private sealed class DelayedFakeWebSocketService : IWebSocketService
    {
        public Task<ClientWebSocket> GetOrConnectAsync(string url, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task CloseAsync(CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task SendAsync(string message, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public async Task<string> ReceiveAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(5000, cancellationToken);
            return JsonSerializer.Serialize(new { type = "auth_ok" });
        }
    }

    /// <summary>
    /// Fake IWebSocketService that never responds (tests internal timeout)
    /// </summary>
    private sealed class NeverRespondingFakeWebSocketService : IWebSocketService
    {
        public Task<ClientWebSocket> GetOrConnectAsync(string url, CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task CloseAsync(CancellationToken cancellationToken)
            => throw new NotSupportedException();

        public Task SendAsync(string message, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public async Task<string> ReceiveAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
            return string.Empty;
        }
    }
}
