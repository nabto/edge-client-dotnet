namespace Nabto.Edge.Client.Impl;

internal class NabtoExceptionFactory
{
    internal static Exception Create(int ec)
    {
        if (ec == NabtoClientError.INVALID_ARGUMENT)
        {
            return new ArgumentException();
        }
        else
        {
            return new Nabto.Edge.Client.NabtoException(ec);
        }
    }
}
