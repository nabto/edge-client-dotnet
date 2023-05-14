public interface ICommand: IComparable<ICommand>
{
    string Help { get; }

    string Name { get; }

    int NumArgs { get; }

    void Execute(string[] args);

    bool ValidateArgs(string[] args);
}

public abstract class AbstractCommand : ICommand {

    public abstract string Help { get; }

    public abstract string Name { get; }

    public abstract int NumArgs { get; }

    public abstract void Execute(string[] args);

    public int CompareTo(ICommand other) {
        return Name.CompareTo(other.Name);
    }

    public bool ValidateArgs(string[] args) {
        if (args.Length >= NumArgs) {
            return true;
        } else {
            Console.WriteLine("Invalid number of arguments");
            Console.WriteLine(Help);
            return false;
        }
    }   
}