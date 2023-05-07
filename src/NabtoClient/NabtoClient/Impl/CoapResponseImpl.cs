namespace Nabto.Edge.Client.Impl;

/// <inheritdoc />
public class CoapResponseImpl : Nabto.Edge.Client.CoapResponse
{
    private CoapRequestImpl _request;

    internal CoapResponseImpl(CoapRequestImpl request)
    {
        _request = request;
    }

    /// <inheritdoc/>
    public ushort GetResponseStatusCode()
    {
        ushort statusCode = 0;
        //ushort statusCode = 0;
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
        byte[] payload;
        int ec = NabtoClientNative.nabto_client_coap_get_response_payload(_request.GetHandle(), out payload);
        if (ec != 0)
        {
            throw NabtoExceptionFactory.Create(ec);
        }
        return payload;
    }
}
