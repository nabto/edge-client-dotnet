namespace Nabto.Edge.Client.Impl;

using System.Buffers;
using System.Collections;
using Nabto.Edge.Client;

using PeterO.Cbor;

public class IamUtilImpl {
    private class UsernameContainer { 
        public string? Username { get; set; }
    }

    private static IamUser IamUserFromCBOR(CBORObject o)
    {
        var username = o["Username"].AsString();

        var user = new IamUser { Username = username };

        var role = o["Role"];
        var sct = o["Sct"];
        var fingerprint = o["Fingerprint"];
        var displayName = o["DisplayName"];
        var notificationCategories = o["NotificationCategories"];

        if (role != null) {
            user.Role = role.AsString();
        }

        if (sct != null) {
            user.Sct = sct.AsString();
        }

        if (fingerprint != null) {
            user.Fingerprint = fingerprint.AsString();
        }

        if (displayName != null) {
            user.DisplayName = displayName.AsString();
        }

        if (notificationCategories != null) { 
            List<string> categories = new List<string>();
            foreach (CBORObject i in notificationCategories.Values)
            {
                categories.Add(i.AsString());
            }
            user.NotificationCategories = categories;
        }

        return user;
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
            throw new IamException(IamError.CANNOT_PARSE_RESPONSE);
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
            throw new IamException(IamError.CANNOT_PARSE_RESPONSE);
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
            throw new IamException(IamError.CANNOT_PARSE_RESPONSE);
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