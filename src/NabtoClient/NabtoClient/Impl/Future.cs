namespace Nabto.Edge.Client.Impl;
using System.Runtime.InteropServices;

class Future {

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClient _client;
    //private NabtoClientNative.FutureCallbackFunc? _cb;
    private GCHandle? _gcHandle;
    private WaitCallbackHandler? _waitCallback;


    public static Future Create(Nabto.Edge.Client.Impl.NabtoClient client)
    {
        IntPtr ptr = NabtoClientNative.nabto_client_future_new(client.GetHandle());
        if (ptr == IntPtr.Zero) {
            throw new NullReferenceException();
        }
        return new Future(client, ptr);
    }



    public Future(Nabto.Edge.Client.Impl.NabtoClient client, IntPtr handle) {
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
        Future future = (Future)gch.Target;
        future.HandleCallback(ec);
    }

    private void HandleCallback(int ec) 
    {
        var cb = _waitCallback;
        _waitCallback = null;
        _gcHandle?.Free();
        cb(ec);
    }

    public delegate void WaitCallbackHandler(int ec);
    public void Wait(WaitCallbackHandler cb)
    {
        if (_waitCallback != null) {
            throw new Exception("Already waiting for a callback on the future.");
        }

        GCHandle handle = GCHandle.Alloc(this);
        _gcHandle = handle;
        _waitCallback = cb;

        NabtoClientNative.nabto_client_future_set_callback(_handle, CallbackHandler, GCHandle.ToIntPtr(handle));
    }
}
