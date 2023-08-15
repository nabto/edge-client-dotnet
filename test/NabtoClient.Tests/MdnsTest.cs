using Xunit;
using Microsoft.Extensions.Logging;

namespace Nabto.Edge.Client.Tests;

public class MdnsTest 
{

    [Fact]
    public void TestMdnsScanner()
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

        var invocationCount = 0;
        MdnsResult? res = null;
        IMdnsScanner.ResultHandler handler = (MdnsResult result) => {
            res = result;
            invocationCount++;
        };
        mdnsScanner.Handlers += handler;
        mdnsScanner.Start();
        await Task.Delay(TimeSpan.FromSeconds(1));
        mdnsScanner.Stop();
        Assert.Equal(invocationCount, 1);
        Assert.NotNull(res);
        Assert.Equal(res.DeviceId, testDevice.DeviceId);
        Assert.Equal(res.ProductId, testDevice.ProductId);
        Assert.NotNull(res.TxtItems);
        Assert.Equal(res.TxtItems["deviceid"], testDevice.DeviceId);
        Assert.Equal(res.TxtItems["fn"], testDevice.FriendlyName);
    }

}


