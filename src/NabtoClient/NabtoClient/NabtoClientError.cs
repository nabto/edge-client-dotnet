namespace Nabto.Edge.Client;

public class NabtoClientError
{
    public static int OK = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_OK_value();
    public static int BAD_RESPONSE = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_BAD_RESPONSE_value();
    public static int BAD_REQUEST = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_BAD_REQUEST_value();
    public static int CLOSED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_CLOSED_value();
    public static int DNS = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_DNS_value();
    public static int EOF = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_EOF_value();
    public static int FORBIDDEN = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_FORBIDDEN_value();
    public static int FUTURE_NOT_RESOLVED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_FUTURE_NOT_RESOLVED_value();
    public static int INVALID_ARGUMENT = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_INVALID_ARGUMENT_value();
    public static int INVALID_STATE = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_INVALID_STATE_value();
    public static int NOT_CONNECTED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NOT_CONNECTED_value();
    public static int NOT_FOUND = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NOT_FOUND_value();
    public static int NOT_IMPLEMENTED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NOT_IMPLEMENTED_value();
    public static int NO_CHANNELS = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NO_CHANNELS_value();
    public static int NO_DATA = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NO_DATA_value();
    public static int OPERATION_IN_PROGRESS = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_OPERATION_IN_PROGRESS_value();
    public static int PARSE = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_PARSE_value();
    public static int PORT_IN_USE = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_PORT_IN_USE_value();
    public static int STOPPED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_STOPPED_value();
    public static int TIMEOUT = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_TIMEOUT_value();
    public static int UNKNOWN = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_UNKNOWN_value();
    public static int NONE = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NONE_value();
    public static int NOT_ATTACHED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NOT_ATTACHED_value();
    public static int TOKEN_REJECTED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_TOKEN_REJECTED_value();
    public static int COULD_BLOCK = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_COULD_BLOCK_value();
    public static int UNAUTHORIZED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_UNAUTHORIZED_value();
    public static int TOO_MANY_REQUESTS = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_TOO_MANY_REQUESTS_value();
    public static int UNKNOWN_PRODUCT_ID = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_UNKNOWN_PRODUCT_ID_value();
    public static int UNKNOWN_DEVICE_ID = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_UNKNOWN_DEVICE_ID_value();
    public static int UNKNOWN_SERVER_KEY = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_UNKNOWN_SERVER_KEY_value();
    public static int CONNECTION_REFUSED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_CONNECTION_REFUSED_value();
    public static int PRIVILEGED_PORT = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_PRIVILEGED_PORT_value();

    public static string GetErrorMessage(int ec) {
        return Nabto.Edge.Client.Impl.NabtoClientNative.nabto_client_error_get_message(ec);
    }

    public static string GetErrorString(int ec) {
        return Nabto.Edge.Client.Impl.NabtoClientNative.nabto_client_error_get_string(ec);
    }
}
