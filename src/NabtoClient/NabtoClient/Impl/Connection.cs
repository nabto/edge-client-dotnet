using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;

namespace Nabto.Edge.Client.Impl;

public class ConnectionEventsListener
{
    private System.WeakReference<Connection> _connection;

    private Listener _connectionEventslistener;

    private Future _connectionEventsFuture;

    private Task _eventsListenerTask;



    public ConnectionEventsListener(Connection connection, Nabto.Edge.Client.Impl.NabtoClient client)
    {
        _connection = new System.WeakReference<Connection>(connection);

        _connectionEventslistener = Listener.Create(client);
        _connectionEventsFuture = Future.Create(client);

        NabtoClientNative.nabto_client_connection_events_init_listener(connection.GetHandle(), _connectionEventslistener.GetHandle());
        _eventsListenerTask = Task.Run(startListenEvents);

    }

    ~ConnectionEventsListener() {
    }

    public void Stop()
    {
        _connectionEventslistener.Stop();
    }



    public async Task startListenEvents()
    {
        // Allocate the connectionEvent on the heap such that we can pin it such that the garbage collector is not moving around with the underlying address of the event.
        var connectionEvent = new int(); 

        GCHandle handle = GCHandle.Alloc(connectionEvent, GCHandleType.Pinned);
        while (true)
        {
            NabtoClientNative.nabto_client_listener_connection_event(_connectionEventslistener.GetHandle(), _connectionEventsFuture.GetHandle(), out connectionEvent);
            var ec = await _connectionEventsFuture.WaitAsync();
            if (ec == 0)
            {
                Connection? connection;
                if (!_connection.TryGetTarget(out connection))
                {
                    return;
                }
                if (connectionEvent == NabtoClientNative.NABTO_CLIENT_CONNECTION_EVENT_CONNECTED_value())
                {
                    connection?.DispatchConnectionEvent(Nabto.Edge.Client.Connection.ConnectionEvent.Connected);
                }
                else if (connectionEvent == NabtoClientNative.NABTO_CLIENT_CONNECTION_EVENT_CLOSED_value())
                {
                    connection?.DispatchConnectionEvent(Nabto.Edge.Client.Connection.ConnectionEvent.Closed);
                }
                else if (connectionEvent == NabtoClientNative.NABTO_CLIENT_CONNECTION_EVENT_CHANNEL_CHANGED_value())
                {
                    connection?.DispatchConnectionEvent(Nabto.Edge.Client.Connection.ConnectionEvent.ChannelChanged);
                }
                else
                {
                    // TODO log error
                }
            }
            else if (ec == Nabto.Edge.Client.NabtoClientError.STOPPED)
            {
                return;
            }
        }
    }

};

public class Connection : Nabto.Edge.Client.Connection
{

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClient _client;
    private ConnectionEventsListener _connectionEventsListener;

    public Nabto.Edge.Client.Connection.ConnectionEventHandler? ConnectionEventHandlers { get; set; }

    public static Connection Create(Nabto.Edge.Client.Impl.NabtoClient client)
    {
        IntPtr ptr = NabtoClientNative.nabto_client_connection_new(client.GetHandle());
        if (ptr == IntPtr.Zero)
        {
            throw new NullReferenceException();
        }
        return new Connection(client, ptr);
    }



    public Connection(Nabto.Edge.Client.Impl.NabtoClient client, IntPtr handle)
    {
        _client = client;
        _handle = handle;
        _connectionEventsListener = new ConnectionEventsListener(this, client);
    }

    ~Connection()
    {
        _connectionEventsListener.Stop();
        NabtoClientNative.nabto_client_connection_free(_handle);
    }

    public void DispatchConnectionEvent(Nabto.Edge.Client.Connection.ConnectionEvent e)
    {
        ConnectionEventHandlers?.Invoke(e);
    }

    public void SetOptions(string json)
    {
        int ec = NabtoClientNative.nabto_client_connection_set_options(_handle, json);
        if (ec != 0)
        {
            throw NabtoException.Create(ec);
        }
    }

    public void SetOptions(ConnectionOptions options)
    {
        var serializerOptions = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        string jsonString = JsonSerializer.Serialize(options, serializerOptions);
        SetOptions(jsonString);
    }

    public string GetDeviceFingerprint()
    {
        string fingerprint;
        int ec = NabtoClientNative.nabto_client_connection_get_device_fingerprint(_handle, out fingerprint);
        if (ec != 0)
        {
            throw NabtoException.Create(ec);
        }
        return fingerprint;
    }

    public string GetClientFingerprint()
    {
        string fingerprint;
        int ec = NabtoClientNative.nabto_client_connection_get_client_fingerprint(_handle, out fingerprint);
        if (ec != 0)
        {
            throw NabtoException.Create(ec);
        }
        return fingerprint;
    }

    public async Task ConnectAsync()
    {
        TaskCompletionSource connectTask = new TaskCompletionSource();
        var task = connectTask.Task;

        var future = Future.Create(_client);

        NabtoClientNative.nabto_client_connection_connect(_handle, future.GetHandle());

        var ec = await future.WaitAsync();
        if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value())
        {
            return;
        }
        else
        {
            throw NabtoException.Create(ec);
        }
    }

    public async Task CloseAsync()
    {
        TaskCompletionSource closeTask = new TaskCompletionSource();
        var task = closeTask.Task;

        var future = Future.Create(_client);

        NabtoClientNative.nabto_client_connection_close(_handle, future.GetHandle());

        var ec = await future.WaitAsync();
        if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value())
        {
            return;
        }
        else
        {
            throw NabtoException.Create(ec);
        }
    }

    public async Task PasswordAuthenticate(string username, string password)
    {
        TaskCompletionSource passwordAuthenticateTask = new TaskCompletionSource();
        var task = passwordAuthenticateTask.Task;

        var future = Future.Create(_client);

        NabtoClientNative.nabto_client_connection_password_authenticate(_handle, username, password, future.GetHandle());

        var ec = await future.WaitAsync();
        if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value())
        {
            return;
        }
        else
        {
            throw NabtoException.Create(ec);
        }
    }

    public int GetLocalChannelErrorCode()
    {
        return NabtoClientNative.nabto_client_connection_get_local_channel_error_code(GetHandle());
    }
    public int GetRemoteChannelErrorCode()
    {
        return NabtoClientNative.nabto_client_connection_get_remote_channel_error_code(GetHandle());
    }
    public int GetDirectCandidatesChannelErrorCode()
    {
        return NabtoClientNative.nabto_client_connection_get_direct_candidates_channel_error_code(GetHandle());
    }


    public Nabto.Edge.Client.CoapRequest CreateCoapRequest(string method, string path)
    {
        return CoapRequest.Create(_client, this, method, path);
    }

    public Nabto.Edge.Client.Stream CreateStream()
    {
        return Stream.Create(_client, this);
    }

    public Nabto.Edge.Client.TcpTunnel CreateTcpTunnel()
    {
        return TcpTunnel.Create(_client, this);
    }

    public IntPtr GetHandle()
    {
        return _handle;
    }
};
