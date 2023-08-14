using System.Runtime.InteropServices;

namespace Nabto.Edge.Client.Impl;

internal class MdnsResultImpl : Nabto.Edge.Client.MdnsResult
{
    public static MdnsResultImpl Create(IntPtr result)
    {
        string serviceInstanceName = NabtoClientNative.nabto_client_mdns_result_get_service_instance_name(result);
        string productId = NabtoClientNative.nabto_client_mdns_result_get_product_id(result);
        string deviceId = NabtoClientNative.nabto_client_mdns_result_get_device_id(result);
        int action = NabtoClientNative.nabto_client_mdns_result_get_action(result);

        return new MdnsResultImpl { ServiceInstanceName = serviceInstanceName, ProductId = productId, DeviceId = deviceId, Action = (Client.MdnsResult.MdnsAction)action };
    }
}

internal class MdnsScannerImpl : Nabto.Edge.Client.IMdnsScanner
{
    private Nabto.Edge.Client.Impl.NabtoClientImpl _client;
    private ListenerImpl _listener;
    private FutureImpl _future;
    private string _subtype;
    private bool _disposed;

    private bool _stopped;

    /// <inheritdoc/>
    public Nabto.Edge.Client.IMdnsScanner.ResultHandler? Handlers { get; set; }

    internal static Nabto.Edge.Client.IMdnsScanner Create(Nabto.Edge.Client.Impl.NabtoClientImpl client, string subtype)
    {
        var listener = ListenerImpl.Create(client);
        var future = FutureImpl.Create(client);
        return new MdnsScannerImpl(client, future, listener, subtype);
    }

    private MdnsScannerImpl(Nabto.Edge.Client.Impl.NabtoClientImpl client, FutureImpl future, ListenerImpl listener, string subtype)
    {
        _client = client;
        _future = future;
        _listener = listener;
        _subtype = subtype;
    }

    /// <inheritdoc/>
    public void Start()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException("MdnsScanner", "The MdnsScanner has been disposed.");
        }
        int ec = NabtoClientNative.nabto_client_mdns_resolver_init_listener(_client.GetHandle(), _listener.GetHandle(), _subtype);
        if (ec != 0)
        {
            throw NabtoExceptionFactory.Create(ec);
        }
        StartListen();
    }

    /// <inheritdoc/>
    public void Stop()
    {
        if (_stopped)
        {
            return;
        }
        _stopped = true;
        _listener.Stop();
    }

    private async void StartListen()
    {
        // make sure the underlying pointer of the mdnsResult stay the same.
        IntPtr mdnsResult = new IntPtr();
        GCHandle handle = GCHandle.Alloc(mdnsResult, GCHandleType.Pinned);
        while (true)
        {
            NabtoClientNative.nabto_client_listener_new_mdns_result(_listener.GetHandle(), _future.GetHandle(), out mdnsResult);
            var ec = await _future.WaitAsync();

            if (ec == 0)
            {
                MdnsResultImpl r = MdnsResultImpl.Create(mdnsResult);
                Handlers?.Invoke(r);
                NabtoClientNative.nabto_client_mdns_result_free(mdnsResult);
            }
            else if (ec == NabtoClientError.STOPPED)
            {
                _listener.Dispose();
                _future.Dispose();
                return;
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    ~MdnsScannerImpl()
    {
        Dispose(false);
    }

    /// <summary>Do the actual resource disposal here</summary>
    protected void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Stop();
            }
            _disposed = true;
        }
    }
}
