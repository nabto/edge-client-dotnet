namespace Nabto.Edge.Client;

using System.Text;

public class IamException : Exception
{
    public IamError Error { get; }

    public IamException(IamError e)
        : base(e.ToString())
    {
        Error = e;
    }
    public IamException(string message, IamError e) 
        : base(message) 
    {
        Error = e;
    }

};