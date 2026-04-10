using System.Net.WebSockets;

namespace BatteryAdvisor.Core.Services;

public class WebSocketService : IWebSocketService, IAsyncDisposable
{
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private ClientWebSocket? _socket;

    public async Task<ClientWebSocket> GetOrConnectAsync(string url, CancellationToken cancellationToken)
    {
        if (_socket is not null && _socket.State == WebSocketState.Open)
        {
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
            await socket.ConnectAsync(new Uri(url), cancellationToken);
            _socket = socket;
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
            return;
        }

        if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived)
        {
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
}