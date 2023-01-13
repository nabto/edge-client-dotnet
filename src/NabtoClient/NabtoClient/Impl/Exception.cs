namespace Nabto.Edge.Client.Impl;

public class NabtoException
{

    public static Exception Create(int ec)
    {
        if (ec == NabtoClientNative.NABTO_CLIENT_EC_NOT_CONNECTED_value())
        {
            return new Nabto.Edge.Client.NotConnectedException();
        }
        else if (ec == NabtoClientNative.NABTO_CLIENT_EC_INVALID_ARGUMENT_value())
            return new ArgumentException();
        else
        {
            return new Exception(NabtoClientNative.nabto_client_error_get_message(ec));
        }
    }
}
