using Microsoft.Extensions.Logging;
using Xunit;

namespace Nabto.Edge.Client.Tests;

public class NabtoClientTest {
    [Fact]
    public void GetVersion()
    {
        var client = NabtoClient.Create();
        var version = client.GetVersion();
        Assert.True(version.Length > 1);
    }

    [Fact]
    public void CreateDestroyClient() {
        for (int i = 0; i < 100; i++) {
            var client = NabtoClient.Create();
        }
    }

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
        Assert.Throws<ArgumentException>(() => connection.SetOptions("Invalid json"));
    }

    [Fact]
    public async void ConnectFails() {
        var client = NabtoClient.Create();
        var connection = client.CreateConnection();
        var exception = await Assert.ThrowsAsync<Exception>(() => connection.ConnectAsync());
        Assert.Equal("Invalid state", exception.Message);
    }

    [Fact]
    public async void ConnectOk() {
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
    public async void GetCoapHelloWorld() {
        var client = NabtoClient.Create();

        var connection = client.CreateConnection();
        var device = TestDevices.GetCoapDevice();
        connection.SetOptions(device.GetConnectOptions());
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() } );
        await connection.ConnectAsync();

        var coapRequest = connection.CreateCoapRequest("GET", "/hello-world");
        var response = await coapRequest.ExecuteAsync();

        ushort statusCode = response.GetResponseStatusCode();
        ushort contentFormat = response.GetResponseContentFormat();
        byte[] payload = response.GetResponsePayload();

        Assert.Equal(205, statusCode);
        Assert.Equal(0, contentFormat);
        Assert.True(payload.Length > 4);
    }

}
