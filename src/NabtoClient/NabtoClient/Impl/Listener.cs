namespace Nabto.Edge.Client.Impl;

public class Listener
{

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClient client_;

    public static Listener Create(Nabto.Edge.Client.Impl.NabtoClient client)
    {
        IntPtr ptr = NabtoClientNative.nabto_client_listener_new(client.GetHandle());
        if (ptr == IntPtr.Zero)
        {
            throw new NullReferenceException();
        }
        return new Listener(client, ptr);
    }



    public Listener(Nabto.Edge.Client.Impl.NabtoClient client, IntPtr handle)
    {
        client_ = client;
        _handle = handle;
    }

    public void Stop()
    {
        NabtoClientNative.nabto_client_listener_stop(_handle);
    }

    ~Listener()
    {
        //NabtoClientNative.nabto_client_listener_free(_handle);
    }

    public IntPtr GetHandle()
    {
        return _handle;
    }
}
