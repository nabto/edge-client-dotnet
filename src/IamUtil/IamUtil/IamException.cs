namespace Nabto.Edge.ClientIam;

using System.Text;

/**
 * <summary>The IamException class is used to represent errors reported from the IAM CoAP interface on the Nabto Edge Embedded Device.</summary>
 */
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
