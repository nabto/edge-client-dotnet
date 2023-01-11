namespace Nabto.Edge.Client.Impl;

class Connection : Nabto.Edge.Client.Connection {

    IntPtr handle_;
    Nabto.Edge.Client.Impl.NabtoClient client_;

    public static Connection Create(Nabto.Edge.Client.Impl.NabtoClient client)
    {
        IntPtr ptr = NabtoClientNative.nabto_client_connection_new(client.GetHandle());
        if (ptr == IntPtr.Zero) {
            throw new NullReferenceException();
        }
        return new Connection(client, ptr);
    }

    public Connection(Nabto.Edge.Client.Impl.NabtoClient client, IntPtr handle) {
        client_ = client;
        handle_ = handle;
    }

    public void SetOptions(string json) {
        int ec = NabtoClientNative.ConnectionSetOptions(handle_, json);
        if (ec != 0) {
            throw NabtoException.Create(ec);
        }
    }

    public string GetDeviceFingerprint() {
        IntPtr reference;
        string fingerprint;
        int ec = NabtoClientNative.ConnectionGetClientFingerprint(handle_, out fingerprint);
        if (ec != 0) {
            throw NabtoException.Create(ec);
        }
        return fingerprint;
    }

    public string GetClientFingerprint() {
        IntPtr reference;
        string fingerprint;
        int ec = NabtoClientNative.ConnectionGetClientFingerprint(handle_, out fingerprint);
        if (ec != 0) {
            throw NabtoException.Create(ec);
        }
        return fingerprint;
    }

    public Task ConnectAsync() {
        return null;
    }
};
