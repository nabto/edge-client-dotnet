using Xunit;
using Microsoft.Extensions.Logging;

namespace Nabto.Edge.Client.Tests;

public class StreamTest
{

    private static async Task<Nabto.Edge.Client.IConnection> CreateStreamDeviceConnectionAsync()
    {
        var client = INabtoClient.Create();
        // using var loggerFactory = LoggerFactory.Create (builder => builder.AddConsole());
        // var logger = loggerFactory.CreateLogger<NabtoClient>();
        // client.SetLogger(logger);
        var connection = client.CreateConnection();
        var device = TestDevices.GetStreamDevice();
        connection.SetOptions(device.GetConnectOptions());
        connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() });
        await connection.ConnectAsync();
        return connection;
    }

    [Fact]
    public async Task TestStreamWriteThenReadSome()
    {
        var connection = await CreateStreamDeviceConnectionAsync();
        var stream = connection.CreateStream();
        UInt32 streamPort = 42;
        await stream.OpenAsync(streamPort);
        var data = System.Text.UTF8Encoding.UTF8.GetBytes("hello");
        await stream.WriteAsync(data);
        var received = await stream.ReadSomeAsync(42);
        Assert.True(received.Length > 0);
    }

    [Fact]
    public async Task TestStreamReadAll()
    {
        var connection = await CreateStreamDeviceConnectionAsync();
        var stream = connection.CreateStream();
        UInt32 streamPort = 42;
        await stream.OpenAsync(streamPort);
        var data = System.Text.UTF8Encoding.UTF8.GetBytes("hello");
        await stream.WriteAsync(data);
        var received = await stream.ReadAllAsync(data.Length);
        Assert.Equal(data, received);
    }

    [Fact]
    public async Task TestStreamWriteAfterClose()
    {
        var connection = await CreateStreamDeviceConnectionAsync();
        var stream = connection.CreateStream();
        UInt32 streamPort = 42;
        await stream.OpenAsync(streamPort);
        await stream.CloseAsync();
        var data = System.Text.UTF8Encoding.UTF8.GetBytes("hello");
        var ex = await Assert.ThrowsAsync<NabtoException>(() => stream.WriteAsync(data));
        Assert.Equal(NabtoClientError.CLOSED, ex.ErrorCode);

    }

    [Fact]
    public async Task TestStreamDispose()
    {
        var connection = await CreateStreamDeviceConnectionAsync();
        var stream = connection.CreateStream();
        UInt32 streamPort = 42;
        await stream.OpenAsync(streamPort);
        stream.Dispose();
        var data = System.Text.UTF8Encoding.UTF8.GetBytes("hello");
        await Assert.ThrowsAsync<ObjectDisposedException>(async () => await stream.WriteAsync(data));
    }

    [Fact]
    public async Task TestStreamStop()
    {
        var connection = await CreateStreamDeviceConnectionAsync();
        var stream = connection.CreateStream();
        UInt32 streamPort = 42;
        await stream.OpenAsync(streamPort);
        stream.Stop();
        var data = System.Text.UTF8Encoding.UTF8.GetBytes("hello");
        var ex = await Assert.ThrowsAsync<NabtoException>(() => stream.WriteAsync(data));
        Assert.Equal(NabtoClientError.STOPPED, ex.ErrorCode);
    }


}
