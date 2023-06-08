namespace Nabto.Edge.Client.Impl;

internal class ListenerImpl
{

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClientImpl _client;

    internal bool _disposed;

    private static void AssertClientIsAlive(NabtoClientImpl client)
    {
        if (client._disposed)
        {
            throw new ObjectDisposedException("NabtoClient", "The NabtoClient instance associated with this Listener instance has been disposed.");
        }
    }

    private void AssertSelfIsAlive()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException("Listener", "This Listener has been disposed.");
        }
    }


    internal static ListenerImpl Create(Nabto.Edge.Client.Impl.NabtoClientImpl client)
    {
        AssertClientIsAlive(client);
        IntPtr ptr = NabtoClientNative.nabto_client_listener_new(client.GetHandle());
        if (ptr == IntPtr.Zero)
        {
            throw new AllocationException("Could not allocate Listener");
        }
        return new ListenerImpl(client, ptr);
    }



    internal ListenerImpl(Nabto.Edge.Client.Impl.NabtoClientImpl client, IntPtr handle)
    {
        _client = client;
        _handle = handle;
    }

    internal void Stop()
    {
        if (!_disposed)
        {
            NabtoClientNative.nabto_client_listener_stop(_handle);
        }
        else
        {
            // just nop, don't throw ObjectDisposedException (this only happens when invoked internally); Stop() may be invoked from owner's Dispose()
        }
    }

    public IntPtr GetHandle()
    {
        AssertSelfIsAlive();
        return _handle;
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
    ~ListenerImpl()
    {
        Dispose(false);
    }

    /// <summary>Do the actual resource disposal here</summary>
    protected void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            NabtoClientNative.nabto_client_listener_free(_handle);
            _disposed = true;
        }
    }
}
