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
 * This exception is thrown if an allocation fails in the underlying Nabto Edge Client SDK. This is most likely due to memory exhaustion.
 * </summary>
 */
public class AllocationException : Exception
{
    /**
     * <summary>Create an AllocationException</summary>
     */
    public AllocationException(string message) :
        base(message)
    {
    }
}
