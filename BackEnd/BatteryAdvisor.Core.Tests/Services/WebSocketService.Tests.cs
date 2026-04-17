using System.Net.WebSockets;
using BatteryAdvisor.Core.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace BatteryAdvisor.Core.Tests;

public class WebSocketServiceTests
{
    [Fact]
    public async Task SendAsync_WhenNotConnected_ThrowsInvalidOperationException()
    {
        await using var service = new WebSocketService(NullLogger<WebSocketService>.Instance);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.SendAsync("ping", CancellationToken.None));
    }

    [Fact]
    public async Task ReceiveAsync_WhenNotConnected_ThrowsInvalidOperationException()
    {
        await using var service = new WebSocketService(NullLogger<WebSocketService>.Instance);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ReceiveAsync(CancellationToken.None));
    }

    [Fact]
    public async Task CloseAsync_WhenNotConnected_DoesNotThrow()
    {
        await using var service = new WebSocketService(NullLogger<WebSocketService>.Instance);

        var exception = await Record.ExceptionAsync(() => service.CloseAsync(CancellationToken.None));

        Assert.Null(exception);
    }

    [Fact]
    public async Task GetOrConnectAsync_WithInvalidUrl_ThrowsUriFormatException()
    {
        await using var service = new WebSocketService(NullLogger<WebSocketService>.Instance);

        await Assert.ThrowsAsync<UriFormatException>(() =>
            service.GetOrConnectAsync("not a valid url", CancellationToken.None));
    }

    [Fact]
    public async Task GetOrConnectAsync_WhenCancellationRequested_ThrowsCancellationException()
    {
        await using var service = new WebSocketService(NullLogger<WebSocketService>.Instance);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // TaskCanceledException is a subclass of OperationCanceledException
        var ex = await Record.ExceptionAsync(() =>
            service.GetOrConnectAsync("ws://localhost:9999", cts.Token));

        Assert.NotNull(ex);
        Assert.IsAssignableFrom<OperationCanceledException>(ex);
    }

    [Fact]
    public async Task GetOrConnectAsync_WithUnreachableHost_ThrowsWebSocketException()
    {
        // Connection to an unreachable host should fail quickly
        await using var service = new WebSocketService(NullLogger<WebSocketService>.Instance);

        var ex = await Record.ExceptionAsync(() =>
            service.GetOrConnectAsync("ws://192.0.2.1:9999", CancellationToken.None));

        Assert.NotNull(ex);
        // Could be WebSocketException or timeout, depending on network setup
        Assert.True(
            ex is WebSocketException || ex is TimeoutException,
            $"Expected WebSocketException or TimeoutException, but got {ex.GetType().Name}: {ex.Message}");
    }

    [Fact]
    public async Task CloseAsync_WhenCalledMultipleTimes_DoesNotThrow()
    {
        await using var service = new WebSocketService(NullLogger<WebSocketService>.Instance);

        // First close when no connection exists
        var ex1 = await Record.ExceptionAsync(() => service.CloseAsync(CancellationToken.None));
        Assert.Null(ex1);

        // Second close should also not throw
        var ex2 = await Record.ExceptionAsync(() => service.CloseAsync(CancellationToken.None));
        Assert.Null(ex2);
    }

    [Fact]
    public async Task DisposeAsync_WhenNotConnected_Completes()
    {
        var service = new WebSocketService(NullLogger<WebSocketService>.Instance);

        var ex = await Record.ExceptionAsync(() => service.DisposeAsync().AsTask());

        Assert.Null(ex);
    }

}
