using Microsoft.Extensions.Logging;


namespace Nabto.Edge.Client.Tests;


public class ConnectionTest {
    [Fact]
    public async void AsyncClose()
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



    // [Fact]
    // public async void TestConnectionEvents()
    // {
    //     bool connected = false;
    //     var client = NabtoClient.Create();
    //     var connection = client.CreateConnection();
    //     connection.ConnectionEventHandlers += ((e) => { if (e == Connection.ConnectionEvent.Connected) { connected = true; }});
    //     var device = TestDevices.GetCoapDevice();
    //     connection.SetOptions(device.GetConnectOptions());
    //     connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() } );
    //     await connection.ConnectAsync();
    //     await connection.CloseAsync();
    //     Assert.True(connected);
    // }

    [Fact]
    public async void TestConnectionEvents()
    {
        bool connected = false;
        bool closed = false;
        var client = NabtoClient.Create();
        var connection = client.CreateConnection();
        connection.ConnectionEventHandlers += ((e) => { if (e == Connection.ConnectionEvent.Connected) { connected = true; } else if (e == Connection.ConnectionEvent.Closed) {closed = true; } });
        var device = TestDevices.GetCoapDevice();
        connection.SetOptions(device.GetConnectOptions());
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() } );
        await connection.ConnectAsync();
        await connection.CloseAsync();
        Assert.True(connected);
        Assert.True(closed);
    }

}
