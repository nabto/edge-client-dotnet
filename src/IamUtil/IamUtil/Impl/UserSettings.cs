namespace Nabto.Edge.Client.Impl;

using PeterO.Cbor;

public class UserSettings
{
    public static async Task UpdateUserSettingAsync(Nabto.Edge.Client.Connection connection, string username, string coapParameterPath, CBORObject value, Action<ushort, Nabto.Edge.Client.CoapResponse>? errorHandler = null)
    {
        var coapRequest = connection.CreateCoapRequest("PUT", $"/iam/users/{username}/{coapParameterPath}");

        var data = value.EncodeToBytes();
        coapRequest.SetRequestPayload(CoapContentFormat.APPLICATION_CBOR, data);

        var response = await coapRequest.ExecuteAsync();

        var statusCode = response.GetResponseStatusCode();

        if (errorHandler != null) {
            errorHandler(statusCode, response);
        }

        switch (statusCode)
        {
            case 404: throw new IamException(IamError.USER_DOES_NOT_EXIST, response);
            default: IamException.HandleDefaultCoap(response); break;
        }
    }

    public static async Task UpdateUserDisplayNameAsync(Nabto.Edge.Client.Connection connection, string username, string displayName)
    {
        await UpdateUserSettingAsync(connection, username, "display-name", CBORObject.FromObject(displayName));
    }

    public static async Task UpdateUserFingerprintAsync(Nabto.Edge.Client.Connection connection, string username, string fingerprint)
    {

        await UpdateUserSettingAsync(connection, username, "fingerprint", CBORObject.FromObject(fingerprint), (statusCode, response) => { if (statusCode == 409) { throw new IamException(IamError.FINGERPRINT_IN_USE, response); } });
    }

    public static async Task UpdateUserNotificationCategoriesAsync(Nabto.Edge.Client.Connection connection, string username, List<string> categories)
    {
        var cbor = CBORObject.NewArray();

        foreach (var c in categories)
        {
            cbor.Add(c);
        }

        await UpdateUserSettingAsync(connection, username, "notification-categories", cbor);
    }

    public static async Task UpdateUserPasswordAsync(Nabto.Edge.Client.Connection connection, string username, string password)
    {
        await UpdateUserSettingAsync(connection, username, "password", CBORObject.FromObject(password));
    }

    public static async Task UpdateUserRoleAsync(Nabto.Edge.Client.Connection connection, string username, string role)
    {
        await UpdateUserSettingAsync(connection, username, "role", CBORObject.FromObject(role), (statusCode, response) => { if (statusCode == 404) { throw new IamException(IamError.USER_OR_ROLE_DOES_NOT_EXISTS, response); } });
    }
    public static async Task UpdateUserSctAsync(Nabto.Edge.Client.Connection connection, string username, string sct)
    {
        await UpdateUserSettingAsync(connection, username, "sct", CBORObject.FromObject(sct));
    }

    public static async Task UpdateUserUsernameAsync(Nabto.Edge.Client.Connection connection, string username, string newUsername)
    {
        await UpdateUserSettingAsync(connection, username, "username", CBORObject.FromObject(newUsername));
    }
}