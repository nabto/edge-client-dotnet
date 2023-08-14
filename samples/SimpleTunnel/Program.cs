using Nabto.Edge.Client;
using System.CommandLine;
using Microsoft.Extensions.Logging;

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

        //using var loggerFactory = LoggerFactory.Create (builder => builder.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Trace));
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<INabtoClient>();
        client.SetLogger(logger);

        var productIdOption = new Option<string?>(
            name: "-p",
            description: "The product id to use."
            )
        { IsRequired = true };
        productIdOption.AddAlias("--product-id");

        var deviceIdOption = new Option<string?>(
            name: "-d",
            description: "The device id to use.")
        { IsRequired = true };
        deviceIdOption.AddAlias("--device-id");

        var serviceOption = new Option<string>(
            name: "--service",
            description: "The TCP Service to use.",
            getDefaultValue: () => "http");


        var localPortOption = new Option<ushort>(
            name: "--local-port",
            description: "The local port to bind the tcp tunnel to",
            getDefaultValue: () => 0
        );

        var rootCommand = new RootCommand("Sample Tunnel App");

        rootCommand.AddOption(productIdOption);
        rootCommand.AddOption(deviceIdOption);
        rootCommand.AddOption(serviceOption);
        rootCommand.AddOption(localPortOption);

        rootCommand.SetHandler(async (productIdValue, deviceIdValue, serviceValue, localPortValue) =>
        {

            Console.WriteLine("Connecting to the device, ProductId: " + productIdValue + ", DeviceId: " + deviceIdValue);
            var connection = client.CreateConnection();
            connection.SetOptions(new ConnectionOptions { ProductId = productIdValue, DeviceId = deviceIdValue, ServerConnectToken = "demosct", PrivateKey = client.CreatePrivateKey() });

            try
            {
                await connection.ConnectAsync();
            }
            catch (NabtoException ex)
            {
                if (ex.ErrorCode == NabtoClientError.NO_CHANNELS)
                {
                    int localError = connection.GetLocalChannelErrorCode();
                    int remoteError = connection.GetRemoteChannelErrorCode();
                    int directCandidatesError = connection.GetDirectCandidatesChannelErrorCode();
                    if (remoteError != NabtoClientError.NONE)
                    {
                        Console.WriteLine("Could not connect to the device remotely. The error message is: " + NabtoClientError.GetErrorMessage(remoteError));
                    }
                    if (localError != NabtoClientError.NONE)
                    {
                        if (localError == NabtoClientError.NOT_FOUND)
                        {
                            Console.WriteLine("The device was not found on the local network");
                        }
                        else
                        {
                            Console.WriteLine("Could not connect to the device locally. The error message is: " + NabtoClientError.GetErrorMessage(localError));
                        }
                    }
                    if (directCandidatesError != NabtoClientError.NONE)
                    {
                        Console.WriteLine("Could not connect to the device using the direct candidate, reason: " + NabtoClientError.GetErrorMessage(directCandidatesError));
                    }
                }
                return;
            }

            var tunnel = connection.CreateTcpTunnel();
            await tunnel.OpenAsync(serviceValue, localPortValue);

            var localPort = tunnel.GetLocalPort();
            Console.WriteLine("Connected to simple tunnel, listening for TCP connects on 127.0.0.1:" + localPort);

            Console.WriteLine("Press CTRL-C to exit.");
            Console.CancelKeyPress += new ConsoleCancelEventHandler(ctrlCHandler);
            _closing.WaitOne();
        },
        productIdOption, deviceIdOption, serviceOption, localPortOption);

        await rootCommand.InvokeAsync(args);
    }

};
