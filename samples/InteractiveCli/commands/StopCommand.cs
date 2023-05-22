using Nabto.Edge.Client;

public class StopCommand : AbstractCommand
{
    public StopCommand(IConnectionManager connectionManager) : base(connectionManager)
    {
    }

    public override string Help => "Stop the connection manager.";

    public override string Name => "stop";

    public override int NumArgs => 0;
    
    public override void Execute(string[] args)
    {
        // exercise the IAsyncDisposable implementation 
        _connectionManager.StopAsync().Wait();
    }
}