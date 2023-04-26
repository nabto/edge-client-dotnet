namespace Nabto.Edge.ClientIam.Impl;

using Nabto.Edge.Client;
using System.Text;

public class IamExceptionImpl
{
    private static string GetMessage(IamError e, Nabto.Edge.Client.CoapResponse r)
    {
        ushort? contentFormat = null;
        try
        {
            contentFormat = r.GetResponseContentFormat();
        } catch (NabtoException ne) {
            if (ne.ErrorCode == NabtoClientError.NO_DATA)
            {
                // no content format, it is still possible that there is an UTF8 text encoded error message in the response
                contentFormat = CoapContentFormat.TEXT_PLAIN;
            }
        }

        if (contentFormat.HasValue && contentFormat == CoapContentFormat.TEXT_PLAIN) {
            try
            {
                var payload = r.GetResponsePayload();
                var s = Encoding.Default.GetString(payload);
                return e.ToString() + ". Details: " + s + ".";
            } catch {
            }
        }

        return e.ToString();

    }

    public static IamException Create(IamError e)
    {
        return new IamException(e.ToString(), e);
    }

    public static IamException Create(IamError e, Nabto.Edge.Client.CoapResponse r)
    {
        return new IamException(GetMessage(e, r), e);
    }

    public static IamException Create(IamError e, Nabto.Edge.Client.CoapResponse r, ushort statusCode)
    {
        return new IamException($"StatusCode {statusCode} is unhandled by the code. " + GetMessage(e, r), e);
    }

    public static void HandleDefaultCoap(Nabto.Edge.Client.CoapResponse r) {
        ushort statusCode = r.GetResponseStatusCode();
        if (statusCode >= 200 && statusCode < 300) {
            return;
        }
        switch (statusCode) {
            case 400: throw Create(IamError.INVALID_INPUT, r);
            case 401: throw Create(IamError.AUTHENTICATION_ERROR, r);
            case 403: throw Create(IamError.FORBIDDEN, r);
            default: throw Create(IamError.FAILED, r, statusCode);
        }
    }

    public static byte[] HandleDefaultCoapCborPayload(Nabto.Edge.Client.CoapResponse r)
    {
        HandleDefaultCoap(r);
        try {
            ushort contentFormat = r.GetResponseContentFormat();
            if (contentFormat != CoapContentFormat.APPLICATION_CBOR) {
                throw new IamException("The content format is not application/cbor", IamError.CANNOT_PARSE_RESPONSE);
            }
        } catch (NabtoException e) {
            if (e.ErrorCode == NabtoClientError.NO_DATA) {
                throw new IamException("The coap response does not contain a content format", IamError.CANNOT_PARSE_RESPONSE);
            }
            throw;
        }

        try {
            var data = r.GetResponsePayload();
            return data;
        } catch (NabtoException e) {
            if (e.ErrorCode == NabtoClientError.NO_DATA) {
                throw new IamException("The coap response is missing expected response data", IamError.CANNOT_PARSE_RESPONSE);
            }
            throw;
        }
    }
};
