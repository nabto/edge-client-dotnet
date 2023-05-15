using Nabto.Edge.Client;
using System.Formats.Cbor;

public class DeviceData
{
    public List<string> Modes { get; set; }
    public string NabtoVersion { get; set; }
    public string AppVersion { get; set; }
    public string AppName { get; set; }
    public string ProductId { get; set; }
    public string DeviceId { get; set; }
    public string FriendlyName { get; set; }
}

public class GetDeviceInfoCoapRequest : AbstractCommand
{
    public override string Help => "Invoke CoAP GET /iam/pairing to get device info: info <connection id>";
    public override string Name => "info";
    public override int NumArgs => 1;

    public override void Execute(string[] args)
    {
        int id;
        if (!Int32.TryParse(args[0], out id))
        {
            Console.WriteLine("Invalid id '{0}'", args[0]);
            return;
        }
        Connection connection;
        if (!_connectionManager.GetConnection(id, out connection))
        {
            Console.WriteLine("No connection with id: {0}", id);
            return;
        }

        AsyncInvoker invoker = new AsyncInvoker(connection, "GET", "/iam/pairing");
        invoker.Execute().Wait();
    }


    // to exercise coap request's async disposable
    public class AsyncInvoker
    {

        Connection _connection;
        string _method;
        string _path;

        public AsyncInvoker(Connection connection, string method, string path)
        {
            _connection = connection;
            _method = method;
            _path = path;
        }

        public async Task Execute()
        {
            await using (var request = _connection.CreateCoapRequest(_method, _path))
            {
                try
                {
                    var response = await request.ExecuteAsync();
                    var status = response.GetResponseStatusCode();
                    var format = response.GetResponseContentFormat();
                    Console.WriteLine($"Successfully invoked CoAP request, response status is [{status}]. Trying to decode payload.");
                    var data = DeserializeDeviceData(response.GetResponsePayload());
                    Console.WriteLine("Got device info:");
                    Console.WriteLine($"  Nabto version: {data.NabtoVersion}");
                    Console.WriteLine($"  App version: {data.AppVersion}");
                    Console.WriteLine($"  App name: {data.AppName}");
                    Console.WriteLine($"  Product id: {data.ProductId}");
                    Console.WriteLine($"  Device id: {data.DeviceId}");
                    Console.WriteLine($"  Friendly name: {data.FriendlyName}");
                    Console.WriteLine($"  Pairing Modes: {string.Join(", ", data.Modes)}");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to invoke CoAP on device: {0}", e.Message);
                    return;
                }
            }
        }

        private static List<string> ReadStringArray(CborReader reader)
        {
            var result = new List<string>();

            var arrayLength = reader.ReadStartArray();

            if (arrayLength == null)
            {
                while (reader.PeekState() != CborReaderState.EndArray)
                {
                    result.Add(reader.ReadTextString());
                }
            }
            else
            {
                // TODO: handle unexpected array type
            }

            reader.ReadEndArray();

            return result;
        }

        private static DeviceData DeserializeDeviceData(byte[] cbor)
        {
            CborReader reader = new CborReader(cbor);
            var data = new DeviceData();
            reader.ReadStartMap();
            while (reader.PeekState() != CborReaderState.EndMap)
            {
                string key = reader.ReadTextString();
                switch (key)
                {
                    case "Modes":
                        data.Modes = ReadStringArray(reader);
                        break;
                    case "NabtoVersion":
                        data.NabtoVersion = reader.ReadTextString();
                        break;
                    case "AppVersion":
                        data.AppVersion = reader.ReadTextString();
                        break;
                    case "AppName":
                        data.AppName = reader.ReadTextString();
                        break;
                    case "ProductId":
                        data.ProductId = reader.ReadTextString();
                        break;
                    case "DeviceId":
                        data.DeviceId = reader.ReadTextString();
                        break;
                    case "FriendlyName":
                        data.FriendlyName = reader.ReadTextString();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }
            return data;
        }
    }



}


