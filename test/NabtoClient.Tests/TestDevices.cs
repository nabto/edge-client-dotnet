namespace Nabto.Edge.Client.Tests;

public class TestDevice {

    public string? ProductId { get; set; }
    public string? DeviceId { get; set; }
    public string? ServerUrl { get; set; }
    public string? ServerKey { get; set; }
    public string? Fingerprint { get; set; }
    public string? ServerConnectToken { get; set; }

    public string? Password { get; set; }

    public bool? Local { get; set; }

    public bool? Remote { get; set; }

    public bool? P2p { get; set; }

    public ConnectionOptions GetConnectOptions() {
        var co = new ConnectionOptions {
            ProductId = ProductId,
            DeviceId = DeviceId,
            ServerUrl = ServerUrl,
            ServerKey = ServerKey,
            ServerConnectToken = ServerConnectToken
        };
        if ((Local != null)) {
            co.Local = Local;
        }

        if ((Remote != null)) {
            co.Remote = Remote;
        }

        if ((P2p != null)) {
            co.Rendezvous = P2p; 
        }

        return co;
    }

};

public class TestDevices
{

    public static TestDevice GetCoapDevice()
    {
        return new TestDevice { ProductId = "pr-fatqcwj9", DeviceId = "de-avmqjaje", ServerUrl = "https://pr-fatqcwj9.clients.nabto.net", ServerKey = "sk-72c860c244a6014248e64d5273e3e0ec", Fingerprint = "fcb78f8d53c67dbc4f72c36ca6cd2d5fc5592d584222059f0d76bdb514a9340c" };
    }

    public static TestDevice GetStreamDevice()
    {
        return new TestDevice { ProductId = "pr-fatqcwj9", DeviceId = "de-bdsotcgm", ServerUrl = "https://pr-fatqcwj9.clients.nabto.net", ServerKey = "sk-72c860c244a6014248e64d5273e3e0ec" };
    }

    public static TestDevice GetTcpTunnelDevice()
    {
        return new TestDevice { ProductId = "pr-fatqcwj9", DeviceId = "de-ijrdq47i", ServerUrl = "https://pr-fatqcwj9.clients.nabto.net", ServerKey = "sk-9c826d2ebb4343a789b280fe22b98305", ServerConnectToken = "WzwjoTabnvux" };
    }


    public static TestDevice GetPasswordAuthenticateDevice()
    {
        return new TestDevice
        {
            ProductId = "pr-fatqcwj9",
            DeviceId = "de-ijrdq47i",
            ServerUrl = "https://pr-fatqcwj9.clients.nabto.net",
            ServerKey = "sk-9c826d2ebb4343a789b280fe22b98305",
            ServerConnectToken = "WzwjoTabnvux",
            Password = "open-password"
        };
    }

    public static TestDevice GetLocalPairPasswordOpenDevice()
    {
        return new TestDevice
        {
            ProductId = "pr-fatqcwj9",
            DeviceId = "de-aiywxrjr",
            ServerKey = "sk-9c826d2ebb4343a789b280fe22b98305",
            ServerConnectToken = "RTLRgFXLwCsk",
            Local = true,
            Password = "pUhkiHnLhaoo"
        };
    }

    public static TestDevice GetLocalAllowAllIamDevice()
    {
        return new TestDevice
        {
            ProductId = "pr-fatqcwj9",
            DeviceId = "de-vm4kq3xy",
            ServerKey = "sk-9c826d2ebb4343a789b280fe22b98305",
            ServerConnectToken = "RTLRgFXLwCsk",
            Local = true,
            Remote = false
        };
    }
}
