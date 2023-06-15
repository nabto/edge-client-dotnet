namespace Nabto.Edge.ClientIam.Impl;

using PeterO.Cbor;
using Nabto.Edge.Client;

public class User
{

    public static async Task CreateUserAsync(Nabto.Edge.Client.Connection connection, IamUser user)
    {
        if (user.Username == null)
        {
            throw new ArgumentException("Missing required Username.");
        }

        await CreateUserAsync(connection, user.Username);
        if (user.DisplayName != null)
        {
            await UserSettings.UpdateUserDisplayNameAsync(connection, user.Username, user.DisplayName);
        }

        if (user.Fcm != null)
        {
            if (user.Fcm.ProjectId == null || user.Fcm.Token == null)
            {
                throw new ArgumentException("If Fcm is set ProjectId and Token is required to be nonnull.");
            }
            await UserSettings.UpdateUserFcmAsync(connection, user.Username, user.Fcm.ProjectId, user.Fcm.Token);
        }

        if (user.NotificationCategories != null)
        {
            await UserSettings.UpdateUserNotificationCategoriesAsync(connection, user.Username, user.NotificationCategories);
        }

        if (user.Sct != null)
        {
            await UserSettings.UpdateUserSctAsync(connection, user.Username, user.Sct);
        }

        if (user.Password != null)
        {
            await UserSettings.UpdateUserPasswordAsync(connection, user.Username, user.Password);
        }

        if (user.Role != null)
        {
            await UserSettings.UpdateUserRoleAsync(connection, user.Username, user.Role);
        }

        if (user.Fingerprint != null)
        {
            await UserSettings.UpdateUserFingerprintAsync(connection, user.Username, user.Fingerprint);
        }
    }

    public static async Task CreateUserAsync(Nabto.Edge.Client.Connection connection, string username)
    {
        var cbor = CBORObject.NewMap().Add("Username", username);

        var req = connection.CreateCoapRequest("POST", "/iam/users");
        req.SetRequestPayload(CoapContentFormat.APPLICATION_CBOR, cbor.EncodeToBytes());

        var response = await req.ExecuteAsync();

        var statusCode = response.GetResponseStatusCode();

        switch (statusCode)
        {
            case 409: throw IamExceptionImpl.Create(IamError.USERNAME_EXISTS, response);
            default: IamExceptionImpl.HandleDefaultCoap(response); break;
        }


    }

    public static async Task DeleteUserAsync(Nabto.Edge.Client.Connection connection, string username)
    {
        var req = connection.CreateCoapRequest("DELETE", $"/iam/users/{username}");
        var response = await req.ExecuteAsync();

        var statusCode = response.GetResponseStatusCode();

        switch (statusCode)
        {
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

        if (role != null)
        {
            user.Role = role.AsString();
        }

        if (sct != null)
        {
            user.Sct = sct.AsString();
        }

        if (fingerprint != null)
        {
            user.Fingerprint = fingerprint.AsString();
        }

        if (displayName != null)
        {
            user.DisplayName = displayName.AsString();
        }

        if (notificationCategories != null)
        {
            List<string> categories = new List<string>();
            foreach (CBORObject i in notificationCategories.Values)
            {
                categories.Add(i.AsString());
            }
            user.NotificationCategories = categories;
        }

        if (fcm != null)
        {
            var projectId = fcm["ProjectId"];
            var token = fcm["Token"];

            user.Fcm = new Fcm();

            if (projectId != null)
            {
                user.Fcm.ProjectId = projectId.AsString();
            }

            if (token != null)
            {
                user.Fcm.Token = token.AsString();
            }
        }

        return user;
    }

    public static async Task<IamUser> GetUserAsync(Nabto.Edge.Client.CoapRequest coapRequest)
    {
        var response = await coapRequest.ExecuteAsync();
        var statusCode = response.GetResponseStatusCode();

        if (statusCode == 404)
        {
            throw new IamException(IamError.USER_DOES_NOT_EXIST);
        }
        var data = IamExceptionImpl.HandleDefaultCoapCborPayload(response);

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
