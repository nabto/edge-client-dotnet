namespace Nabto.Edge.Client.Impl;

class Future {

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClient _client;
    private NabtoClientNative.FutureCallbackFunc? _cb;

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

    public delegate void WaitCallbackHandler(int ec);
    public void Wait(WaitCallbackHandler cb)
    {
        if (_cb != null) {
            throw new Exception("Already waiting for a callback on the future.");
        }

        _cb = (ptr, ec, userData) => {
            _cb = null;
            cb(ec);
        };

        NabtoClientNative.nabto_client_future_set_callback(_handle, _cb, IntPtr.Zero);
    }
}
