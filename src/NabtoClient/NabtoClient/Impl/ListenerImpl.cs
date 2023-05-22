namespace Nabto.Edge.Client.Impl;

public class ListenerImpl
{

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClientImpl _client;

    internal bool _disposedUnmanaged;

    private static void AssertClientIsAlive(NabtoClientImpl client) {
        if (client._disposedUnmanaged) {
            throw new ObjectDisposedException("NabtoClient", "The NabtoClient instance associated with this Listener instance has been disposed.");
        }
    }

    private void AssertSelfIsAlive() {
        if (_disposedUnmanaged) {
            throw new ObjectDisposedException("Listener", "This Listener has been disposed.");
        }
    }


    public static ListenerImpl Create(Nabto.Edge.Client.Impl.NabtoClientImpl client)
    {
        AssertClientIsAlive(client);
        IntPtr ptr = NabtoClientNative.nabto_client_listener_new(client.GetHandle());
        if (ptr == IntPtr.Zero)
        {
            throw new NullReferenceException();
        }
        return new ListenerImpl(client, ptr);
    }



    public ListenerImpl(Nabto.Edge.Client.Impl.NabtoClientImpl client, IntPtr handle)
    {
        _client = client;
        _handle = handle;
    }

    public void Stop()
    {
        AssertSelfIsAlive();
        NabtoClientNative.nabto_client_listener_stop(_handle);
    }

    public IntPtr GetHandle()
    {
        return _handle;
    }

            /// <inheritdoc/>
    public void Dispose()
    {
        Console.WriteLine("*** ListenerImpl Dispose called");
        DisposeUnmanaged();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        Console.WriteLine("*** ListenerImpl DisposeAsync called");
        DisposeUnmanaged();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    ~ListenerImpl()
    {
        Console.WriteLine("*** ListenerImpl finalizer called");
        DisposeUnmanaged();
    }

    private void DisposeUnmanaged() {
        if (!_disposedUnmanaged) {
            NabtoClientNative.nabto_client_listener_free(_handle);
        }
        _disposedUnmanaged = true;
    }
}
