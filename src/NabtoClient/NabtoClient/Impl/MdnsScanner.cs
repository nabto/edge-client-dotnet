using System.Runtime.InteropServices;

namespace Nabto.Edge.Client.Impl;

public class MdnsResult : Nabto.Edge.Client.MdnsResult
{
    public static MdnsResult Create(IntPtr result)
    {
        string serviceInstanceName = NabtoClientNative.nabto_client_mdns_result_get_service_instance_name(result);
        string productId = NabtoClientNative.nabto_client_mdns_result_get_product_id(result);
        string deviceId = NabtoClientNative.nabto_client_mdns_result_get_device_id(result);
        int action = NabtoClientNative.nabto_client_mdns_result_get_action(result);

        return new MdnsResult { ServiceInstanceName = serviceInstanceName, ProductId = productId, DeviceId = deviceId, Action = (Client.MdnsResult.MdnsAction)action };
    }
}

public class MdnsScanner : Nabto.Edge.Client.MdnsScanner
{
    private Nabto.Edge.Client.Impl.NabtoClient _client;
    private IntPtr _listener;
    private Future _future;
    private IntPtr _mdnsResult;
    private string? _subtype;
    public MdnsResultHandler Handlers { get; set; }

    public static Nabto.Edge.Client.MdnsScanner Create(Nabto.Edge.Client.Impl.NabtoClient client, string? subtype)
    {
        var listener = NabtoClientNative.nabto_client_listener_new(client.GetHandle());
        var future = Future.Create(client);
        return new MdnsScanner(client, future, listener, subtype);
    }

    private MdnsScanner(Nabto.Edge.Client.Impl.NabtoClient client, Future future, IntPtr listener, string? subtype) {
        Handlers = (r => {});
        _client = client;
        _future = future;
        _listener = listener;
        _subtype = subtype;
    }

    public void Start()
    {
        int ec = NabtoClientNative.nabto_client_mdns_resolver_init_listener(_client.GetHandle(),_listener, _subtype);
        if (ec != 0) {
            throw NabtoException.Create(ec);
        }
        StartListen();
    }

    private void StartListen() {
        NabtoClientNative.nabto_client_listener_new_mdns_result(_listener, _future.GetHandle(), out _mdnsResult);
        _future.Wait((int ec) => {

            if (ec == 0) {
                MdnsResult r = MdnsResult.Create(_mdnsResult);
                Handlers.Invoke(r);
                NabtoClientNative.nabto_client_mdns_result_free(_mdnsResult);
            }

            StartListen();
        });
    }
}
