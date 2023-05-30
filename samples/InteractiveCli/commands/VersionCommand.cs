using Nabto.Edge.Client;

public class VersionCommand : AbstractCommand
{
    public override string Help => "Print the Nabto Edge Client SDK version";

    public override string Name => "version";

    public override int NumArgs => 0;
    
    public override void Execute(string[] args)
    {
        var client = Nabto.Edge.Client.NabtoClient.Create();

        Console.WriteLine("Nabto Edge Client SDK Version: {0}", client.GetVersion());
    }
}