namespace Nabto.Edge.Client.Impl;

class Future {

    IntPtr handle_;
    Nabto.Edge.Client.Impl.NabtoClient client_;

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
        handle_ = handle;
    }

    ~Future()
    {
        NabtoClientNative.nabto_client_future_free(handle_);
    }

    public IntPtr GetHandle()
    {
        return handle_;
    }

    public delegate void WaitCallbackHandler(int ec);
    public void Wait(WaitCallbackHandler cb)
    {
        // TODO: Can the lambda go out of scope?
        NabtoClientNative.nabto_client_future_set_callback(handle_, (ptr, ec, userData) => cb(ec), IntPtr.Zero);
    }
}
