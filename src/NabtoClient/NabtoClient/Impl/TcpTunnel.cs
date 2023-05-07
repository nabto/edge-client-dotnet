using System.Runtime.InteropServices;

namespace Nabto.Edge.Client.Impl;

/// <inheritdocs/>
public class TcpTunnel : Nabto.Edge.Client.TcpTunnel
{

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClientImpl _client;
    private Nabto.Edge.Client.Impl.ConnectionImpl _connection;

    internal static Nabto.Edge.Client.TcpTunnel Create(Nabto.Edge.Client.Impl.NabtoClientImpl client, Nabto.Edge.Client.Impl.ConnectionImpl connection)
    {
        IntPtr ptr = NabtoClientNative.nabto_client_tcp_tunnel_new(connection.GetHandle());
        if (ptr == IntPtr.Zero)
        {
            throw new NullReferenceException();
        }
        return new TcpTunnel(client, connection, ptr);
    }

    internal TcpTunnel(Nabto.Edge.Client.Impl.NabtoClientImpl client, Nabto.Edge.Client.Impl.ConnectionImpl connection, IntPtr handle)
    {
        _client = client;
        _connection = connection;
        _handle = handle;
    }

    /// <inheritdocs/>
     ~TcpTunnel()
    {
        NabtoClientNative.nabto_client_tcp_tunnel_free(_handle);
    }

    /// <inheritdocs/>
    public async Task OpenAsync(string service, ushort localPort)
    {
        TaskCompletionSource openTask = new TaskCompletionSource();
        var task = openTask.Task;

        var future = FutureImpl.Create(_client);

        NabtoClientNative.nabto_client_tcp_tunnel_open(_handle, future.GetHandle(), service, localPort);

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

    /// <inheritdocs/>
    public async Task CloseAsync()
    {
        TaskCompletionSource closeTask = new TaskCompletionSource();
        var task = closeTask.Task;

        var future = FutureImpl.Create(_client);

        NabtoClientNative.nabto_client_tcp_tunnel_close(_handle, future.GetHandle());

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

    /// <inheritdocs/>
    public ushort GetLocalPort()
    {
        ushort localPort = 0;
        int ec = NabtoClientNative.nabto_client_tcp_tunnel_get_local_port(_handle, out localPort);
        if (ec != NabtoClientNative.NABTO_CLIENT_EC_OK_value())
        {
            throw NabtoExceptionFactory.Create(ec);
        }
        return localPort;
    }

}
