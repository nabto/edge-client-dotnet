namespace Nabto.Edge.ClientIam.Tests;

using Xunit;

using Nabto.Edge.Client.Tests;
using Nabto.Edge.Client;

public class LocalAllowAllIamFixture : IAsyncLifetime, IDisposable
{

    protected Nabto.Edge.Client.NabtoClient _client;
    protected Nabto.Edge.Client.Connection _connection;

    protected Nabto.Edge.Client.Tests.TestDeviceRunner _testDevice = new TestDeviceRunner();

    public LocalAllowAllIamFixture()
    {
        _client = Nabto.Edge.Client.NabtoClient.Create();
        _connection = _client.CreateConnection();
        _connection.SetOptions(new ConnectionOptions { ProductId = _testDevice.ProductId, DeviceId = _testDevice.DeviceId, Local = true, Remote = false });
        _connection.SetOptions(new ConnectionOptions { PrivateKey = _client.CreatePrivateKey() });
    }

    public void Dispose()
    {
        _testDevice.Dispose();
    }

    public ConnectionOptions GetConnectionOptions()
    {
        return new ConnectionOptions { ProductId = _testDevice.ProductId, DeviceId = _testDevice.DeviceId, Local = true, Remote = false };
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        await _connection.ConnectAsync();
    }

    public async Task<string> CreateDefaultUser()
    {
        var username = TestUtil.UniqueUsername();
        await IamUtil.CreateUserAsync(_connection, new IamUser { Username = username, Role = "Administrator", DisplayName = "displayname", Fingerprint = _connection.GetClientFingerprint() });
        return username;
    }
}
