namespace Nabto.Edge.Client.Impl;

using System.Buffers;
using System.Collections;
using Nabto.Edge.Client;

using PeterO.Cbor;

public class IamUtilImpl {
    private class UsernameContainer { 
        public string? Username { get; set; }
    }

    public static async Task PairLocalOpenAsync(Nabto.Edge.Client.Connection connection, string desiredUsername) {
        var coapRequest = connection.CreateCoapRequest("POST", "/iam/pairing/local-open");

        var cbor = CBORObject.NewMap().Add("Username", desiredUsername);

        coapRequest.SetRequestPayload((ushort)CoapContentFormat.APPLICATION_CBOR, cbor.EncodeToBytes());

        var response = await coapRequest.ExecuteAsync();

        var statusCode = response.GetResponseStatusCode();


        return;
    }

    private static async Task<IamError> CoapPairPasswordOpenAsync(Nabto.Edge.Client.Connection connection, string desiredUsername)
    { 
        var coapRequest = connection.CreateCoapRequest("POST", "/iam/pairing/password-open");

        var cbor = CBORObject.NewMap().Add("Username", desiredUsername);

        coapRequest.SetRequestPayload((ushort)CoapContentFormat.APPLICATION_CBOR, cbor.EncodeToBytes());

        var response = await coapRequest.ExecuteAsync();
        var statusCode = response.GetResponseStatusCode();
        switch (statusCode) { 
            case 201: return IamError.NONE;
            case 400: return IamError.INVALID_INPUT;
            case 403: return IamError.BLOCKED_BY_DEVICE_CONFIGURATION;
            case 404: return IamError.PAIRING_MODE_DISABLED;
            case 409: return IamError.USERNAME_EXISTS;
            default: return IamError.FAILED;
        }
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

        var iamError = await CoapPairPasswordOpenAsync(connection, desiredUsername);

        if (iamError != IamError.NONE) {
            throw new IamException(iamError);
        }
    }

    private static IamUser IamUserFromCBOR(CBORObject o)
    {
        var username = o["Username"].AsString();
        var role = o["Role"].AsString();
        var sct = o["Sct"].AsString();
        var fingerprint = o["Fingerprint"].AsString();
        var notificationCategories = o["NotificationCategories"];
        string? displayName = null;
        if (o["DisplayName"] != null) {
            displayName = o["DisplayName"].AsString();
        }
        

        List<string> categories = new List<string>();
        foreach (CBORObject i in notificationCategories.Values) {
            categories.Add(i.AsString());
        }

        return new IamUser { Username = username, Role = role, Sct = sct, Fingerprint = fingerprint, NotificationCategories = categories, DisplayName = displayName };
    }

    public static async Task<IamUser> GetUserAsync(Nabto.Edge.Client.CoapRequest coapRequest)
    { 
        var response = await coapRequest.ExecuteAsync();
        var statusCode = response.GetResponseStatusCode();
        switch (statusCode) { 
            case 205: break;
            case 400: throw new IamException(IamError.INVALID_INPUT);
            case 403: throw new IamException(IamError.BLOCKED_BY_DEVICE_CONFIGURATION);
            case 404: throw new IamException(IamError.USER_DOES_NOT_EXIST);
            default: throw new IamException(IamError.FAILED);
        }

        var responseFormat = response.GetResponseContentFormat();
        if (responseFormat != (ushort)CoapContentFormat.APPLICATION_CBOR) {
            // TODO throw a parse error or data format error
            throw new IamException(IamError.FAILED);
        }
        var data = response.GetResponsePayload();
        
        CBORObject cborObject = CBORObject.DecodeFromBytes(data);

        IamUser iamUser = IamUserFromCBOR(cborObject);
        return iamUser;
    }

    public static async Task<IamUser> GetUserAsync(Nabto.Edge.Client.Connection connection, string username)
    { 
        var coapRequest = connection.CreateCoapRequest("GET", $"/iam/users/{username}");
        return await GetUserAsync(coapRequest);
    }


