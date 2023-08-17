namespace Nabto.Edge.Client.Impl;

/// <inheritdoc />
public class CoapResponseImpl : Nabto.Edge.Client.ICoapResponse
{
    private CoapRequestImpl _request;

    internal CoapResponseImpl(CoapRequestImpl request)
    {
        _request = request;
    }

    private void AssertRequestIsAlive()
    {
        if (_request._disposed)
        {
            throw new ObjectDisposedException("CoapRequest", "The CoapRequest used for generating this CoapResponse has been disposed.");
        }
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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
