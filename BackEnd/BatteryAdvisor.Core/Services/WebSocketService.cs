using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Logging;
using BatteryAdvisor.Core.Contracts.Services;

namespace BatteryAdvisor.Core.Services;

public class WebSocketService : IWebSocketService, IAsyncDisposable
{

    private readonly ILogger<WebSocketService> _logger;

    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private ClientWebSocket? _socket;

    public WebSocketService(ILogger<WebSocketService> logger)
    {
        _logger = logger;
    }

    public async Task<ClientWebSocket> GetOrConnectAsync(string url, CancellationToken cancellationToken)
    {
        if (_socket is not null && _socket.State == WebSocketState.Open)
        {
            _logger.LogDebug("Reusing existing open WebSocket connection.");
            return _socket;
        }

        await _connectionLock.WaitAsync(cancellationToken);

        try
        {
            if (_socket is not null && _socket.State == WebSocketState.Open)
            {
                return _socket;
            }

            if (_socket is not null)
            {
                _socket.Dispose();
                _socket = null;
            }

            var socket = new ClientWebSocket();
            _logger.LogInformation("Connecting WebSocket to {Url}.", url);
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(5));

            try
            {
                await socket.ConnectAsync(new Uri(url), timeoutCts.Token);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                socket.Dispose();
                throw new TimeoutException($"WebSocket connection timed out after 5 seconds for URL '{url}'.");
            }

            _socket = socket;
            _logger.LogInformation("WebSocket connected.");
            return socket;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task CloseAsync(CancellationToken cancellationToken)
    {
        ClientWebSocket? socket = null;

        await _connectionLock.WaitAsync(cancellationToken);

        try
        {
            socket = _socket;
            _socket = null;
        }
        finally
        {
            _connectionLock.Release();
        }

        if (socket is null)
        {
            _logger.LogDebug("Close requested but no active WebSocket connection was found.");
            return;
        }

        if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived)
        {
            _logger.LogInformation("Closing WebSocket connection.");
            await socket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Closed by WebSocketService",
                cancellationToken);
        }

        socket.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        ClientWebSocket? socket;

        await _connectionLock.WaitAsync(CancellationToken.None);

        try
        {
            socket = _socket;
            _socket = null;
        }
        finally
        {
            _connectionLock.Release();
            _connectionLock.Dispose();
        }

        if (socket is null)
        {
            return;
        }

        if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived)
        {
            try
            {
                await socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Disposing WebSocketService",
                    CancellationToken.None);
            }
            catch
            {
                // Ignore close failures while disposing resources.
            }
        }

        socket.Dispose();
    }

    public Task SendAsync(string message, CancellationToken cancellationToken)
    {
        if (_socket is null || _socket.State != WebSocketState.Open)
        {
            throw new InvalidOperationException("WebSocket is not connected.");
        }

        _logger.LogDebug("Sending WebSocket message");

        var messageBuffer = System.Text.Encoding.UTF8.GetBytes(message);
        var segment = new ArraySegment<byte>(messageBuffer);

        return _socket.SendAsync(segment, WebSocketMessageType.Text, true, cancellationToken);
    }

    public async Task<string> ReceiveAsync(CancellationToken cancellationToken)
    {
        if (_socket is null || _socket.State != WebSocketState.Open)
        {
            throw new InvalidOperationException("WebSocket is not connected.");
        }

        var buffer = new byte[4096];
        var textBuilder = new StringBuilder();

        while (true)
        {
            var result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                _logger.LogWarning("WebSocket closed while waiting for a message.");
                throw new WebSocketException("WebSocket closed while waiting for a message.");
            }

            if (result.MessageType != WebSocketMessageType.Text)
            {
                continue;
            }

            textBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

            if (result.EndOfMessage)
            {
                _logger.LogDebug("Received WebSocket text message with {CharacterCount} characters.", textBuilder.Length);
                return textBuilder.ToString();
            }
        }
    }

}