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
}
