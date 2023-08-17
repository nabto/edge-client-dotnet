using Nabto.Edge.Client;

namespace Nabto.Edge.ClientIam.Impl;



using PeterO.Cbor;

internal class IamSettings
{

    public static Nabto.Edge.ClientIam.IamSettings DecodeIamSettings(CBORObject settings)
    {
        var passwordOpenPairing = settings["PasswordOpenPairing"];
        var passwordInvitePairing = settings["PasswordInvitePairing"];
        var localOpenPairing = settings["LocalOpenPairing"];

        var passwordOpenSct = settings["PasswordOpenSct"];
        var passwordOpenPassword = settings["PasswordOpenPassword"];


        var iamSettings = new Nabto.Edge.ClientIam.IamSettings();

        if (passwordOpenPairing != null)
        {
            iamSettings.PasswordOpenPairing = passwordOpenPairing.AsBoolean();
        }

        if (passwordInvitePairing != null)
        {
            iamSettings.PasswordInvitePairing = passwordInvitePairing.AsBoolean();
        }

        if (localOpenPairing != null)
        {
            iamSettings.LocalOpenPairing = localOpenPairing.AsBoolean();
        }

        if (passwordOpenSct != null)
        {
            iamSettings.PasswordOpenSct = passwordOpenSct.AsString();
        }

        if (passwordOpenPassword != null)
        {
            iamSettings.PasswordOpenPassword = passwordOpenPassword.AsString();
        }

        return iamSettings;
    }

    public static async Task<Nabto.Edge.ClientIam.IamSettings> GetIamSettingsAsync(Nabto.Edge.Client.IConnection connection)
    {
        var coapRequest = connection.CreateCoapRequest("GET", "/iam/settings");
        var response = await coapRequest.ExecuteAsync();

        var data = IamExceptionImpl.HandleDefaultCoapCborPayload(response);

        CBORObject o = CBORObject.DecodeFromBytes(data);
        var s = DecodeIamSettings(o);
        return s;
    }

    public static async Task UpdateIamSettingsAsync(Nabto.Edge.Client.IConnection connection, string setting, CBORObject value)
    {
        var coapRequest = connection.CreateCoapRequest("PUT", $"/iam/settings/{setting}");

        coapRequest.SetRequestPayload(CoapContentFormat.APPLICATION_CBOR, value.EncodeToBytes());

        var response = await coapRequest.ExecuteAsync();
        IamExceptionImpl.HandleDefaultCoap(response);
    }

    public static Task UpdateIamSettingsPasswordOpenPairingAsync(Nabto.Edge.Client.IConnection connection, bool enabled)
    {
        return UpdateIamSettingsAsync(connection, "password-open-pairing", CBORObject.FromObject(enabled));
    }

    public static Task UpdateIamSettingsPasswordInvitePairingAsync(Nabto.Edge.Client.IConnection connection, bool enabled)
    {
        return UpdateIamSettingsAsync(connection, "password-invite-pairing", CBORObject.FromObject(enabled));
    }

    public static Task UpdateIamSettingsLocalOpenPairingAsync(Nabto.Edge.Client.IConnection connection, bool enabled)
    {
        return UpdateIamSettingsAsync(connection, "local-open-pairing", CBORObject.FromObject(enabled));
    }
}
