namespace Nabto.Edge.Client;

/**
 * <summary>
 * Base class for Nabto specific exceptions.
 * </summary>
 */
public class NabtoException : Exception
{
    /**
     * <summary>Access the underlying Nabto Edge Client SDK error code.</summary>
     * @return The underlying Nabto Edge Client SDK error code.
     */
    public int ErrorCode { get; }

    /**
     * <summary>Create an exception, representing the underlying Nabto Edge Client SDK error code.</summary>
     * <param name="ec">The error to represent</param>
     */
    public NabtoException(int ec)
        : base(NabtoClientError.GetErrorMessage(ec))
    {
        ErrorCode = ec;
    }
}

/**
 * <summary>
 * This exception is thrown from a function requiring a Connection to be connected but it was not.
 * </summary>
 */
public class NotConnectedException : Exception {
    /**
     * <summary>Create a NotConnectedException</summary>
     */
    public NotConnectedException() :
        base("The connection is not connected")
    {
    }
}
