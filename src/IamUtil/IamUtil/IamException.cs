namespace Nabto.Edge.ClientIam;

using System.Text;

/**
 * <summary>The IamException class is used to represent errors reported from the IAM CoAP interface on the Nabto Edge Embedded Device.</summary>
 */
public class IamException : Exception
{
    /**
     * <summary>Access the underlying Nabto Edge Client SDK error code.</summary>
     */
    public IamError Error { get; }

    /**
     * <summary>Create an exception, representing the underlying Nabto Edge Client SDK error code.</summary>
     * <param name="e">The error to represent</param>
     */
    public IamException(IamError e)
        : base(e.ToString())
    {
        Error = e;
    }

    /**
     * <summary>Create an exception, representing the underlying Nabto Edge Client SDK error code.</summary>
     * <param name="message">A description of the exceptional condition</param>
     * <param name="e">The error to represent</param>
     */
    public IamException(string message, IamError e)
        : base(message)
    {
        Error = e;
    }

};
