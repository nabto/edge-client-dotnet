namespace Nabto.Edge.Client.Tests;

public class TestDevice {

    public string? ProductId { get; set; }
    public string? DeviceId { get; set; }
    public string? ServerUrl { get; set; }
    public string? ServerKey { get; set; }
    public string? Fingerprint { get; set; }

    public ConnectionOptions GetConnectOptions() {
        return new ConnectionOptions {
            ProductId = ProductId,
            DeviceId = DeviceId,
            ServerUrl = ServerUrl,
            ServerKey = ServerKey
        };
    }

};

public class TestDevices {

    public static TestDevice GetCoapDevice() {
        return new TestDevice { ProductId = "pr-fatqcwj9", DeviceId = "de-avmqjaje", ServerUrl = "https://pr-fatqcwj9.clients.nabto.net", ServerKey = "sk-72c860c244a6014248e64d5273e3e0ec", Fingerprint = "fcb78f8d53c67dbc4f72c36ca6cd2d5fc5592d584222059f0d76bdb514a9340c" };
    }
}
