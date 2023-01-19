using Xunit;
using Microsoft.Extensions.Logging;

namespace Nabto.Edge.Client.Tests;

public class MdnsTest {

    [Fact]
    public void TestMdnsScanner() {
        var client = Nabto.Edge.Client.NabtoClient.Create();
        var mdnsScanner = client.CreateMdnsScanner();

        MdnsScanner.ResultHandler handler = (MdnsResult result) => {};

        mdnsScanner.Handlers += handler;

        mdnsScanner.Handlers -= handler;
    }

    [Fact]
    public void TestMdnsScannerSetHandlerToNull() {
        var client = Nabto.Edge.Client.NabtoClient.Create();
        var mdnsScanner = client.CreateMdnsScanner();

        MdnsScanner.ResultHandler handler = (MdnsResult result) => {};

        mdnsScanner.Handlers = null;

        mdnsScanner.Handlers += handler;
        mdnsScanner.Handlers -= handler;
    }
}
