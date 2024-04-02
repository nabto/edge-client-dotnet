using Nabto.Edge.Client;
using Microsoft.Extensions.Logging;
using System.CommandLine;

class Program
{
    private static readonly AutoResetEvent _closing = new AutoResetEvent(false);


    protected static void ctrlCHandler(object? sender, ConsoleCancelEventArgs args)
    {
        _closing.Set();
    }

    public static async Task Main(string[] args)
    {

        var client = INabtoClient.Create();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<INabtoClient>();
        client.SetLogger(logger);

        var productIdOption = new Option<string?>(
            name: "-p",
            description: "The product id to use."
            )
        { IsRequired = true };

        var deviceIdOption = new Option<string?>(
            name: "-d",
            description: "The device id to use.")
        { IsRequired = true };

        var rootCommand = new RootCommand("Stream Reader App");
        rootCommand.AddOption(productIdOption);
        rootCommand.AddOption(deviceIdOption);

        rootCommand.SetHandler(async (productIdValue, deviceIdValue) =>
        {

            var connection = client.CreateConnection();
            var privateKey = client.CreatePrivateKey();
            connection.SetOptions(new ConnectionOptions { PrivateKey = privateKey, ProductId = productIdValue, DeviceId = deviceIdValue, ServerConnectToken = "demosct" });

            await connection.ConnectAsync();

            var stream = connection.CreateStream();
            await stream.OpenAsync(1234);
            var readTotal = 0;
            var readIncremental = 0;
            while (true) {
                var data = await stream.ReadSomeAsync(1024);
                readTotal += data.Length;
                readIncremental += data.Length;

                if (readIncremental > 1024 * 1024)
                {
                    Console.WriteLine("Total Read: " + readTotal);
                    readIncremental = 0;
                }
            }


            Console.WriteLine("Press CTRL-C to exit.");
            Console.CancelKeyPress += new ConsoleCancelEventHandler(ctrlCHandler);
            _closing.WaitOne();
        }, productIdOption, deviceIdOption);
        await rootCommand.InvokeAsync(args);
    }

};
