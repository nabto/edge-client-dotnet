namespace Nabto.Edge.Client;

public class ConnectionOptions {
    public string? PrivateKey { get; set; }
    public string? ProductId { get; set; }
    public string? DeviceId { get; set; }
    public string? ServerUrl { get; set; }
    public string? ServerKey { get; set; }
    public string? ServerJwtToken { get; set; }
    public string? ServerConnectToken { get; set; }
    public string? AppName { get; set; }
    public string? AppVersion { get; set; }

    public int? KeepAliveInterval { get; set; }
    public int? KeepAliveRetryInterval { get; set; }
    public int? KeepAliveMaxRetries { get; set; }

    public int? DtlsHelloTimeout { get; set; }

    public bool? Local { get; set; }

    public bool? Remote { get; set; }

    public bool? Rendezvous { get; set; }

    public bool? ScanLocalConnect { get; set; }
}

public interface Connection
{

    public void SetOptions(string json);

    public void SetOptions(ConnectionOptions options);
    public string GetDeviceFingerprint();
    public string GetClientFingerprint();
    public Task ConnectAsync();

    public Nabto.Edge.Client.CoapRequest CreateCoapRequest(string method, string path);

};
