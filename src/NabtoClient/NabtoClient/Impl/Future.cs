namespace Nabto.Edge.Client.Impl;

class Future {

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClient client_;

    public static Future Create(Nabto.Edge.Client.Impl.NabtoClient client)
    {
        IntPtr ptr = NabtoClientNative.nabto_client_future_new(client.GetHandle());
        if (ptr == IntPtr.Zero) {
            throw new NullReferenceException();
        }
        return new Future(client, ptr);
    }



    public Future(Nabto.Edge.Client.Impl.NabtoClient client, IntPtr handle) {
        client_ = client;
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
        // TODO: Can the lambda go out of scope?
        NabtoClientNative.nabto_client_future_set_callback(_handle, (ptr, ec, userData) => cb(ec), IntPtr.Zero);
    }
}
