using Nabto.Edge.Client;
using Microsoft.Extensions.Logging;

class Program {
    private static readonly AutoResetEvent _closing = new AutoResetEvent(false);

    static void HandleMdnsResult(MdnsResult r) {
        var action = r.Action;
        Console.WriteLine("Mdns Result. Action: " + r.Action + ", ServiceInstanceName " + r.ServiceInstanceName + ", ProductId: " + r.ProductId + ", DeviceId: " + r.DeviceId);
    }


    protected static void ctrlCHandler(object? sender, ConsoleCancelEventArgs args)
    {
        _closing.Set();
    }

    public static void Main() {

        var client = NabtoClient.Create();

        using var loggerFactory = LoggerFactory.Create (builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<NabtoClient>();
        client.SetLogger(logger);

        var mdnsScanner = client.CreateMdnsScanner();
        mdnsScanner.Handlers += HandleMdnsResult;

        mdnsScanner.Start();

        Console.WriteLine("Press CTRL-C to exit.");
        Console.CancelKeyPress += new ConsoleCancelEventHandler(ctrlCHandler);
        _closing.WaitOne();
    }

};
