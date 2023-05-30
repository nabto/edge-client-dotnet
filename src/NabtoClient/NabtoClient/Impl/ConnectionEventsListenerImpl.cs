using System.Runtime.InteropServices;

namespace Nabto.Edge.Client.Impl;


internal class ConnectionEventsListenerImpl : IDisposable, IAsyncDisposable
{
    private System.WeakReference<ConnectionImpl> _connection;

    private ListenerImpl _listener;

    private FutureImpl _connectionEventsFuture;

    private Task _eventsListenerTask;

    private bool _disposed;
    private bool _stopped = false;

    private void AssertConnectionIsAlive(ConnectionImpl connection)
    {
        if (connection._disposed)
        {
            throw new ObjectDisposedException("Connection", "The Connection instance associated with this ConnectionEventsListener instance has been disposed.");
        }
    }

    private void AssertListenerIsAlive()
    {
        if (_listener._disposed)
        {
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
        if (_stopped) {
            return;
        }
        _stopped = true;
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
                _listener.Dispose();
                _connectionEventsFuture.Dispose();
                return;
            }
        }
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
    ~ConnectionEventsListenerImpl()
    {
        Dispose(false);
    }

    /// <summary>Do the actual resource disposal here</summary>
    protected void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Stop();
            }
            _disposed = true;
        }
    }


};
