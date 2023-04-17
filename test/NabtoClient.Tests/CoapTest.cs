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
}