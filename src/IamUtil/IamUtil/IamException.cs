namespace Nabto.Edge.Client;
public class IamException : Exception
{
    public IamError Error { get; }
    public IamException(IamError e) 
        : base(e.ToString())
    {
        Error = e;
    }
};