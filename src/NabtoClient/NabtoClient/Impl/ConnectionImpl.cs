using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;

namespace Nabto.Edge.Client.Impl;

/**
 * <summary>This is used to hold a connection event such that we can pin the object, in turn such
 * that garbage collection does not change the address of the ConnectionEvent.</summary>
 */
internal class ConnectionEventHolder {
    internal int ConnectionEvent;
}

/// <inheritdoc />
public class ConnectionEventsListenerImpl : IDisposable, IAsyncDisposable
{
    private System.WeakReference<ConnectionImpl> _connection;

    private ListenerImpl _listener;

    private FutureImpl _connectionEventsFuture;

    private Task _eventsListenerTask;

    private bool _disposedUnmanaged;

    private void AssertConnectionIsAlive(ConnectionImpl connection) {
        if (connection._disposedUnmanaged) {
            throw new ObjectDisposedException("Connection", "The Connection instance associated with this ConnectionEventsListener instance has been disposed.");
        }
    }

    private void AssertListenerIsAlive() {
        if (_listener._disposedUnmanaged) {
            throw new ObjectDisposedException("ConnectionEventsListener", "The Listener instance associated with this ConnectionEventsListener instance has been disposed.");
        }
    }


    /// <inheritdoc />
    public ConnectionEventsListenerImpl(ConnectionImpl connection, Nabto.Edge.Client.Impl.NabtoClientImpl client)
    {
        _connection = new System.WeakReference<ConnectionImpl>(connection);

        _listener = ListenerImpl.Create(client);
        _connectionEventsFuture = FutureImpl.Create(client);

        AssertConnectionIsAlive(connection);
        AssertListenerIsAlive();

        NabtoClientNative.nabto_client_connection_events_init_listener(connection.GetHandle(), _listener.GetHandle());
        _eventsListenerTask = Task.Run(startListenEvents);
    }

    /// <inheritdoc />
    public void Stop()
    {
        _listener.Stop();
    }

    /// <inheritdoc />
    public async Task startListenEvents()
    {
        // Allocate the connectionEvent on the heap such that we can pin it such that the garbage collector is not moving around with the underlying address of the event.
        var connectionEventHolder = new ConnectionEventHolder();

        GCHandle handle = GCHandle.Alloc(connectionEventHolder, GCHandleType.Pinned);
        while (true)
        {
            AssertListenerIsAlive();
            NabtoClientNative.nabto_client_listener_connection_event(_listener.GetHandle(), _connectionEventsFuture.GetHandle(), out connectionEventHolder.ConnectionEvent);
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
        DisposeUnmanaged();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        DisposeUnmanaged();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    ~ConnectionEventsListenerImpl()
    {
        DisposeUnmanaged();
    }


    // public static void LogStack()
    // {
    //     var trace = new System.Diagnostics.StackTrace();
    //     foreach (var frame in trace.GetFrames())
    //     {
    //         var method = frame.GetMethod();
    //         if (method.Name.Equals("LogStack")) continue;
    //         Console.WriteLine(string.Format("    {0}::{1}",
    //             method.ReflectedType != null ? method.ReflectedType.Name : string.Empty,
    //             method.Name));
    //     }
    // }

    private void DisposeUnmanaged() {
        if (!_disposedUnmanaged) {
//            LogStack();
            _listener.Stop();
            _listener.Dispose();
        }
        _disposedUnmanaged = true;
    }


};

/// <inheritdoc />
public class ConnectionImpl : Nabto.Edge.Client.Connection
{

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClientImpl _client;
    private ConnectionEventsListenerImpl _connectionEventsListener;
    internal bool _disposedUnmanaged;

    /// <inheritdoc/>
    public Nabto.Edge.Client.Connection.ConnectionEventHandler? ConnectionEventHandlers { get; set; }

    private void AssertClientIsAlive() {
        if (_client._disposedUnmanaged) {
            throw new ObjectDisposedException("NabtoClient", "The NabtoClient instance associated with this Connection instance has been disposed.");
        }
    }

    internal static ConnectionImpl Create(Nabto.Edge.Client.Impl.NabtoClientImpl client)
    {

        IntPtr ptr = NabtoClientNative.nabto_client_connection_new(client.GetHandle());
        if (ptr == IntPtr.Zero)
        {
            throw new NullReferenceException();
        }
        return new ConnectionImpl(client, ptr);
    }



    /// <inheritdoc />
    public ConnectionImpl(Nabto.Edge.Client.Impl.NabtoClientImpl client, IntPtr handle)
    {
        _client = client;
        _handle = handle;
        _connectionEventsListener = new ConnectionEventsListenerImpl(this, client);
    }

    /// <inheritdoc />
    public void DispatchConnectionEvent(Nabto.Edge.Client.Connection.ConnectionEvent e)
    {
        ConnectionEventHandlers?.Invoke(e);
    }


    /// <inheritdoc />
    public void SetOptions(string json)
    {
        int ec = NabtoClientNative.nabto_client_connection_set_options(_handle, json);
        if (ec != 0)
        {
            throw NabtoExceptionFactory.Create(ec);
        }
    }


    /// <inheritdoc />
    public void SetOptions(ConnectionOptions options)
    {
        var serializerOptions = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        string jsonString = JsonSerializer.Serialize(options, serializerOptions);
        SetOptions(jsonString);
    }


    /// <inheritdoc />
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


    /// <inheritdoc />
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


    /// <inheritdoc />
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


    /// <inheritdoc />
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


    /// <inheritdoc />
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

    /// <inheritdoc />
    public int GetLocalChannelErrorCode()
    {
        return NabtoClientNative.nabto_client_connection_get_local_channel_error_code(GetHandle());
    }

    /// <inheritdoc />
    public int GetRemoteChannelErrorCode()
    {
        return NabtoClientNative.nabto_client_connection_get_remote_channel_error_code(GetHandle());
    }

    /// <inheritdoc />
    public int GetDirectCandidatesChannelErrorCode()
    {
        return NabtoClientNative.nabto_client_connection_get_direct_candidates_channel_error_code(GetHandle());
    }


    /// <inheritdoc />
    public Nabto.Edge.Client.CoapRequest CreateCoapRequest(string method, string path)
    {
        return CoapRequestImpl.Create(_client, this, method, path);
    }

    /// <inheritdoc />
    public Nabto.Edge.Client.Stream CreateStream()
    {
        return Stream.Create(_client, this);
    }

    /// <inheritdoc />
    public Nabto.Edge.Client.TcpTunnel CreateTcpTunnel()
    {
        return TcpTunnel.Create(_client, this);
    }

    /// <inheritdoc />
    public Connection.ConnectionType GetConnectionType()
    {
        int connectionType;
        int ec = NabtoClientNative.nabto_client_connection_get_type(GetHandle(), out connectionType);
        if (ec != NabtoClientNative.NABTO_CLIENT_EC_OK_value()) {
            throw NabtoExceptionFactory.Create(ec);
        }
        switch ((NabtoClientNative.NabtoClientConnectionType)connectionType) {
            case NabtoClientNative.NabtoClientConnectionType.Direct: return Connection.ConnectionType.Direct;
            case NabtoClientNative.NabtoClientConnectionType.Relay: return Connection.ConnectionType.Relay;
            default: throw NabtoExceptionFactory.Create(NabtoClientNative.NABTO_CLIENT_EC_UNKNOWN_value());
        }
    }

    /// <inheritdoc />
    public IntPtr GetHandle()
    {
        return _handle;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        DisposeUnmanaged();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        DisposeUnmanaged();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    ~ConnectionImpl()
    {
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