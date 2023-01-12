using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nabto.Edge.Client.Impl;

public class Connection : Nabto.Edge.Client.Connection {

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClient _client;

    public static Connection Create(Nabto.Edge.Client.Impl.NabtoClient client)
    {
        IntPtr ptr = NabtoClientNative.nabto_client_connection_new(client.GetHandle());
        if (ptr == IntPtr.Zero) {
            throw new NullReferenceException();
        }
        return new Connection(client, ptr);
    }



    public Connection(Nabto.Edge.Client.Impl.NabtoClient client, IntPtr handle) {
        _client = client;
        _handle = handle;
    }

    ~Connection()
    {
        NabtoClientNative.nabto_client_connection_free(_handle);
    }

    public void SetOptions(string json) {
        int ec = NabtoClientNative.nabto_client_connection_set_options(_handle, json);
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
        int ec = NabtoClientNative.ConnectionGetDeviceFingerprint(_handle, out fingerprint);
        if (ec != 0) {
            throw NabtoException.Create(ec);
        }
        return fingerprint;
    }

    public string GetClientFingerprint() {
        string fingerprint;
        int ec = NabtoClientNative.ConnectionGetClientFingerprint(_handle, out fingerprint);
        if (ec != 0) {
            throw NabtoException.Create(ec);
        }
        return fingerprint;
    }

    public Task ConnectAsync() {
        TaskCompletionSource connectTask = new TaskCompletionSource();
        var task = connectTask.Task;

        var future = Future.Create(_client);

        NabtoClientNative.nabto_client_connection_connect(_handle, future.GetHandle());

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
        return CoapRequest.Create(_client, this, method, path);
    }

    public IntPtr GetHandle() {
        return _handle;
    }
};
