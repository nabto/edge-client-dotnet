using Xunit;
using Microsoft.Extensions.Logging;

namespace Nabto.Edge.Client.Tests;

public class MdnsTest {

    [Fact]
    public void TestMdnsScanner() {
        var client = Nabto.Edge.Client.NabtoClient.Create();
        var mdnsScanner = client.CreateMdnsScanner();

        MdnsResultHandler handler = (MdnsResult result) => {};

        mdnsScanner.Handlers += handler;

        mdnsScanner.Handlers -= handler;
    }
}
