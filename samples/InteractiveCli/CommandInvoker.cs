public class CommandInvoker
{
    private readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();

    public CommandInvoker(IConnectionManager connectionManager)
    {
        RegisterCommands(connectionManager);
    }

    public List<ICommand> GetCommands() {  
        return _commands.Values.ToList().OrderBy(x => x).ToList();
    }

    public void RegisterCommand(ICommand command)
    {
        _commands[command.Name] = command;
    }

    private void RegisterCommands(IConnectionManager connectionManager)
    {
        RegisterCommand(new VersionCommand());
        RegisterCommand(new ListConnectionsCommand(connectionManager));
        RegisterCommand(new ConnectCommand(connectionManager));
        RegisterCommand(new ConnectAxisCommand(connectionManager));
        RegisterCommand(new ShowPublicKeyFingerprintCommand(connectionManager));
        RegisterCommand(new InvokeCoapCommand(connectionManager));
        RegisterCommand(new GetDeviceInfoCoapRequest(connectionManager));
        RegisterCommand(new StopCommand(connectionManager));
        RegisterCommand(new CloseCommand(connectionManager));
        RegisterCommand(new HelpCommand(GetCommands()));
    }

    public void ExecuteCommand(string line)
    {
        var parts = line.Split(' ');
        var name = parts[0];
        var args = parts.Skip(1).ToArray();

        if (_commands.TryGetValue(name, out var command))
        {
            if (command.ValidateArgs(args)) {
                command.Execute(args);  
            }
        }
        else
        {
            Console.WriteLine($"Unknown command: {name}");
        }
    }

    public void Run() {
        Console.WriteLine("Nabto Edge Interactive CLI");
        Console.WriteLine("Write 'help' to see available commands and 'exit' to stop.");
        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine();
            if (line == "exit")
            {
                break;
            }
            ExecuteCommand(line);
        }
    }

}