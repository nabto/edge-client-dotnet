using System.Runtime.InteropServices;

namespace Nabto.Edge.Client.Impl;

public class TcpTunnel : Nabto.Edge.Client.TcpTunnel {

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClient _client;
    private Nabto.Edge.Client.Impl.Connection _connection;

    public static Nabto.Edge.Client.TcpTunnel Create(Nabto.Edge.Client.Impl.NabtoClient client, Nabto.Edge.Client.Impl.Connection connection)
    {
        IntPtr ptr = NabtoClientNative.nabto_client_tcp_tunnel_new(connection.GetHandle());
        if (ptr == IntPtr.Zero) {
            throw new NullReferenceException();
        }
        return new TcpTunnel(client, connection, ptr);
    }

    public TcpTunnel(Nabto.Edge.Client.Impl.NabtoClient client, Nabto.Edge.Client.Impl.Connection connection, IntPtr handle)
    {
        _client = client;
        _connection = connection;
        _handle = handle;
    }

    ~TcpTunnel()
    {
        NabtoClientNative.nabto_client_tcp_tunnel_free(_handle);
    }

    public Task OpenAsync(string service, ushort localPort)
    {
        TaskCompletionSource connectTask = new TaskCompletionSource();
        var task = connectTask.Task;

        var future = Future.Create(_client);

        NabtoClientNative.nabto_client_tcp_tunnel_open(_handle, future.GetHandle(), service, localPort);

        future.Wait((ec) => {
            if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value()) {
                connectTask.SetResult();
            } else {
                connectTask.SetException(NabtoException.Create(ec));
            }
        });

        return task;
    }
    public Task CloseAsync()
    {
        TaskCompletionSource connectTask = new TaskCompletionSource();
        var task = connectTask.Task;

        var future = Future.Create(_client);

        NabtoClientNative.nabto_client_tcp_tunnel_close(_handle, future.GetHandle());

        future.Wait((ec) => {
            if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value()) {

                connectTask.SetResult();
            } else {
                connectTask.SetException(NabtoException.Create(ec));
            }
        });

        return task;
    }

    public ushort GetLocalPort() {
        ushort localPort = 0;
        int ec = NabtoClientNative.nabto_client_tcp_tunnel_get_local_port(_handle, out localPort);
        if (ec != NabtoClientNative.NABTO_CLIENT_EC_OK_value()) {
            throw NabtoException.Create(ec);
        }
        return localPort;
    }
}
