using Nabto.Edge.Client;
using System.Formats.Cbor;

public class GetDeviceInfoCoapRequest : AbstractCommand
{
    public override string Help => "Invoke CoAP GET /iam/pairing to get device info: info <connection id>";
    public override string Name => "info";
    public override int NumArgs => 1;

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

    public override void Execute(string[] args)
    {
        int id;
        if (!Int32.TryParse(args[0], out id)) {
            Console.WriteLine("Invalid id '{0}'", args[0]);
            return;
        }
        Connection connection;
        if (!ConnectionManager.Instance.GetConnection(id, out connection)) {
            Console.WriteLine("No connection with id: {0}", id);
            return;
        }

        try
        {
            var request = connection.CreateCoapRequest("GET", "/iam/pairing");
            var response = request.ExecuteAsync().Result;
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
        } catch (Exception e) {
            Console.WriteLine("Failed to invoke CoAP on device: {0}", e.Message);
            return;
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