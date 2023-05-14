public class HelpCommand: AbstractCommand {

    List<ICommand> _commands;

    public override string Name => "help";
    public override string Help => "show help";

    public override int NumArgs => 0;

    public HelpCommand(List<ICommand> commands)
    {
        _commands = commands;
    }

    public override void Execute(string[] args) {
        Console.WriteLine("Available commands:");
        foreach (var command in _commands) {            
            Console.WriteLine($"  {command.Name}: {command.Help}");
        }       

    }
}