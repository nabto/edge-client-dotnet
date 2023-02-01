using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nabto.Edge.Client.Impl;

public class Connection : Nabto.Edge.Client.Connection {

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClient _client;
    private Listener _connectionEventslistener;
    private int _connectionEvent;
    private Future _connectionEventsFuture;

    public Nabto.Edge.Client.Connection.ConnectionEventHandler? ConnectionEventHandlers { get; set; }

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

        _connectionEventslistener = Listener.Create(client);
        _connectionEventsFuture = Future.Create(_client);

        NabtoClientNative.nabto_client_connection_events_init_listener(_handle, _connectionEventslistener.GetHandle());
        startListenEvents();
    }

    ~Connection()
    {
        NabtoClientNative.nabto_client_connection_free(_handle);
    }

    public void startListenEvents()
    {
        NabtoClientNative.nabto_client_listener_connection_event(_connectionEventslistener.GetHandle(), _connectionEventsFuture.GetHandle(), out _connectionEvent);
        _connectionEventsFuture.Wait((ec) => {
            if (ec == 0) {
                if (_connectionEvent == NabtoClientNative.NABTO_CLIENT_CONNECTION_EVENT_CONNECTED_value()) {
                    ConnectionEventHandlers?.Invoke(Nabto.Edge.Client.Connection.ConnectionEvent.Connected);
                } else if (_connectionEvent == NabtoClientNative.NABTO_CLIENT_CONNECTION_EVENT_CLOSED_value()) {
                    ConnectionEventHandlers?.Invoke(Nabto.Edge.Client.Connection.ConnectionEvent.Closed);
                } else if (_connectionEvent == NabtoClientNative.NABTO_CLIENT_CONNECTION_EVENT_CHANNEL_CHANGED_value()) {
                    ConnectionEventHandlers?.Invoke(Nabto.Edge.Client.Connection.ConnectionEvent.ChannelChanged);
                } else {
                    // TODO log error
                }
            } else if (ec == Nabto.Edge.Client.NabtoClientError.STOPPED) {
                return;
            }
            // TODO handle stopped ec
            startListenEvents();
        });
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
        int ec = NabtoClientNative.nabto_client_connection_get_device_fingerprint(_handle, out fingerprint);
        if (ec != 0) {
            throw NabtoException.Create(ec);
        }
        return fingerprint;
    }

    public string GetClientFingerprint() {
        string fingerprint;
        int ec = NabtoClientNative.nabto_client_connection_get_client_fingerprint(_handle, out fingerprint);
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

    public Task CloseAsync() {
        TaskCompletionSource connectTask = new TaskCompletionSource();
        var task = connectTask.Task;

        var future = Future.Create(_client);

        NabtoClientNative.nabto_client_connection_close(_handle, future.GetHandle());

        future.Wait((ec) => {
            if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value()) {
                connectTask.SetResult();
            } else {
                connectTask.SetException(NabtoException.Create(ec));
            }
        });

        return task;
    }

    public int GetLocalChannelErrorCode() {
        return NabtoClientNative.nabto_client_connection_get_local_channel_error_code(GetHandle());
    }
    public int GetRemoteChannelErrorCode() {
        return NabtoClientNative.nabto_client_connection_get_remote_channel_error_code(GetHandle());
    }
    public int GetDirectCandidatesChannelErrorCode() {
        return NabtoClientNative.nabto_client_connection_get_direct_candidates_channel_error_code(GetHandle());
    }


    public Nabto.Edge.Client.CoapRequest CreateCoapRequest(string method, string path)
    {
        return CoapRequest.Create(_client, this, method, path);
    }

    public Nabto.Edge.Client.Stream CreateStream()
    {
        return Stream.Create(_client, this);
    }

    public Nabto.Edge.Client.TcpTunnel CreateTcpTunnel()
    {
        return TcpTunnel.Create(_client, this);
    }

    public IntPtr GetHandle() {
        return _handle;
    }
};
