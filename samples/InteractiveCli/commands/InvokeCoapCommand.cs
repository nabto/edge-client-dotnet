using Nabto.Edge.Client;
using System.Formats.Cbor;

public class InvokeCoapCommand : AbstractCommand
{
    public override string Help => "Invoke CoAP request on a connection: coap <id> <method> <path> [<payload>]";
    public override string Name => "invoke";
    public override int NumArgs => 3;
    
    public override void Execute(string[] args)
    {
        int id;
        if (!Int32.TryParse(args[0], out id)) {
            Console.WriteLine("Invalid id '{0}'", args[0]);
            return;
        }
        Connection connection;
        if (!_connectionManager.GetConnection(id, out connection)) {
            Console.WriteLine("No connection with id: {0}", id);
            return;
        }

        try {
            using (var request = connection.CreateCoapRequest(args[1], args[2])) {
                var response = request.ExecuteAsync().Result;
                var status = response.GetResponseStatusCode();
                var format = response.GetResponseContentFormat();
                var payload = System.Text.Encoding.UTF8.GetString(response.GetResponsePayload());
                Console.WriteLine($"Successfully invoked CoAP request, response status is [{status}], format is [{format}], payload dumped as string:");
                Console.WriteLine(payload);
            }
        } catch (Exception e) {
            Console.WriteLine("Failed to invoke CoAP on device: {0}", e.Message);
            return;
        }

    }
}