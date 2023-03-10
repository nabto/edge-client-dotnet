namespace Nabto.Edge.Client.Impl;
using System.Runtime.InteropServices;

class Future
{

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClient _client;
    private GCHandle? _gcHandle;

    TaskCompletionSource<int>? _waitTask;


    public static Future Create(Nabto.Edge.Client.Impl.NabtoClient client)
    {
        IntPtr ptr = NabtoClientNative.nabto_client_future_new(client.GetHandle());
        if (ptr == IntPtr.Zero)
        {
            throw new NullReferenceException();
        }
        return new Future(client, ptr);
    }



    public Future(Nabto.Edge.Client.Impl.NabtoClient client, IntPtr handle)
    {
        _client = client;
        _handle = handle;
    }

    ~Future()
    {
        NabtoClientNative.nabto_client_future_free(_handle);
    }

    public IntPtr GetHandle()
    {
        return _handle;
    }

    private static void CallbackHandler(IntPtr ptr, int ec, IntPtr userData)
    {
        GCHandle gch = GCHandle.FromIntPtr(userData);
        Future? future = (Future?)gch.Target;
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

        NabtoClientNative.nabto_client_future_set_callback(_handle, CallbackHandler, GCHandle.ToIntPtr(handle));

        return task;
    }
}
