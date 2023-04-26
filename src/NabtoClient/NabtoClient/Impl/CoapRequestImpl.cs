namespace Nabto.Edge.Client.Impl;

public class CoapRequestImpl : Nabto.Edge.Client.CoapRequest
{

    private IntPtr _handle;
    private NabtoClientImpl _client;

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

    ~CoapRequestImpl()
    {
        NabtoClientNative.nabto_client_coap_free(_handle);
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
