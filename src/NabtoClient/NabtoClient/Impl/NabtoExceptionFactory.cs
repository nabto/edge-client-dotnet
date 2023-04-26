namespace Nabto.Edge.Client.Impl;

public class NabtoExceptionFactory
{
    public static Exception Create(int ec)
    {
        return new Nabto.Edge.Client.NabtoException(ec);
    }
}
