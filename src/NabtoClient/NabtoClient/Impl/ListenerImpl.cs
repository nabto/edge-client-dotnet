namespace Nabto.Edge.Client.Impl;

public class ListenerImpl
{

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClientImpl client_;

    public static ListenerImpl Create(Nabto.Edge.Client.Impl.NabtoClientImpl client)
    {
        IntPtr ptr = NabtoClientNative.nabto_client_listener_new(client.GetHandle());
        if (ptr == IntPtr.Zero)
        {
            throw new NullReferenceException();
        }
        return new ListenerImpl(client, ptr);
    }



    public ListenerImpl(Nabto.Edge.Client.Impl.NabtoClientImpl client, IntPtr handle)
    {
        client_ = client;
        _handle = handle;
    }

    public void Stop()
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
