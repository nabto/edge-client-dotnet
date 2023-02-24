using Nabto.Edge.Client;
using Microsoft.Extensions.Logging;
using Nabto.Edge.Client.Tests;



class Program {
    private static readonly AutoResetEvent _closing = new AutoResetEvent(false);

    protected static void ctrlCHandler(object? sender, ConsoleCancelEventArgs args)
    {
        _closing.Set();
    }

    private static void GcCallback(object? state) {
        Console.WriteLine("invoking GC");
        GC.Collect();
    }

    public static async Task Main(string[] args) 
    {
        int i = 0;
        var gcTimer = new System.Threading.Timer(GcCallback, null, 0, 500);
    
        var client = NabtoClient.Create();
        using var loggerFactory = LoggerFactory.Create (builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<NabtoClient>();
        client.SetLogger(logger);

        while (true) {
            var connection = client.CreateConnection();
            var device = TestDevices.GetTcpTunnelDevice();
            connection.SetOptions(device.GetConnectOptions());
            connection.SetOptions(new ConnectionOptions { PrivateKey = client.CreatePrivateKey() });
            await connection.ConnectAsync();

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
            i++;
        }

        //Dispose the timer
        gcTimer.Dispose();
    
    }

};
