namespace Nabto.Edge.Client.Impl;

internal class ListenerImpl
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


    internal static ListenerImpl Create(Nabto.Edge.Client.Impl.NabtoClientImpl client)
    {
        AssertClientIsAlive(client);
        IntPtr ptr = NabtoClientNative.nabto_client_listener_new(client.GetHandle());
        if (ptr == IntPtr.Zero)
        {
            throw new NullReferenceException();
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
        if (!_disposedUnmanaged) {
            NabtoClientNative.nabto_client_listener_stop(_handle);
        } else {
            // just nop, don't throw ObjectDisposedException (this only happens when invoked internally); Stop() may be invoked from owner's finalizer 
            // and this instance may itself have been disposed from a finalizer if both objects were eligible (no guarantee on ordering, despite owner having a reference)
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
    ~ListenerImpl()
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

    private void DisposeUnmanaged()
    {
        if (!_disposedUnmanaged) {            
//            Console.WriteLine(" *** ListenerImpl.DisposeUnmanaged(); ${0}", this.GetHashCode());
//            LogStack();
            NabtoClientNative.nabto_client_listener_free(_handle);
        }
        _disposedUnmanaged = true;
    }
}
