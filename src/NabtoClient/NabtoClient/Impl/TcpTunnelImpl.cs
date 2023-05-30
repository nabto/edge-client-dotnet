using System.Runtime.InteropServices;

namespace Nabto.Edge.Client.Impl;

/// <inheritdoc/>
public class TcpTunnelImpl : Nabto.Edge.Client.TcpTunnel
{

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClientImpl _client;
    private Nabto.Edge.Client.Impl.ConnectionImpl _connection;
    private bool _disposed;

    internal static Nabto.Edge.Client.TcpTunnel Create(Nabto.Edge.Client.Impl.NabtoClientImpl client, Nabto.Edge.Client.Impl.ConnectionImpl connection)
    {
        IntPtr ptr = NabtoClientNative.nabto_client_tcp_tunnel_new(connection.GetHandle());
        if (ptr == IntPtr.Zero)
        {
            throw new NullReferenceException();
        }
        return new TcpTunnelImpl(client, connection, ptr);
    }

    private IntPtr GetHandle()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException("Stream", "The Stream instance has been disposed.");
        }
        return _handle;
    }

    internal TcpTunnelImpl(Nabto.Edge.Client.Impl.NabtoClientImpl client, Nabto.Edge.Client.Impl.ConnectionImpl connection, IntPtr handle)
    {
        _client = client;
        _connection = connection;
        _handle = handle;
    }

    /// <inheritdoc/>
    public async Task OpenAsync(string service, ushort localPort)
    {
        TaskCompletionSource openTask = new TaskCompletionSource();
        var task = openTask.Task;

        var future = FutureImpl.Create(_client);

        NabtoClientNative.nabto_client_tcp_tunnel_open(GetHandle(), future.GetHandle(), service, localPort);

        var ec = await future.WaitAsync();

        if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value())
        {
            return;
        }
        else
        {
            throw NabtoExceptionFactory.Create(ec);
        }
    }

    /// <inheritdoc/>
    public async Task CloseAsync()
    {
        TaskCompletionSource closeTask = new TaskCompletionSource();
        var task = closeTask.Task;

        var future = FutureImpl.Create(_client);

        NabtoClientNative.nabto_client_tcp_tunnel_close(GetHandle(), future.GetHandle());

        var ec = await future.WaitAsync();

        if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value())
        {
            return;
        }
        else
        {
            throw NabtoExceptionFactory.Create(ec);
        }
    }

    /// <inheritdoc/>
    public ushort GetLocalPort()
    {
        ushort localPort = 0;
        int ec = NabtoClientNative.nabto_client_tcp_tunnel_get_local_port(GetHandle(), out localPort);
        if (ec != NabtoClientNative.NABTO_CLIENT_EC_OK_value())
        {
            throw NabtoExceptionFactory.Create(ec);
        }
        return localPort;
    }


    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    ~TcpTunnelImpl()
    {
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            NabtoClientNative.nabto_client_tcp_tunnel_free(_handle);
            _disposed = true;
        }
    }

}
