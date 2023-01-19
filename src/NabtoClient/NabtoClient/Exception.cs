namespace Nabto.Edge.Client;

public class NabtoException : Exception
{
    public int ErrorCode { get; }
    public int Error { get; }

    public NabtoException(int ec)
        : base(NabtoClientError.GetErrorMessage(ec))
    {
        ErrorCode = ec;
    }
}

public class NotConnectedException : Exception {
    public NotConnectedException() :
        base("The connection is not connected")
    {
    }
}
