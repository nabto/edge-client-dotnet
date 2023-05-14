public class CommandInvoker
{
    private readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();

    public List<ICommand> GetCommands() {  
        return _commands.Values.ToList().OrderBy(x => x).ToList();
    }

    public void RegisterCommand(ICommand command)
    {
        _commands[command.Name] = command;
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

}