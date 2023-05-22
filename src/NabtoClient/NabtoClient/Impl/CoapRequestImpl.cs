namespace Nabto.Edge.Client.Impl;

/// <inheritdoc />
public class CoapRequestImpl : Nabto.Edge.Client.CoapRequest
{

    private IntPtr _handle;
    private NabtoClientImpl _client;
    internal bool _disposedUnmanaged;

    private void AssertClientIsAlive() {
        if (_client._disposedUnmanaged) {
            throw new ObjectDisposedException("NabtoClient", "The NabtoClient instance associated with this CoapRequest instance has been disposed.");
        }
    }

    private static void AssertConnectionIsAlive(ConnectionImpl connection) {
        if (connection._disposedUnmanaged) {
            throw new ObjectDisposedException("Connection", "The Connection instance associated with this CoapRequest instance has been disposed.");
        }
    }

    private void AssertSelfIsAlive() {
        if (_disposedUnmanaged) {
            throw new ObjectDisposedException("CoapRequest", "This CoapRequest has been disposed.");
        }
    }

    internal static CoapRequestImpl Create(Nabto.Edge.Client.Impl.NabtoClientImpl client, Nabto.Edge.Client.Impl.ConnectionImpl connection, string method, string path)
    {
        AssertConnectionIsAlive(connection);
        IntPtr handle = NabtoClientNative.nabto_client_coap_new(connection.GetHandle(), method, path);
        if (handle == IntPtr.Zero)
        {
            throw new NullReferenceException();
        }
        return new CoapRequestImpl(client, handle);
    }

    internal CoapRequestImpl(NabtoClientImpl client, IntPtr handle)
    {
        _handle = handle;
        _client = client;
    }

    internal IntPtr GetHandle()
    {
        return _handle;
    }

    /// <inheritdoc />
    public void SetRequestPayload(ushort contentFormat, byte[] data)
    {
        AssertSelfIsAlive();
        int ec = NabtoClientNative.nabto_client_coap_set_request_payload(_handle, contentFormat, data);
    }

    /// <inheritdoc />
    public async Task<Nabto.Edge.Client.CoapResponse> ExecuteAsync()
    {
        AssertClientIsAlive();
        AssertSelfIsAlive();

        TaskCompletionSource<Nabto.Edge.Client.CoapResponse> executeTask = new TaskCompletionSource<Nabto.Edge.Client.CoapResponse>();
        var task = executeTask.Task;
        await using (FutureImpl future = FutureImpl.Create(_client)) {

            NabtoClientNative.nabto_client_coap_execute(_handle, future.GetHandle());

            var ec = await future.WaitAsync();

            if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value())
            {
                return new CoapResponseImpl(this);
            }
            else
            {
                throw NabtoExceptionFactory.Create(ec);
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Console.WriteLine("*** CoapRequestImpl Dispose called");
        DisposeUnmanaged();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        Console.WriteLine("*** CoapRequestImpl DisposeAsync called");
        DisposeUnmanaged();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    ~CoapRequestImpl()
    {
        Console.WriteLine("*** CoapRequestImpl finalizer called");
        DisposeUnmanaged();
    }

    private void DisposeUnmanaged() {
        if (!_disposedUnmanaged) {
            NabtoClientNative.nabto_client_coap_free(_handle);
        }
        _disposedUnmanaged = true;
    }

}
