using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nabto.Edge.Client.Impl;

public class Connection : Nabto.Edge.Client.Connection {

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

    ~Connection()
    {
        NabtoClientNative.nabto_client_connection_free(handle_);
    }

    public void SetOptions(string json) {
        int ec = NabtoClientNative.nabto_client_connection_set_options(handle_, json);
        if (ec != 0) {
            throw NabtoException.Create(ec);
        }
    }

    public void SetOptions(ConnectionOptions options)
    {
        var serializerOptions = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        string jsonString = JsonSerializer.Serialize(options, serializerOptions);
        SetOptions(jsonString);
    }

    public string GetDeviceFingerprint() {
        string fingerprint;
        int ec = NabtoClientNative.ConnectionGetDeviceFingerprint(handle_, out fingerprint);
        if (ec != 0) {
            throw NabtoException.Create(ec);
        }
        return fingerprint;
    }

    public string GetClientFingerprint() {
        string fingerprint;
        int ec = NabtoClientNative.ConnectionGetClientFingerprint(handle_, out fingerprint);
        if (ec != 0) {
            throw NabtoException.Create(ec);
        }
        return fingerprint;
    }

    public Task ConnectAsync() {
        TaskCompletionSource connectTask = new TaskCompletionSource();
        var task = connectTask.Task;

        var future = Future.Create(client_);

        NabtoClientNative.nabto_client_connection_connect(handle_, future.GetHandle());

        future.Wait((ec) => {
            if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value()) {
                connectTask.SetResult();
            } else {
                connectTask.SetException(NabtoException.Create(ec));
            }
        });

        return task;
    }

    public Nabto.Edge.Client.CoapRequest CreateCoapRequest(string method, string path)
    {
        return CoapRequest.Create(client_, this, method, path);
    }

    public IntPtr GetHandle() {
        return handle_;
    }
};
