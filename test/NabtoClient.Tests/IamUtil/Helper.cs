namespace Nabto.Edge.Client.Tests;

using Microsoft.Extensions.Logging;

public class IamConnection {
    public static async Task<IamConnection> Create() {
        var c = new IamConnection();

        await c.init();
        return c;
    }

    public IamConnection() {
        _client = Nabto.Edge.Client.NabtoClient.Create();
        var loggerFactory = LoggerFactory.Create (builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<NabtoClient>();
        _client.SetLogger(logger);
        Connection = _client.CreateConnection();
        Username = TestUtil.UniqueUsername();
    }

    public async Task init() { 
        var testDevice = TestDevices.GetLocalAllowAllIamDevice();
        Connection.SetOptions(testDevice.GetConnectOptions());
        Connection.SetOptions(new ConnectionOptions { PrivateKey = _client.CreatePrivateKey() });
        try
        {
            await Connection.ConnectAsync();
        } catch (NabtoException e) {
            if (e.ErrorCode == NabtoClientError.NO_CHANNELS) { 

            }
        }
        await IamUtil.PairLocalOpenAsync(Connection, Username);
    }

    public Nabto.Edge.Client.Connection Connection { get; set; }
    private Nabto.Edge.Client.NabtoClient _client;
    public string Username { get; set; }
}
