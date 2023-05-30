using Microsoft.Extensions.Logging;
using Xunit;

namespace Nabto.Edge.Client.Tests;

public class CoapTest {
    [Fact]
    public async Task GetCoapHelloWorld() {
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

    [Fact]
    public async Task GracefullyHandleDisposeRequestBeforeResponse() {
        var client = NabtoClient.Create();
        var connection = client.CreateConnection();
        var device = TestDevices.GetCoapDevice();
        connection.SetOptions(device.GetConnectOptions());
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() } );
        await connection.ConnectAsync();

        CoapResponse response;
        using (var coapRequest = connection.CreateCoapRequest("GET", "/hello-world")) {
            response = await coapRequest.ExecuteAsync();
        }
        Assert.Throws<ObjectDisposedException>(() => response.GetResponseStatusCode());
    }

    [Fact]
    public async Task GracefullyHandleDisposeConnectionBeforeRequest() {
        var client = NabtoClient.Create();

        // using var loggerFactory = LoggerFactory.Create (builder => builder.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Trace));
        // var logger = loggerFactory.CreateLogger<NabtoClient>();
        // client.SetLogger(logger);

        CoapRequest request;
        using (var connection = client.CreateConnection()) {
            var device = TestDevices.GetCoapDevice();
            connection.SetOptions(device.GetConnectOptions());
            connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() } );
            await connection.ConnectAsync();
            request = connection.CreateCoapRequest("GET", "/hello-world");
        }
        Exception ex = await Record.ExceptionAsync(async () => await request.ExecuteAsync());
        Assert.IsType<NabtoException>(ex);
        Assert.Equal(((NabtoException)ex).ErrorCode, NabtoClientError.STOPPED);
    }


}