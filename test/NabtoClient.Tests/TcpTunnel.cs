using Xunit;
using Microsoft.Extensions.Logging;

namespace Nabto.Edge.Client.Tests;

public class TcpTunnelTest {

    private static async Task<Nabto.Edge.Client.Connection> CreateTcpTunnelDeviceConnectionAsync()
    {
        var client = NabtoClient.Create();
        // using var loggerFactory = LoggerFactory.Create (builder => builder.AddConsole());
        // var logger = loggerFactory.CreateLogger<NabtoClient>();
        // client.SetLogger(logger);
        var connection = client.CreateConnection();
        var device = TestDevices.GetTcpTunnelDevice();
        connection.SetOptions(device.GetConnectOptions());
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() } );
        await connection.ConnectAsync();
        return connection;
    }

    [Fact]
    public async void TestOpenClose()
    {
        var connection = await CreateTcpTunnelDeviceConnectionAsync();
        var tunnel = connection.CreateTcpTunnel();

        ushort localPort = 0;
        await tunnel.OpenAsync("http", localPort);
        await tunnel.CloseAsync();
    }

    [Fact]
    public async void TestGetDataFromTunnel()
    {
        var connection = await CreateTcpTunnelDeviceConnectionAsync();
        var tunnel = connection.CreateTcpTunnel();

        ushort localPort = 0;
        await tunnel.OpenAsync("http", localPort);
        ushort boundPort = tunnel.GetLocalPort();
        HttpClient client = new HttpClient();

        using HttpResponseMessage response = await client.GetAsync("http://127.0.0.1:"+ boundPort + "/");
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        await tunnel.CloseAsync();
    }
}
