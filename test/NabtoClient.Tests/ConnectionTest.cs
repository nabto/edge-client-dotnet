using Microsoft.Extensions.Logging;


namespace Nabto.Edge.Client.Tests;


public class ConnectionTest {
    [Fact]
    public async Task AsyncClose()
    {
        var client = NabtoClient.Create();

        //using var loggerFactory = LoggerFactory.Create (builder => builder.AddConsole());
        //var logger = loggerFactory.CreateLogger<NabtoClient>();
        //client.SetLogger(logger);

        var connection = client.CreateConnection();
        var device = TestDevices.GetCoapDevice();
        connection.SetOptions(device.GetConnectOptions());
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() } );
        await connection.ConnectAsync();
        await connection.CloseAsync();
    }

    [Fact]
    public async Task TestConnectionEvents()
    {
        var connected = new TaskCompletionSource<bool>();
        var closed = new TaskCompletionSource<bool>();
        var client = NabtoClient.Create();
        var connection = client.CreateConnection();
        connection.ConnectionEventHandlers += ((e) => { if (e == Connection.ConnectionEvent.Connected) { connected.SetResult(true); } else if (e == Connection.ConnectionEvent.Closed) { closed.SetResult(true); } });
        var device = TestDevices.GetCoapDevice();
        connection.SetOptions(device.GetConnectOptions());
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() } );
        await connection.ConnectAsync();
        await connection.CloseAsync();
        connected.Task.Wait(1000);
        closed.Task.Wait(1000);
        Assert.True(connected.Task.Result);
        Assert.True(closed.Task.Result);
    }

}
