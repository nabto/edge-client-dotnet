using Nabto.Edge.Client;

public class ShowPublicKeyFingerprintCommand : AbstractCommand
{
    public override string Help => "Show the fingerprint of the public key derived from the client's private key associated with this connection.";

    public override string Name => "fingerprint";
    public override int NumArgs => 0;
    
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
        Console.WriteLine(connection.GetClientFingerprint());
    }
}