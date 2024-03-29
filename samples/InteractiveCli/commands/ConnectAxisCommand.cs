using Nabto.Edge.Client;

public class ConnectAxisCommand : AbstractCommand
{
    public ConnectAxisCommand(IConnectionManager connectionManager) : base(connectionManager)
    {
    }

    public override string Help => "Connect to lab Axis cam";

    public override string Name => "axis";

    public override int NumArgs => 0;
    
    public override void Execute(string[] args)
    {
        ConnectCommand connectCommand = new ConnectCommand(_connectionManager);
        connectCommand.Execute(new string[] { "p=pr-fatqcwj9,d=de-73j47kox,sct=mLjkLTubx9dP"});
    }

}