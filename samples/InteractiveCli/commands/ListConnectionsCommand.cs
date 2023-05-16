public class ListConnectionsCommand: AbstractCommand {
    public ListConnectionsCommand(IConnectionManager connectionManager) : base(connectionManager)
    {
    }

    public override string Name => "list";
    public override string Help => "List established connections";
    public override int NumArgs => 0;

    public override void Execute(string[] args) {
        Console.WriteLine("Established connections:");
        var connectionEntries = _connectionManager.GetConnectionEntries();

        if (connectionEntries.Count > 0) {
            var keys = connectionEntries.Keys.ToList().OrderBy(x => x);
            foreach (var key in keys) {         
                Console.WriteLine($"  [{key}]: {connectionEntries[key].ProductId}.{connectionEntries[key].DeviceId}");
            }       
        } else {
            Console.WriteLine($"  None.");
        }
    }
}