using Microsoft.Extensions.Logging;


namespace Nabto.Edge.Client.Tests;


public class ConnectionTest {

    [Fact]
    public void createDestroyConnection()
    {
        var client = NabtoClient.Create();
        for (int i = 0; i < 100; i++) {
            var connection = client.CreateConnection();
        }
    }

    [Fact]
    public void InvalidFormattedJsonToConnectOptionsThrowsArgumentException() {
        var client = NabtoClient.Create();
        var connection = client.CreateConnection();
        var ex = Assert.Throws<NabtoException>(() => connection.SetOptions("Invalid json"));
        Assert.Equal(NabtoClientError.INVALID_ARGUMENT, ex.ErrorCode);
    }

    [Fact]
    public async Task ConnectFails() {
        var client = NabtoClient.Create();
        var connection = client.CreateConnection();
        var exception = await Assert.ThrowsAsync<NabtoException>(() => connection.ConnectAsync());
        Assert.Equal(NabtoClientError.INVALID_STATE, exception.ErrorCode);
    }

    [Fact]
    public async Task ConnectOk() {
        var client = NabtoClient.Create();

        //using var loggerFactory = LoggerFactory.Create (builder => builder.AddConsole());
        //var logger = loggerFactory.CreateLogger<NabtoClient>();
        //client.SetLogger(logger);

        var connection = client.CreateConnection();
        var device = TestDevices.GetCoapDevice();
        connection.SetOptions(device.GetConnectOptions());
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() } );
        await connection.ConnectAsync();
        var deviceFingerprint = connection.GetDeviceFingerprint();
        Assert.Equal(deviceFingerprint, device.Fingerprint);
    }

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
        //using var loggerFactory = LoggerFactory.Create (builder => builder.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Trace));
        //var logger = loggerFactory.CreateLogger<NabtoClient>();
        //client.SetLogger(logger);
        var connection = client.CreateConnection();
        connection.ConnectionEventHandlers += ((e) => {
            //Console.WriteLine("event {0}", e);
            if (e == Connection.ConnectionEvent.Connected) { connected.SetResult(true); } else if (e == Connection.ConnectionEvent.Closed) { closed.SetResult(true); } 
            });
        var device = TestDevices.GetCoapDevice();
        connection.SetOptions(device.GetConnectOptions());
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() } );
        await connection.ConnectAsync();
        await connection.CloseAsync();
        Assert.True(connected.Task.Wait(5000));
        Assert.True(closed.Task.Wait(5000));
        Assert.True(connected.Task.Result);
        Assert.True(closed.Task.Result);
    }

    [Fact]
    public async Task TestPasswordAuthenticate()
    {
        var connected = new TaskCompletionSource<bool>();
        var closed = new TaskCompletionSource<bool>();
        var client = NabtoClient.Create();
        //using var loggerFactory = LoggerFactory.Create (builder => builder.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Trace));
        //var logger = loggerFactory.CreateLogger<NabtoClient>();
        //client.SetLogger(logger);
        var connection = client.CreateConnection();
        var device = TestDevices.GetPasswordAuthenticateDevice();
        connection.SetOptions(device.GetConnectOptions());
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() } );
        await connection.ConnectAsync();
        await connection.PasswordAuthenticate("", device.Password);
        await connection.CloseAsync();
    }

}
