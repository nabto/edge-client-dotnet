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

public interface IConnectionManager
{
    int AddConnection(ConnectionEntry connection);
    Task CloseAsync();
    int Connect(string deviceId, string productId, string sct = null);
    void Dispose();
    ValueTask DisposeAsync();
    bool GetConnection(int id, out Connection connection);
    Dictionary<int, ConnectionEntry> GetConnectionEntries();
    bool RemoveConnection(int id);
    void Start();
    void Stop();
    Task StopAsync();
    bool TryReadPrivateKey();
    void WritePrivateKey();
}

public sealed class ConnectionManager : IDisposable, IAsyncDisposable, IConnectionManager
{
    private const string _privateKeyFileName = "private-key.pem";
    private readonly Dictionary<int, ConnectionEntry> _connections = new Dictionary<int, ConnectionEntry>();
    private int _idCounter = 0;
    private NabtoClient _client;
    private string _privateKey;
    private bool _disposedUnmanaged;

    public void Start()
    {
        _client = NabtoClient.Create();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
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

    public void Stop()
    {
        //        _client.Stop(); TODO client dispose
        foreach (var value in _connections.Values)
        {
            value.Connection.Dispose();
        }
    }

    public async Task CloseAsync()
    {
        var tasks = new List<Task>();
        foreach (var entry in _connections.Values)
        {
            var task = entry.Connection.CloseAsync();
            tasks.Add(task);
        }
        await Task.WhenAll(tasks);
    }

    public async Task StopAsync()
    {
        var tasks = new List<ValueTask>();
        foreach (var entry in _connections.Values)
        {
            var task = entry.Connection.DisposeAsync();
            tasks.Add(task);
        }
        for (var i = 0; i < tasks.Count; i++)
        {
            await tasks[i].ConfigureAwait(false);
        }
        _connections.Clear();
    }

    public int Connect(string deviceId, string productId, string? sct = null)
    {
        var connection = _client.CreateConnection();
        ConnectionOptions options = new ConnectionOptions
        {
            PrivateKey = _privateKey,
            DeviceId = deviceId,
            ProductId = productId
        };
        if (sct != null)
        {
            options.ServerConnectToken = sct;
        }
        connection.SetOptions(options);
        try
        {
            connection.ConnectAsync().Wait();
            Console.WriteLine("Successfully connected to device {0} {1}", productId, deviceId);
        }
        catch (Exception e)
        {
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
            Console.WriteLine("Adding connection [{0}]", id);
            connection.Connection.ConnectionEventHandlers += ((e) =>
            {
                Console.WriteLine("Got connection event {0} on connection [{1}]", e, id);
            });
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
            if (_connections.TryGetValue(id, out entry))
            {
                connection = entry.Connection;
                return true;
            }
            else
            {
                connection = null;
                return false;
            }
        }
    }
    public Dictionary<int, ConnectionEntry> GetConnectionEntries()
    {
        return _connections;
    }


    /// <inheritdoc/>
    public void Dispose()
    {
        Console.WriteLine("*** ConnectionManager Dispose called");
        if (!_disposedUnmanaged)
        {
            StopAsync().Wait();
            _disposedUnmanaged = true;
        }
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        Console.WriteLine("*** ConnectionManager DisposeAsync called");
        await StopAsync();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    ~ConnectionManager()
    {
        Console.WriteLine("*** ConnectionManager finalizer called");
        StopAsync().Wait();
    }

}