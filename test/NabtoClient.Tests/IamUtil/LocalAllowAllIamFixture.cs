namespace Nabto.Edge.Client.Tests;

using Xunit;

public class LocalAllowAllIamFixture : IAsyncLifetime
{

    protected Nabto.Edge.Client.NabtoClient _client;
    protected Nabto.Edge.Client.Connection _connection;

    public LocalAllowAllIamFixture()
    {
        _client = Nabto.Edge.Client.NabtoClient.Create();
        _connection = _client.CreateConnection();
        var testDevice = TestDevices.GetLocalAllowAllIamDevice();
        _connection.SetOptions(testDevice.GetConnectOptions());
        _connection.SetOptions(new ConnectionOptions { PrivateKey = _client.CreatePrivateKey() });
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        await _connection.ConnectAsync();
    }
}