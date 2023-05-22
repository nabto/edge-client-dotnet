using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;

namespace Nabto.Edge.Client.Impl;

// This is used to hold a connection event such that we can pin the object such
// that garbage collection does not change the address of the ConnectionEvent.
public class ConnectionEventHolder {
    public int ConnectionEvent;
}

public class ConnectionEventsListenerImpl : IDisposable, IAsyncDisposable
{
    private System.WeakReference<ConnectionImpl> _connection;

    private ListenerImpl _connectionEventsListener;

    private FutureImpl _connectionEventsFuture;

    private Task _eventsListenerTask;

    private bool _disposedUnmanaged;

    private void AssertConnectionIsAlive(ConnectionImpl connection) {
        if (connection._disposedUnmanaged) {
            throw new ObjectDisposedException("Connection", "The Connection instance associated with this ConnectionEventsListener instance has been disposed.");
        }
    }

    private void AssertListenerIsAlive() {
        if (_connectionEventsListener._disposedUnmanaged) {
            throw new ObjectDisposedException("ConnectionEventsListener", "The Listener instance associated with this ConnectionEventsListener instance has been disposed.");
        }
    }


    public ConnectionEventsListenerImpl(ConnectionImpl connection, Nabto.Edge.Client.Impl.NabtoClientImpl client)
    {
        _connection = new System.WeakReference<ConnectionImpl>(connection);

        _connectionEventsListener = ListenerImpl.Create(client);
        _connectionEventsFuture = FutureImpl.Create(client);

        AssertConnectionIsAlive(connection);
        AssertListenerIsAlive();
        
        NabtoClientNative.nabto_client_connection_events_init_listener(connection.GetHandle(), _connectionEventsListener.GetHandle());
        _eventsListenerTask = Task.Run(startListenEvents);
    }

    public void Stop()
    {
        _connectionEventsListener.Stop();
    }



    public async Task startListenEvents()
    {
        // Allocate the connectionEvent on the heap such that we can pin it such that the garbage collector is not moving around with the underlying address of the event.
        var connectionEventHolder = new ConnectionEventHolder();

        GCHandle handle = GCHandle.Alloc(connectionEventHolder, GCHandleType.Pinned);
        while (true)
        {
            AssertListenerIsAlive();
            NabtoClientNative.nabto_client_listener_connection_event(_connectionEventsListener.GetHandle(), _connectionEventsFuture.GetHandle(), out connectionEventHolder.ConnectionEvent);
            var ec = await _connectionEventsFuture.WaitAsync();
            if (ec == 0)
            {
                var connectionEvent = connectionEventHolder.ConnectionEvent;
                ConnectionImpl? connection;
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

        /// <inheritdoc/>
    public void Dispose()
    {
        Console.WriteLine("*** ConnectionEventsListenerImpl Dispose called");
        DisposeUnmanaged();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        Console.WriteLine("*** ConnectionEventsListenerImpl DisposeAsync called");
        DisposeUnmanaged();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    ~ConnectionEventsListenerImpl()
    {
        Console.WriteLine("*** ConnectionEventsListenerImpl finalizer called");
        DisposeUnmanaged();
    }

    private void DisposeUnmanaged() {
        if (!_disposedUnmanaged) {
            _connectionEventsListener.Stop();
            _connectionEventsListener.Dispose();
        }
        _disposedUnmanaged = true;
    }


};

public class ConnectionImpl : Nabto.Edge.Client.Connection
{

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClientImpl _client;
    private ConnectionEventsListenerImpl _connectionEventsListener;
    internal bool _disposedUnmanaged;

    public Nabto.Edge.Client.Connection.ConnectionEventHandler? ConnectionEventHandlers { get; set; }

    private void AssertClientIsAlive() {
        if (_client._disposedUnmanaged) {
            throw new ObjectDisposedException("NabtoClient", "The NabtoClient instance associated with this Connection instance has been disposed.");
        }
    }

  public static ConnectionImpl Create(Nabto.Edge.Client.Impl.NabtoClientImpl client)
    {

        IntPtr ptr = NabtoClientNative.nabto_client_connection_new(client.GetHandle());
        if (ptr == IntPtr.Zero)
        {
            throw new NullReferenceException();
        }
        return new ConnectionImpl(client, ptr);
    }



    public ConnectionImpl(Nabto.Edge.Client.Impl.NabtoClientImpl client, IntPtr handle)
    {
        _client = client;
        _handle = handle;
        _connectionEventsListener = new ConnectionEventsListenerImpl(this, client);
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
            throw NabtoExceptionFactory.Create(ec);
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
            throw NabtoExceptionFactory.Create(ec);
        }
        return fingerprint;
    }

    public string GetClientFingerprint()
    {
        string fingerprint;
        int ec = NabtoClientNative.nabto_client_connection_get_client_fingerprint(_handle, out fingerprint);
        if (ec != 0)
        {
            throw NabtoExceptionFactory.Create(ec);
        }
        return fingerprint;
    }

    public async Task ConnectAsync()
    {
        AssertClientIsAlive();

        TaskCompletionSource connectTask = new TaskCompletionSource();
        var task = connectTask.Task;

        var future = FutureImpl.Create(_client);

        NabtoClientNative.nabto_client_connection_connect(_handle, future.GetHandle());

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

    public async Task CloseAsync()
    {
        AssertClientIsAlive();

        TaskCompletionSource closeTask = new TaskCompletionSource();
        var task = closeTask.Task;

        var future = FutureImpl.Create(_client);

        NabtoClientNative.nabto_client_connection_close(_handle, future.GetHandle());

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

    public async Task PasswordAuthenticate(string username, string password)
    {
        AssertClientIsAlive();

        TaskCompletionSource passwordAuthenticateTask = new TaskCompletionSource();
        var task = passwordAuthenticateTask.Task;

        var future = FutureImpl.Create(_client);

        NabtoClientNative.nabto_client_connection_password_authenticate(_handle, username, password, future.GetHandle());

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
        return CoapRequestImpl.Create(_client, this, method, path);
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

    /// <inheritdoc/>
    public void Dispose()
    {
        Console.WriteLine("*** ConnectionImpl Dispose called");
        DisposeUnmanaged();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        Console.WriteLine("*** ConnectionImpl DisposeAsync called");
        DisposeUnmanaged();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    ~ConnectionImpl()
    {
        Console.WriteLine("*** ConnectionImpl finalizer called");
        DisposeUnmanaged();
    }

    private void DisposeUnmanaged() {
        if (!_disposedUnmanaged) {
            _connectionEventsListener.Stop();
            NabtoClientNative.nabto_client_connection_free(_handle);
        }
        _disposedUnmanaged = true;
    }

}