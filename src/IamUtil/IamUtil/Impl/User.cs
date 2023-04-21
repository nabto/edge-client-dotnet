namespace Nabto.Edge.Client.Impl;

using PeterO.Cbor;

public class User {
    public static async Task CreateUserAsync(Nabto.Edge.Client.Connection connection, string username)
    { 
        var cbor = CBORObject.NewMap().Add("Username", username);

        var req = connection.CreateCoapRequest("POST", "/iam/users");
        req.SetRequestPayload(CoapContentFormat.APPLICATION_CBOR, cbor.EncodeToBytes());

        var response = await req.ExecuteAsync();

        var statusCode = response.GetResponseStatusCode();

        switch (statusCode) {
            case 409: throw new IamException(IamError.USERNAME_EXISTS, response);
            default: IamException.HandleDefaultCoap(response); break;
        }


    }

    public static async Task DeleteUserAsync(Nabto.Edge.Client.Connection connection, string username)
    { 
        var req = connection.CreateCoapRequest("DELETE", $"/iam/users/{username}");
        var response = await req.ExecuteAsync();

        var statusCode = response.GetResponseStatusCode();

        switch (statusCode) {
            case 404: throw new IamException(IamError.USER_DOES_NOT_EXIST, response);
            default: IamException.HandleDefaultCoap(response); break;
        }
    }



}