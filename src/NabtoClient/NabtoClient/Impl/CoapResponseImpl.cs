namespace Nabto.Edge.Client.Impl;

public class CoapResponseImpl : Nabto.Edge.Client.CoapResponse
{
    private CoapRequestImpl _request;

    public CoapResponseImpl(CoapRequestImpl request)
    {
        _request = request;
    }

    private void AssertRequestIsAlive() {
        if (_request._disposedUnmanaged) {
            throw new ObjectDisposedException("CoapRequest", "The CoapRequest used for generating this CoapResponse has been disposed.");
        }
    }

    public ushort GetResponseStatusCode()
    {
        AssertRequestIsAlive();
        ushort statusCode = 0;
        int ec;
        ec = NabtoClientNative.nabto_client_coap_get_response_status_code(_request.GetHandle(), out statusCode);
        if (ec != 0)
        {
            throw NabtoExceptionFactory.Create(ec);
        }
        return statusCode;
    }

    public ushort GetResponseContentFormat()
    {
        AssertRequestIsAlive();
        ushort contentFormat = 0;
        int ec = NabtoClientNative.nabto_client_coap_get_response_content_format(_request.GetHandle(), out contentFormat);
        if (ec != 0)
        {
            throw NabtoExceptionFactory.Create(ec);
        }
        return contentFormat;
    }

    public byte[] GetResponsePayload()
    {
        AssertRequestIsAlive();
        byte[] payload;
        int ec = NabtoClientNative.nabto_client_coap_get_response_payload(_request.GetHandle(), out payload);
        if (ec != 0)
        {
            throw NabtoExceptionFactory.Create(ec);
        }
        return payload;
    }
}
