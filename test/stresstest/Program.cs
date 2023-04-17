using Nabto.Edge.Client;
using Microsoft.Extensions.Logging;
using Nabto.Edge.Client.Tests;
//using System.CommandLine;



class Program
{
    private static readonly AutoResetEvent _closing = new AutoResetEvent(false);

    protected static void ctrlCHandler(object? sender, ConsoleCancelEventArgs args)
    {
        _closing.Set();
    }

    private static void GcCallback(object? state)
    {
        Console.WriteLine("invoking GC");
        GC.Collect();
        Console.WriteLine("GC done");

    }

    public static async Task DoConnections(int n)
    {
        var client = NabtoClient.Create();
        //using var loggerFactory = LoggerFactory.Create (builder => builder.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Trace));
        //using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Trace));
        //var logger = loggerFactory.CreateLogger<NabtoClient>();
        //client.SetLogger(logger);


        //var c2 = Nabto.Edge.Client.Impl.NabtoClient.Create();

        // for (int j = 0; j < 1000; j++) {
        //     var l = Nabto.Edge.Client.Impl.Listener.Create(c2);
        //     Int64 sum = 0;


        //     for (Int64 k = 0; k < 10000000; k++) {
        //          sum+=k;
        //     }
        //     // Console.WriteLine("{0}", sum);
        //     l.Stop();

        //     for (Int64 k = 0; k < 10000000; k++) {
        //         sum+=k;
        //     }
        //     Console.WriteLine("{0}", sum);
        // }


        for (int i = 0; i < n; i++)
        {
            var connection = client.CreateConnection();
            var device = TestDevices.GetTcpTunnelDevice();
            connection.SetOptions(device.GetConnectOptions());
            connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() });
            await connection.ConnectAsync();

            // Int64 sum = 0;


            // for (Int64 j = 0; j < 1000000000; j++) {
            //     sum+=j;
            // }
            // Console.WriteLine("{0}", sum);

            //var coapRequest = connection.CreateCoapRequest("GET", "/hello-world");
            //var response = await coapRequest.ExecuteAsync();


            var tunnel = connection.CreateTcpTunnel();

            ushort localPort = 0;
            await tunnel.OpenAsync("http", localPort);
            ushort boundPort = tunnel.GetLocalPort();
            HttpClient httpClient = new HttpClient();

            using HttpResponseMessage response = await httpClient.GetAsync("http://127.0.0.1:" + boundPort + "/");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            await tunnel.CloseAsync();
            await connection.CloseAsync();
            Console.WriteLine("connection {0}", i);
        }
    }

    public static async Task Main(string[] args)
    {
        //System.Threading.Timer timer = new System.Threading.Timer(GcCallback, null, 200, 200);
        var tasks = new List<Task>();

        for (int j = 0; j < 100; j++)
        {
            tasks.Add(Task.Run(() => DoConnections(100)));
        }

        await Task.WhenAll(tasks);
    }

};
