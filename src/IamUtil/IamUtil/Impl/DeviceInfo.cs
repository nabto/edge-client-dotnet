namespace Nabto.Edge.Client.Impl;

using PeterO.Cbor;

public class DeviceInfo
{

    private static Nabto.Edge.Client.DeviceDetails DecodeDeviceDetails(CBORObject cbor) {
        var pairingModes = cbor["Modes"];

        var nabtoVersion = cbor["NabtoVersion"];

        var appVersion = cbor["AppVersion"];
        var appName = cbor["AppName"];

        var productId = cbor["ProductId"];
        var deviceId = cbor["DeviceId"];
        var friendlyName = cbor["FriendlyName"];

        var dd = new DeviceDetails();

        if (pairingModes != null) {
            var values = pairingModes.Values;
            foreach (var v in values) {
                var pairingMode = v.AsString();
                switch (pairingMode) { 
                    case "LocalOpen": dd.Modes.Add(PairingMode.LOCAL_OPEN); break;
                    case "PasswordOpen": dd.Modes.Add(PairingMode.PASSWORD_OPEN); break;
                    case "LocalInitial": dd.Modes.Add(PairingMode.LOCAL_INITIAL); break;
                    case "PasswordInvite": dd.Modes.Add(PairingMode.PASSWORD_INVITE); break;
                    default: break; // ignore unknown pairing modes.
                }
            }
        }

        if (nabtoVersion != null) {
            dd.NabtoVersion = nabtoVersion.AsString();
        }

        if (appVersion != null) {
            dd.AppVersion = appVersion.AsString();
        }

        if (appName != null) {
            dd.AppName = appName.AsString();
        }

        if (productId != null) {
            dd.ProductId = productId.AsString();
        }

        if (deviceId != null) {
            dd.DeviceId = deviceId.AsString();
        }

        if (friendlyName != null) {
            dd.FriendlyName = friendlyName.AsString();
        }
        return dd;
    }

    public static async Task<DeviceDetails> GetDeviceDetailsAsync(Nabto.Edge.Client.Connection connection)
    {
        var coapRequest = connection.CreateCoapRequest("GET", "/iam/pairing");
        var response = await coapRequest.ExecuteAsync();
        IamException.HandleDefaultCoap(response);

        ushort contentFormat = response.GetResponseContentFormat();
        if (contentFormat != CoapContentFormat.APPLICATION_CBOR) {
            throw new IamException(IamError.CANNOT_PARSE_RESPONSE);
        }

        var dd = DecodeDeviceDetails(CBORObject.DecodeFromBytes(response.GetResponsePayload()));

        return dd;
    }

    public static async Task UpdateDeviceInfoAsync(Nabto.Edge.Client.Connection connection, string setting, CBORObject value)
    { 
        var coapRequest = connection.CreateCoapRequest("PUT", $"/iam/device-info/{setting}");

        coapRequest.SetRequestPayload(CoapContentFormat.APPLICATION_CBOR, value.EncodeToBytes());

        var response = await coapRequest.ExecuteAsync();
        IamException.HandleDefaultCoap(response);
    }

    public static Task UpdateDeviceFriendlyNameAsync(Nabto.Edge.Client.Connection connection, string friendlyName) {
        return UpdateDeviceInfoAsync(connection, "friendly-name", CBORObject.FromObject(friendlyName));
    }
};