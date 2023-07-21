using Nabto.Edge.Client;

public class CloseCommand : AbstractCommand
{
    public CloseCommand(IConnectionManager connectionManager) : base(connectionManager)
    {
    }

    public override string Help => "Close connection with <id>";

    public override string Name => "close";

    public override int NumArgs => 1;
    
    public override void Execute(string[] args)
    {
        if (int.TryParse(args[0], out int id))
        {
            if (_connectionManager.GetConnection(id, out Connection connection))
            {
                connection.CloseAsync().Wait();
                _connectionManager.RemoveConnection(id);
            }
            else
            {
                Console.WriteLine($"No connection with id {id}");
            }
        }
        else
        {
            Console.WriteLine($"Invalid connection id {args[0]}");
        }
    }


    
}