    public static async Task<IamUser> GetCurrentUserAsync(Nabto.Edge.Client.Connection connection)
    { 
        var coapRequest = connection.CreateCoapRequest("GET", "/iam/me");
        return await GetUserAsync(coapRequest);
    }

    public static async Task UpdateUserNotificationCategoriesAsync(Nabto.Edge.Client.Connection connection, string username, List<string> categories)
    {
        var cbor = CBORObject.NewArray();

        foreach (var c in categories) {
            cbor.Add(c);
        }

        await UpdateUserSettingAsync(connection, username, "notification-categories", cbor);
    }

    public static async Task UpdateUserSettingAsync(Nabto.Edge.Client.Connection connection, string username, string coapParameterPath, CBORObject value)
    { 
        var coapRequest = connection.CreateCoapRequest("PUT", $"/iam/users/{username}/{coapParameterPath}");

        var data = value.EncodeToBytes();
        coapRequest.SetRequestPayload(CoapContentFormat.APPLICATION_CBOR, data);

        var response = await coapRequest.ExecuteAsync();
        
        var statusCode = response.GetResponseStatusCode();
        switch (statusCode) { 
            case 204: break;
            case 400: throw new IamException(IamError.INVALID_INPUT);
            case 403: throw new IamException(IamError.BLOCKED_BY_DEVICE_CONFIGURATION);
            case 404: throw new IamException(IamError.USER_DOES_NOT_EXIST);
            default: throw new IamException(IamError.FAILED);
        }
    }


    public static async Task UpdateUserDisplayNameAsync(Nabto.Edge.Client.Connection connection, string username, string displayName)
    {
        await UpdateUserSettingAsync(connection, username, "display-name", CBORObject.FromObject(displayName));
    }

    public static async Task<List<string>> ListRolesAsync(Nabto.Edge.Client.Connection connection)
    { 
        var coapRequest = connection.CreateCoapRequest("GET", "/iam/roles");
        var response = await coapRequest.ExecuteAsync();
        
        var statusCode = response.GetResponseStatusCode();
        switch (statusCode) { 
            case 205: break;
            case 400: throw new IamException(IamError.INVALID_INPUT);
            case 403: throw new IamException(IamError.BLOCKED_BY_DEVICE_CONFIGURATION);
            default: throw new IamException(IamError.FAILED);
        }

        var contentFormat = response.GetResponseContentFormat();
        if (contentFormat != (ushort)CoapContentFormat.APPLICATION_CBOR) {
            throw new IamException(IamError.FAILED);
        }

        var data = response.GetResponsePayload();

        CBORObject cborObject = CBORObject.DecodeFromBytes(data);
        List<string> roles = new List<string>();
        foreach (CBORObject i in cborObject.Values) {
            roles.Add(i.AsString());
        }

        return roles;

    }

    public static async Task<List<string>> ListNotificationCategoriesAsync(Nabto.Edge.Client.Connection connection)
    { 
        var coapRequest = connection.CreateCoapRequest("GET", "/iam/notification-categories");
        var response = await coapRequest.ExecuteAsync();
        
        var statusCode = response.GetResponseStatusCode();
        switch (statusCode) { 
            case 205: break;
            case 400: throw new IamException(IamError.INVALID_INPUT);
            case 403: throw new IamException(IamError.BLOCKED_BY_DEVICE_CONFIGURATION);
            default: throw new IamException(IamError.FAILED);
        }

        var contentFormat = response.GetResponseContentFormat();
        if (contentFormat != (ushort)CoapContentFormat.APPLICATION_CBOR) {
            throw new IamException(IamError.FAILED);
        }

        var data = response.GetResponsePayload();

        CBORObject cborObject = CBORObject.DecodeFromBytes(data);
        List<string> roles = new List<string>();
        foreach (CBORObject i in cborObject.Values) {
            roles.Add(i.AsString());
        }

        return roles;

    }
}