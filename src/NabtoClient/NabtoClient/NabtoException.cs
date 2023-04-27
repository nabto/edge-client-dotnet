namespace Nabto.Edge.Client;

/**
 * <summary>
 * Base class for Nabto specific exceptions.
 * </summary>
 */
public class NabtoException : Exception
{
     /* TODO
     * Access the underlying Nabto Edge Client SDK error code.
     *
     * @return The underlying Nabto Edge Client SDK error code.
     */
    public int ErrorCode { get; }

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
    public NotConnectedException() :
        base("The connection is not connected")
    {
    }
}
