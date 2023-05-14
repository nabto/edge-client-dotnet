using Nabto.Edge.Client;
using System.CommandLine;
using Microsoft.Extensions.Logging;

public class ConnectionEntry
{
    public string DeviceId { get; }
    public string ProductId { get; }
    public Connection Connection { get; }

    public ConnectionEntry(string deviceId, string productId, Connection connection)
    {
        DeviceId = deviceId;
        ProductId = productId;
        Connection = connection;
    }
}

public sealed class ConnectionManager
{
    private const string _privateKeyFileName = "private-key.pem";
    private readonly Dictionary<int, ConnectionEntry> _connections = new Dictionary<int, ConnectionEntry>();
    private int _idCounter = 0;
    private NabtoClient _client;
    private string _privateKey;
    private static readonly Lazy<ConnectionManager> _instance = new Lazy<ConnectionManager>(() => new ConnectionManager());

    private ConnectionManager() { }

    public static ConnectionManager Instance
    {
        get
        {
            return _instance.Value;
        }
    }

    public void Start() {
        _client = NabtoClient.Create();
        using var loggerFactory = LoggerFactory.Create (builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<NabtoClient>();
        _client.SetLogger(logger);

        if (!TryReadPrivateKey())
        {
            _privateKey = _client.CreatePrivateKey();
            WritePrivateKey();
        }
    }

    public bool TryReadPrivateKey()
    {
        if (File.Exists(_privateKeyFileName))
        {
            _privateKey = File.ReadAllText(_privateKeyFileName);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void WritePrivateKey()
    {
        File.WriteAllText(_privateKeyFileName, _privateKey);
    }

    public void Stop() {
//        _client.Stop(); todo - dispose
    }

    public int Connect(string deviceId, string productId, string? sct = null) {
        var connection = _client.CreateConnection();
        ConnectionOptions options = new ConnectionOptions { 
            PrivateKey = _privateKey,
            DeviceId = deviceId,
            ProductId = productId
        };         
        if (sct != null) {
            options.ServerConnectToken = sct;
        }
        connection.SetOptions(options);
        try {
            connection.ConnectAsync().Wait();
            Console.WriteLine("Successfully connected to device {0} {1}", productId, deviceId);
        } catch (Exception e) {
            Console.WriteLine("Failed to connect to device: {0}", e.Message);
            return -1;            
        }
        var id = AddConnection(new ConnectionEntry(deviceId, productId, connection));
        return id;
    }

    public int AddConnection(ConnectionEntry connection)
    {
        lock (_connections)
        {
            var id = _idCounter++;
            _connections[id] = connection;
            return id;
        }
    }

    public bool RemoveConnection(int id)
    {
        lock (_connections)
        {
            return _connections.Remove(id);
        }
    }

    public bool GetConnection(int id, out Connection connection)
    {
        lock (_connections)
        {
            ConnectionEntry entry;
            if (_connections.TryGetValue(id, out entry)) {
                connection = entry.Connection;
                return true;
            } else {
                connection = null;
                return false;
            }
        }
    }
    public Dictionary<int, ConnectionEntry> GetConnectionEntries() {  
        return _connections;
    }

}