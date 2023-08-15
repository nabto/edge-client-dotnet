using Xunit;
using Microsoft.Extensions.Logging;

namespace Nabto.Edge.Client.Tests;

public class MdnsTest
{

    [Fact]
    public void TestMdnsScannerAddHandler()
    {
        var client = Nabto.Edge.Client.INabtoClient.Create();
        var mdnsScanner = client.CreateMdnsScanner();

        IMdnsScanner.ResultHandler handler = (MdnsResult result) => { };

        mdnsScanner.Handlers += handler;

        mdnsScanner.Handlers -= handler;
    }

    [Fact]
    public void TestMdnsScannerSetHandlerToNull()
    {
        var client = Nabto.Edge.Client.INabtoClient.Create();
        var mdnsScanner = client.CreateMdnsScanner();

        IMdnsScanner.ResultHandler handler = (MdnsResult result) => { };

        mdnsScanner.Handlers = null;

        mdnsScanner.Handlers += handler;
        mdnsScanner.Handlers -= handler;
    }

    [Fact]
    public void TestDisposeMdnsScanner()
    {
        var client = Nabto.Edge.Client.INabtoClient.Create();
        var mdnsScanner = client.CreateMdnsScanner();

        IMdnsScanner.ResultHandler handler = (MdnsResult result) => { };
        mdnsScanner.Handlers += handler;
        mdnsScanner.Dispose();
        Assert.Throws<ObjectDisposedException>(() => mdnsScanner.Start());
    }

    [Fact]
    public async Task TestMdnsScan()
    {
        var client = Nabto.Edge.Client.INabtoClient.Create();
        using var testDevice = new TestDeviceRunner();
        var mdnsScanner = client.CreateMdnsScanner();

        var cts = new CancellationTokenSource();
        var invoked = false;
        MdnsResult? res = null;
        IMdnsScanner.ResultHandler handler = (MdnsResult result) =>
        {
            res = result;
            invoked = true;
            mdnsScanner.Stop();
            cts.Cancel();
        };
        mdnsScanner.Handlers += handler;
        mdnsScanner.Start();
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(10), cts.Token);
        }
        catch (TaskCanceledException)
        {
        }
        mdnsScanner.Stop();
        Assert.True(invoked);
        Assert.NotNull(res);
        Assert.Equal(res.DeviceId, testDevice.DeviceId);
        Assert.Equal(res.ProductId, testDevice.ProductId);
        Assert.NotNull(res.TxtItems);
        Assert.Equal(res.TxtItems["deviceid"], testDevice.DeviceId);
        Assert.Equal(res.TxtItems["fn"], testDevice.FriendlyName);
    }
}
