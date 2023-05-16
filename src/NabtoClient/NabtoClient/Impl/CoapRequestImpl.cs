namespace Nabto.Edge.Client.Impl;

public class CoapRequestImpl : Nabto.Edge.Client.CoapRequest
{

    private IntPtr _handle;
    private NabtoClientImpl _client;
    private bool _disposedUnmanaged;

    public static CoapRequestImpl Create(Nabto.Edge.Client.Impl.NabtoClientImpl client, Nabto.Edge.Client.Impl.ConnectionImpl connection, string method, string path)
    {
        IntPtr handle = NabtoClientNative.nabto_client_coap_new(connection.GetHandle(), method, path);
        if (handle == IntPtr.Zero)
        {
            throw new NullReferenceException();
        }
        return new CoapRequestImpl(client, handle);
    }

    public CoapRequestImpl(NabtoClientImpl client, IntPtr handle)
    {
        _handle = handle;
        _client = client;
    }

    public IntPtr GetHandle()
    {
        return _handle;
    }

    public void SetRequestPayload(ushort contentFormat, byte[] data)
    {
        int ec = NabtoClientNative.nabto_client_coap_set_request_payload(_handle, contentFormat, data);
    }

    public async Task<Nabto.Edge.Client.CoapResponse> ExecuteAsync()
    {
        TaskCompletionSource<Nabto.Edge.Client.CoapResponse> executeTask = new TaskCompletionSource<Nabto.Edge.Client.CoapResponse>();
        var task = executeTask.Task;
        FutureImpl future = FutureImpl.Create(_client);

        NabtoClientNative.nabto_client_coap_execute(_handle, future.GetHandle());

        var ec = await future.WaitAsync();

        Console.WriteLine(" *** CoapRequestImpl.ExecuteAsync() done");

        if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value())
        {

            return new CoapResponseImpl(this);
        }
        else
        {
            throw NabtoExceptionFactory.Create(ec);
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
