var invoker = new CommandInvoker();
invoker.RegisterCommand(new VersionCommand());
invoker.RegisterCommand(new ListConnectionsCommand());
invoker.RegisterCommand(new ConnectCommand());
invoker.RegisterCommand(new ShowPublicKeyFingerprintCommand());

invoker.RegisterCommand(new HelpCommand(invoker.GetCommands()));

Console.WriteLine("Nabto Edge Interactive CLI");
Console.WriteLine("Write 'help' to see available commands");

ConnectionManager.Instance.Start();

while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine();
    if (line == "exit")
    {
        break;
    }
    invoker.ExecuteCommand(line);
}