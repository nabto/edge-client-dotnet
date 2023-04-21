namespace Nabto.Edge.Client.Impl;

using PeterO.Cbor;

public class Pairing { 
    private static void HandlePairingResponse(Nabto.Edge.Client.CoapResponse response) 
    { 
        var statusCode = response.GetResponseStatusCode();

        switch (statusCode) { 
            case 201: break;
            case 400: throw new IamException(IamError.INVALID_INPUT, response);
            case 403: throw new IamException(IamError.BLOCKED_BY_DEVICE_CONFIGURATION, response);
            case 404: throw new IamException(IamError.PAIRING_MODE_DISABLED, response);
            case 409: throw new IamException(IamError.USERNAME_EXISTS, response);
            default: throw new IamException(IamError.FAILED, response, statusCode);
        }
    }

    public static async Task PairLocalInitialAsync(Nabto.Edge.Client.Connection connection)
    {
        var coapRequest = connection.CreateCoapRequest("POST", "/iam/pairing/local-initial");
        var response = await coapRequest.ExecuteAsync();

        HandlePairingResponse(response);
    }

    public static async Task PairLocalOpenAsync(Nabto.Edge.Client.Connection connection, string desiredUsername) {
        var coapRequest = connection.CreateCoapRequest("POST", "/iam/pairing/local-open");

        var cbor = CBORObject.NewMap().Add("Username", desiredUsername);

        coapRequest.SetRequestPayload((ushort)CoapContentFormat.APPLICATION_CBOR, cbor.EncodeToBytes());

        var response = await coapRequest.ExecuteAsync();

        HandlePairingResponse(response);
    }

    public static async Task PairInvitePasswordAsync(Nabto.Edge.Client.Connection connection, string username, string password)
    { 
        try
        {
            await connection.PasswordAuthenticate(username, password);
        } catch (NabtoException e) {
            if (e.ErrorCode == NabtoClientError.UNAUTHORIZED) {
                throw new IamException(IamError.AUTHENTICATION_ERROR);
            }
            throw;
        }

        var coapRequest = connection.CreateCoapRequest("POST", "/iam/pairing/password-invite");
        var response = await coapRequest.ExecuteAsync();
        HandlePairingResponse(response);
    }

    public static async Task PairPasswordOpenAsync(Nabto.Edge.Client.Connection connection, string desiredUsername, string password)
    {
        try
        {
            await connection.PasswordAuthenticate("", password);
        } catch (NabtoException e) {
            if (e.ErrorCode == NabtoClientError.UNAUTHORIZED) {
                throw new IamException(IamError.AUTHENTICATION_ERROR);
            }
            throw;
        }
       
        var coapRequest = connection.CreateCoapRequest("POST", "/iam/pairing/password-open");

        var cbor = CBORObject.NewMap().Add("Username", desiredUsername);

        coapRequest.SetRequestPayload((ushort)CoapContentFormat.APPLICATION_CBOR, cbor.EncodeToBytes());

        var response = await coapRequest.ExecuteAsync();
        HandlePairingResponse(response);
    }
}