namespace Nabto.Edge.Client.Impl;

internal class ListenerImpl
{

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClientImpl client_;

    internal static ListenerImpl Create(Nabto.Edge.Client.Impl.NabtoClientImpl client)
    {
        IntPtr ptr = NabtoClientNative.nabto_client_listener_new(client.GetHandle());
        if (ptr == IntPtr.Zero)
        {
            throw new NullReferenceException();
        }
        return new ListenerImpl(client, ptr);
    }



    internal ListenerImpl(Nabto.Edge.Client.Impl.NabtoClientImpl client, IntPtr handle)
    {
        client_ = client;
        _handle = handle;
    }

    internal void Stop()
    {
        NabtoClientNative.nabto_client_listener_stop(_handle);
    }

    ~ListenerImpl()
    {
        //NabtoClientNative.nabto_client_listener_free(_handle);
    }

    public IntPtr GetHandle()
    {
        return _handle;
    }
}
