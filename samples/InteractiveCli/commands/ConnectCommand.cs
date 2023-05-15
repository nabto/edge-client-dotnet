using Nabto.Edge.Client;

public class ConnectCommand : AbstractCommand
{
    public override string Help => "Connect to specified device: connect {<product-id> <device-id> [<sct>] | <pairing-string> }";

    public override string Name => "connect";

    public override int NumArgs => 1;
    
    public override void Execute(string[] args)
    {
        if (args.Length == 1) {
            TryAsPairingString(args[0]);
        } else {
            string productId = args[0];
            string deviceId = args[1];
            string? sct = args.Length > 2 ? args[2] : null;        
            _connectionManager.Connect(deviceId: deviceId, productId: productId, sct: sct);
        }   
    }

    public void TryAsPairingString(string input) {
        var tokens = new Dictionary<string, string>();
        var pairs = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var pair in pairs)
        {
            var parts = pair.Split(new[] { '=' }, 2);
            if (parts.Length != 2)
            {
                Console.WriteLine($"Invalid pairing string member: {pair}");
                return;
            }

            var key = parts[0].Trim();
            var value = parts[1].Trim();
            if (string.IsNullOrEmpty(key))
            {
                Console.WriteLine($"Invalid pairing string (empty key): {pair}");
                return;
            }
            tokens[key] = value;
        }

        if (tokens.ContainsKey("p") && tokens.ContainsKey("d")) {
            string? sct = tokens.ContainsKey("sct") ? tokens["sct"] : null;
            _connectionManager.Connect(deviceId: tokens["d"], productId: tokens["p"], sct: sct);
        } else {
            Console.WriteLine("Invalid pairing string - either spcecify product-id and device-id or a valid pairing string.");
        }

    }
}