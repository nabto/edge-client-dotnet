namespace Nabto.Edge.Client;

using System.Text;

public class IamException : Exception
{
    public IamError Error { get; }

    public string? ErrorMessage { get; set; }

    private static string GetMessage(IamError e, CoapResponse r)
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
            } catch (NabtoException ne) {
            }
        }

        return e.ToString();

    }

    public IamException(IamError e) 
        : base(e.ToString())
    {
        Error = e;
    }

    public IamException(IamError e, CoapResponse r)
        : base(GetMessage(e, r))
    {
        Error = e;
    }

    public IamException(IamError e, CoapResponse r, ushort statusCode)
        : base($"StatusCode {statusCode} is unhandled by the code. " + GetMessage(e,r) )
    {
        Error = e;
    }

    public static void HandleDefaultCoap(CoapResponse r) {
        ushort statusCode = r.GetResponseStatusCode();
        if (statusCode >= 200 && statusCode < 300) {
            return;
        }
        switch (statusCode) { 
            case 400: throw new IamException(IamError.INVALID_INPUT, r);
            case 401: throw new IamException(IamError.AUTHENTICATION_ERROR, r);
            case 403: throw new IamException(IamError. AUTHENTICATION_ERROR, r);
            default: throw new IamException(IamError.FAILED, r, statusCode);
        }
    }
};