namespace Nabto.Edge.Client.Impl;

public class NabtoException
{
    public static Exception Create(int ec)
    {
        return new Nabto.Edge.Client.NabtoException(ec);
    }
}
