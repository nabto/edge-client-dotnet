namespace Nabto.Edge.Client.Impl;
using System.Runtime.InteropServices;

class FutureImpl : IDisposable, IAsyncDisposable
{

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClientImpl _client;
    private GCHandle? _gcHandle;
    private bool _disposed;

    TaskCompletionSource<int>? _waitTask;

    private static void AssertClientIsAlive(NabtoClientImpl client)
    {
        if (client._disposed)
        {
            throw new ObjectDisposedException("NabtoClient", "The NabtoClient instance associated with this Future instance has been disposed.");
        }
    }

    private void AssertClientIsAlive()
    {
        AssertClientIsAlive(_client);
    }

    public static FutureImpl Create(Nabto.Edge.Client.Impl.NabtoClientImpl client)
    {
        AssertClientIsAlive(client);
        IntPtr ptr = NabtoClientNative.nabto_client_future_new(client.GetHandle());
        if (ptr == IntPtr.Zero)
        {
            throw new AllocationException("Could not allocate Future");
        }
        return new FutureImpl(client, ptr);
    }

    public FutureImpl(Nabto.Edge.Client.Impl.NabtoClientImpl client, IntPtr handle)
    {
        _client = client;
        _handle = handle;
    }

    public IntPtr GetHandle()
    {
        AssertClientIsAlive();
        return _handle;
    }

    private static void CallbackHandler(IntPtr ptr, int ec, IntPtr userData)
    {
        GCHandle gch = GCHandle.FromIntPtr(userData);
        FutureImpl? future = (FutureImpl?)gch.Target;
        future?.HandleCallback(ec);
    }

    private void HandleCallback(int ec)
    {
        var waitTask = _waitTask;
        _waitTask = null;
        _gcHandle?.Free();
        waitTask?.SetResult(ec);
    }

    public delegate void WaitCallbackHandler(int ec);


    // Ensure that the FutureCallbackFunc delegate is not garbage collected.
    private readonly NabtoClientNative.FutureCallbackFunc _callbackHandler = new NabtoClientNative.FutureCallbackFunc(CallbackHandler);

    public Task<int> WaitAsync()
    {
        if (_waitTask != null)
        {
            throw new Exception("Already waiting for a callback on the future.");
        }
        _waitTask = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var task = _waitTask.Task;

        GCHandle handle = GCHandle.Alloc(this);
        _gcHandle = handle;

        AssertClientIsAlive();
        NabtoClientNative.nabto_client_future_set_callback(_handle, _callbackHandler, GCHandle.ToIntPtr(handle));

        return task;
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
    ~FutureImpl()
    {
        Dispose(false);
    }

    /// <summary>Do the actual resource disposal here</summary>
    protected void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            NabtoClientNative.nabto_client_future_free(_handle);
        }
        _disposed = true;
    }

}
