namespace Nabto.Edge.Client.Impl;

public class NabtoClient : Nabto.Edge.Client.NabtoClient {
    private IntPtr _handle;

    public NabtoClient(IntPtr h)
    {
        _handle = h;
    }

    ~NabtoClient() {
        NabtoClientNative.nabto_client_free(_handle);
    }

    public IntPtr GetHandle() {
        return _handle;
    }

    public string GetVersion() {
        return NabtoClientNative.GetVersion();
    }

    public Nabto.Edge.Client.Connection CreateConnection()
    {
        return Nabto.Edge.Client.Impl.Connection.Create(this);
    }
}
