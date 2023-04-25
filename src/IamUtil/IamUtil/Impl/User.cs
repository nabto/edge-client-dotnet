namespace Nabto.Edge.ClientIam.Impl;

using PeterO.Cbor;
using Nabto.Edge.Client;

public class User {
    public static async Task CreateUserAsync(Nabto.Edge.Client.Connection connection, string username)
    {
        var cbor = CBORObject.NewMap().Add("Username", username);

        var req = connection.CreateCoapRequest("POST", "/iam/users");
        req.SetRequestPayload(CoapContentFormat.APPLICATION_CBOR, cbor.EncodeToBytes());

        var response = await req.ExecuteAsync();

        var statusCode = response.GetResponseStatusCode();

        switch (statusCode) {
            case 409: throw IamExceptionImpl.Create(IamError.USERNAME_EXISTS, response);
            default: IamExceptionImpl.HandleDefaultCoap(response); break;
        }


    }

    public static async Task DeleteUserAsync(Nabto.Edge.Client.Connection connection, string username)
    {
        var req = connection.CreateCoapRequest("DELETE", $"/iam/users/{username}");
        var response = await req.ExecuteAsync();

        var statusCode = response.GetResponseStatusCode();

        switch (statusCode) {
            case 404: throw IamExceptionImpl.Create(IamError.USER_DOES_NOT_EXIST, response);
            default: IamExceptionImpl.HandleDefaultCoap(response); break;
        }
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
        var fcm = o["Fcm"];

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

        if (fcm != null) {
            var projectId = fcm["ProjectId"];
            var token = fcm["Token"];

            user.Fcm = new Fcm();

            if (projectId != null) {
                user.Fcm.ProjectId = projectId.AsString();
            }

            if (token != null) {
                user.Fcm.Token = token.AsString();
            }
        }

        return user;
    }

    public static async Task<IamUser> GetUserAsync(Nabto.Edge.Client.CoapRequest coapRequest)
    {
        var response = await coapRequest.ExecuteAsync();
        var statusCode = response.GetResponseStatusCode();

        if (statusCode == 404) {
            throw new IamException(IamError.USER_DOES_NOT_EXIST);
        }
        IamExceptionImpl.HandleDefaultCoap(response);

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

}
