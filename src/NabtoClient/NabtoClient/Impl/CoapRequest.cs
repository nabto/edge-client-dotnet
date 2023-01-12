namespace Nabto.Edge.Client.Impl;

public class CoapRequest : Nabto.Edge.Client.CoapRequest {

    private IntPtr _handle;
    private NabtoClient _client;

    public static CoapRequest Create(Nabto.Edge.Client.Impl.NabtoClient client, Nabto.Edge.Client.Impl.Connection connection, string method, string path)
    {
        IntPtr handle = NabtoClientNative.nabto_client_coap_new(connection.GetHandle(), method, path);
        if (handle == IntPtr.Zero) {
            throw new NullReferenceException();
        }
        return new CoapRequest(client, handle);
    }

    public CoapRequest(NabtoClient client, IntPtr handle)
    {
        _handle = handle;
        _client = client;
    }

    public IntPtr GetHandle() {
        return _handle;
    }

    public void SetRequestPayload(ushort contentFormat, byte[] data)
    {
        int ec = NabtoClientNative.CoapSetRequestPayload(_handle, contentFormat, data);
    }

    public Task<Nabto.Edge.Client.CoapResponse> ExecuteAsync() {
        TaskCompletionSource<Nabto.Edge.Client.CoapResponse> connectTask = new TaskCompletionSource<Nabto.Edge.Client.CoapResponse>();
        var task = connectTask.Task;
        Future future = Future.Create(_client);

        NabtoClientNative.nabto_client_coap_execute(_handle, future.GetHandle());

        future.Wait((ec) => {
            if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value()) {

                connectTask.SetResult(new CoapResponse(this));
            } else {
                connectTask.SetException(NabtoException.Create(ec));
            }
        });
        return task;
    }
}
