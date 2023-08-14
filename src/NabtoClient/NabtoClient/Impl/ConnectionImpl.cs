using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;

namespace Nabto.Edge.Client.Impl;

/// <inheritdoc />
public class ConnectionImpl : Nabto.Edge.Client.IConnection
{

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClientImpl _client;
    private ConnectionEventsListenerImpl _connectionEventsListener;
    internal bool _disposed;

    /// <inheritdoc/>
    public Nabto.Edge.Client.IConnection.ConnectionEventHandler? ConnectionEventHandlers { get; set; }

    private static void AssertClientIsAlive(NabtoClientImpl client)
    {
        if (client._disposed)
        {
            throw new ObjectDisposedException("NabtoClient", "The NabtoClient instance associated with this Connection instance has been disposed.");
        }
    }

    private void AssertClientIsAlive()
    {
        AssertClientIsAlive(_client);
    }

    private void AssertSelfIsAlive()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException("Connection", "This Connection instance has been disposed.");
        }
    }

    internal static ConnectionImpl Create(Nabto.Edge.Client.Impl.NabtoClientImpl client)
    {
        AssertClientIsAlive(client);
        IntPtr ptr = NabtoClientNative.nabto_client_connection_new(client.GetHandle());
        if (ptr == IntPtr.Zero)
        {
            throw new AllocationException("Could not allocate Connection");
        }
        return new ConnectionImpl(client, ptr);
    }

    /// <inheritdoc />
    public ConnectionImpl(Nabto.Edge.Client.Impl.NabtoClientImpl client, IntPtr handle)
    {
        _client = client;
        _handle = handle;
        _connectionEventsListener = new ConnectionEventsListenerImpl(this, client);
    }

    /// <inheritdoc />
    public void DispatchConnectionEvent(Nabto.Edge.Client.IConnection.ConnectionEvent e)
    {
        ConnectionEventHandlers?.Invoke(e);
    }


    /// <inheritdoc />
    public void SetOptions(string json)
    {
        AssertSelfIsAlive();
        int ec = NabtoClientNative.nabto_client_connection_set_options(_handle, json);
        if (ec != 0)
        {
            throw NabtoExceptionFactory.Create(ec);
        }
    }

    /// <inheritdoc />
    public void SetOptions(ConnectionOptions options)
    {
        var serializerOptions = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        string jsonString = JsonSerializer.Serialize(options, serializerOptions);
        SetOptions(jsonString);
    }

    /// <inheritdoc />
    public string GetOptionsAsJson()
    {
        AssertSelfIsAlive();
        string options;
        int ec = NabtoClientNative.nabto_client_connection_get_options(_handle, out options);
        if (ec != 0)
        {
            throw NabtoExceptionFactory.Create(ec);
        }
        return options;
    }

    /// <inheritdoc />
    public ConnectionOptions GetOptions()
    {
        AssertSelfIsAlive();
        string json = GetOptionsAsJson();
        try
        {
            var options = JsonSerializer.Deserialize<ConnectionOptions>(json);
            if (options == null)
            {
                throw NabtoExceptionFactory.Create(NabtoClientError.PARSE);
            }
            return options;
        }
        catch (JsonException)
        {
            throw NabtoExceptionFactory.Create(NabtoClientError.PARSE);
        }
    }


    /// <inheritdoc />
    public string GetDeviceFingerprint()
    {
        AssertSelfIsAlive();
        string fingerprint;
        int ec = NabtoClientNative.nabto_client_connection_get_device_fingerprint(_handle, out fingerprint);
        if (ec != 0)
        {
            throw NabtoExceptionFactory.Create(ec);
        }
        return fingerprint;
    }


    /// <inheritdoc />
    public string GetClientFingerprint()
    {
        AssertSelfIsAlive();
        string fingerprint;
        int ec = NabtoClientNative.nabto_client_connection_get_client_fingerprint(_handle, out fingerprint);
        if (ec != 0)
        {
            throw NabtoExceptionFactory.Create(ec);
        }
        return fingerprint;
    }


    /// <inheritdoc />
    public async Task ConnectAsync()
    {
        AssertSelfIsAlive();

        TaskCompletionSource connectTask = new TaskCompletionSource();
        var task = connectTask.Task;

        var future = FutureImpl.Create(_client);

        NabtoClientNative.nabto_client_connection_connect(_handle, future.GetHandle());

        var ec = await future.WaitAsync();
        if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value())
        {
            return;
        }
        else
        {
            throw NabtoExceptionFactory.Create(ec);
        }
    }


    /// <inheritdoc />
    public async Task CloseAsync()
    {
        AssertSelfIsAlive();

        TaskCompletionSource closeTask = new TaskCompletionSource();
        var task = closeTask.Task;

        var future = FutureImpl.Create(_client);

        NabtoClientNative.nabto_client_connection_close(_handle, future.GetHandle());

        var ec = await future.WaitAsync();
        if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value())
        {
            return;
        }
        else
        {
            throw NabtoExceptionFactory.Create(ec);
        }
    }


    /// <inheritdoc/>
    public void Stop() {
        NabtoClientNative.nabto_client_connection_stop(GetHandle());
    }

    /// <inheritdoc />
    public async Task PasswordAuthenticateAsync(string username, string password)
    {
        AssertSelfIsAlive();

        TaskCompletionSource passwordAuthenticateTask = new TaskCompletionSource();
        var task = passwordAuthenticateTask.Task;

        var future = FutureImpl.Create(_client);

        NabtoClientNative.nabto_client_connection_password_authenticate(_handle, username, password, future.GetHandle());

        var ec = await future.WaitAsync();
        if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value())
        {
            return;
        }
        else
        {
            throw NabtoExceptionFactory.Create(ec);
        }
    }

    /// <inheritdoc />
    public int GetLocalChannelErrorCode()
    {
        return NabtoClientNative.nabto_client_connection_get_local_channel_error_code(GetHandle());
    }

    /// <inheritdoc />
    public int GetRemoteChannelErrorCode()
    {
        return NabtoClientNative.nabto_client_connection_get_remote_channel_error_code(GetHandle());
    }

    /// <inheritdoc />
    public int GetDirectCandidatesChannelErrorCode()
    {
        return NabtoClientNative.nabto_client_connection_get_direct_candidates_channel_error_code(GetHandle());
    }


    /// <inheritdoc />
    public Nabto.Edge.Client.ICoapRequest CreateCoapRequest(string method, string path)
    {
        return CoapRequestImpl.Create(_client, this, method, path);
    }

    /// <inheritdoc />
    public Nabto.Edge.Client.IStream CreateStream()
    {
        return StreamImpl.Create(_client, this);
    }

    /// <inheritdoc />
    public Nabto.Edge.Client.ITcpTunnel CreateTcpTunnel()
    {
        return TcpTunnelImpl.Create(_client, this);
    }

    /// <inheritdoc />
    public IConnection.ConnectionType GetConnectionType()
    {
        int connectionType;
        int ec = NabtoClientNative.nabto_client_connection_get_type(GetHandle(), out connectionType);
        if (ec != NabtoClientNative.NABTO_CLIENT_EC_OK_value())
        {
            throw NabtoExceptionFactory.Create(ec);
        }
        switch ((NabtoClientNative.NabtoClientConnectionType)connectionType)
        {
            case NabtoClientNative.NabtoClientConnectionType.Direct: return IConnection.ConnectionType.Direct;
            case NabtoClientNative.NabtoClientConnectionType.Relay: return IConnection.ConnectionType.Relay;
            default: throw NabtoExceptionFactory.Create(NabtoClientNative.NABTO_CLIENT_EC_UNKNOWN_value());
        }
    }

    /// <inheritdoc />
    public IntPtr GetHandle()
    {
        AssertSelfIsAlive();
        return _handle;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    ~ConnectionImpl()
    {
        Dispose(false);
    }

    /// <summary>Do the actual resource disposal here</summary>
    protected void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _connectionEventsListener.Stop();
                _connectionEventsListener.Dispose();
            }
            Stop();
            NabtoClientNative.nabto_client_connection_free(_handle);
            _disposed = true;
        }
    }

}

/**
 * <summary>This is used to hold a connection event such that we can pin the object, in turn such
 * that garbage collection does not change the address of the ConnectionEvent.</summary>
 */
internal class ConnectionEventHolder
{
    internal int ConnectionEvent;
}
