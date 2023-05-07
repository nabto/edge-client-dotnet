namespace Nabto.Edge.Client.Impl;

internal class NabtoExceptionFactory
{
    internal static Exception Create(int ec)
    {
        return new Nabto.Edge.Client.NabtoException(ec);
    }
}
