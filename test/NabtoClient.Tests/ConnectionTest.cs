using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;


namespace Nabto.Edge.Client.Tests;


public class ConnectionTest
{

    [Fact]
    public void createDestroyConnection()
    {
        var client = NabtoClient.Create();
        for (int i = 0; i < 100; i++)
        {
            var connection = client.CreateConnection();
        }
    }

    [Fact]
    public void InvalidFormattedJsonToConnectOptionsThrowsArgumentException()
    {
        var client = NabtoClient.Create();
        var connection = client.CreateConnection();
        var ex = Assert.Throws<ArgumentException>(() => connection.SetOptions("Invalid json"));
    }

    [Fact]
    public async Task ConnectFails()
    {
        var client = NabtoClient.Create();
        var connection = client.CreateConnection();
        var exception = await Assert.ThrowsAsync<NabtoException>(() => connection.ConnectAsync());
        Assert.Equal(NabtoClientError.INVALID_STATE, exception.ErrorCode);
    }

    [Fact]
    public async Task ConnectNoChannelsException()
    {
        var client = NabtoClient.Create();
        var device = TestDevices.GetCoapDevice();
        device.DeviceId = "foo";
        var connection = client.CreateConnection();
        connection.SetOptions(device.GetConnectOptions());
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() });
        var exception = await Assert.ThrowsAsync<NabtoException>(() => connection.ConnectAsync());
        Assert.Equal(NabtoClientError.NO_CHANNELS, exception.ErrorCode);
    }

    [Fact]
    public async Task GetConnectionType()
    {
        var client = NabtoClient.Create();

        // using var loggerFactory = LoggerFactory.Create (builder => builder.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Trace));
        // var logger = loggerFactory.CreateLogger<NabtoClient>();
        // client.SetLogger(logger);

        var connection = client.CreateConnection();
        var device = TestDevices.GetCoapDevice();
        device.Local = false;
        device.P2p = false;
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() });
        connection.SetOptions(device.GetConnectOptions());

        var serializerOptions = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        var options = device.GetConnectOptions();
        string jsonString = JsonSerializer.Serialize(options, serializerOptions);
        //        Console.WriteLine("Connection.options: {0}", jsonString);

        await connection.ConnectAsync();
        var type = connection.GetConnectionType();
        Assert.Equal(Connection.ConnectionType.Relay, type);
    }


    [Fact]
    public async Task ConnectOk()
    {
        var client = NabtoClient.Create();

        //using var loggerFactory = LoggerFactory.Create (builder => builder.AddConsole());
        //var logger = loggerFactory.CreateLogger<NabtoClient>();
        //client.SetLogger(logger);

        var connection = client.CreateConnection();
        var device = TestDevices.GetCoapDevice();
        connection.SetOptions(device.GetConnectOptions());
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() });
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
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() });
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
        connection.ConnectionEventHandlers += ((e) =>
        {
            //Console.WriteLine("event {0}", e);
            if (e == Connection.ConnectionEvent.Connected) { connected.SetResult(true); } else if (e == Connection.ConnectionEvent.Closed) { closed.SetResult(true); }
        });
        var device = TestDevices.GetCoapDevice();
        connection.SetOptions(device.GetConnectOptions());
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() });
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
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() });
        await connection.ConnectAsync();
        Assert.NotNull(device.Password);
        await connection.PasswordAuthenticateAsync("", device.Password);
        await connection.CloseAsync();
    }

    [Fact]
    public void TestSyncDispose()
    {
        var client = NabtoClient.Create();
        using (var connection = client.CreateConnection())
        {
            // todo: use a mock framework and inject the nabto resources as dependencies to actually test; for now just put a breakpoint in the dispose impl 
            //            Console.WriteLine("Invoking Dispose after this line");
        }
    }

    [Fact]
    public async Task TestAsyncDispose()
    {
        var client = NabtoClient.Create();
        await using (var connection = client.CreateConnection())
        {
            // todo: use a mock framework and inject the nabto resources as dependencies to actually test; for now just put a breakpoint in the dispose impl
            //            Console.WriteLine("Invoking AsyncDispose after this line");
        }
    }

    [Fact]
    public void TestSyncDoubleDisposeOk()
    {
        var client = NabtoClient.Create();
        var connection = client.CreateConnection();

        connection.Dispose();

        Exception ex = Record.Exception(() => connection.Dispose());
        Assert.Null(ex);
    }

    [Fact]
    public async void TestAsyncDoubleDisposalOk()
    {
        var client = NabtoClient.Create();
        var connection = client.CreateConnection();

        await connection.DisposeAsync();

        Exception ex = await Record.ExceptionAsync(async () => await connection.DisposeAsync());
        Assert.Null(ex);
    }

    [Fact]
    public async Task GracefullyHandleDisposeClientBeforeConnect()
    {
        var client = NabtoClient.Create();
        var connection = client.CreateConnection();
        var device = TestDevices.GetCoapDevice();
        connection.SetOptions(device.GetConnectOptions());
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() });

        await client.DisposeAsync();
        await Assert.ThrowsAsync<ObjectDisposedException>(async () => await connection.ConnectAsync());
    }

    [Fact]
    public async Task GracefullyHandleDisposeClientBeforeCoapRequest()
    {
        var client = NabtoClient.Create();
        var connection = client.CreateConnection();
        var device = TestDevices.GetCoapDevice();
        connection.SetOptions(device.GetConnectOptions());
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() });
        await connection.ConnectAsync();
        var coapRequest = connection.CreateCoapRequest("GET", "/hello-world");

        await client.DisposeAsync();
        await Assert.ThrowsAsync<ObjectDisposedException>(async () => await coapRequest.ExecuteAsync());
    }
}
