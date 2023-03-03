namespace Nabto.Edge.Client;

// TODO: this doc was copied from the android SDK. The Android SDK has ABORTED not found on this list. This list has BAD_REQUEST and PRIVILEGED_PORT not found in the android list. Sync these lists, and also include swift in the sync.

/**
 * This class represents error codes that the nabto client sdk exposes. The documentation for the
 * actual error codes varies slightly with context, so see the detailed documentation for the Nabto
 * Client SDK for details where the codes can be returned.
 */
public class NabtoClientError
{
    /**
     * Operation was successful.
     */
    public static int OK = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_OK_value();

    /**
     * Unexpected or wrongly formatted response from remote peer.
     */
    public static int BAD_RESPONSE = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_BAD_RESPONSE_value();

    /**
     * Unexpected or wrongly formatted resquest.
     */
    public static int BAD_REQUEST = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_BAD_REQUEST_value();

    /**
     * Connection is closed.
     */
    public static int CLOSED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_CLOSED_value();

    /**
     * A DNS related error occurred.
     */
    public static int DNS = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_DNS_value();

    /**
     * End-of-file was reached
     */
    public static int EOF = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_EOF_value();

    /**
     * The current user has been rejected access to the requested resource.
     */
    public static int FORBIDDEN = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_FORBIDDEN_value();

    /**
     * The queried future is not resolved yet.
     */
    public static int FUTURE_NOT_RESOLVED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_FUTURE_NOT_RESOLVED_value();

    /**
     * Invalid argument specified.
     */
    public static int INVALID_ARGUMENT = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_INVALID_ARGUMENT_value();

    /**
     * A function was invoked on the SDK when it was not in an appropriate state, for instance
     * setting configuration parameters on a connection after it is connected.
     */
    public static int INVALID_STATE = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_INVALID_STATE_value();

    /**
     * The connection failed for some unspecified reason.
     */
    public static int NOT_CONNECTED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NOT_CONNECTED_value();

    /**
     * An entity was not found (e.g., no hosts found during discovery).
     */
    public static int NOT_FOUND = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NOT_FOUND_value();

    /**
     * Functionality not implemented.
     */
    public static int NOT_IMPLEMENTED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NOT_IMPLEMENTED_value();

    /**
     * It was not possible to open any channel (p2p, relay, direct) to the target device, ie no
     * connection could be established.
     */
    public static int NO_CHANNELS = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NO_CHANNELS_value();

    /**
     * No data received.
     */
    public static int NO_DATA = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NO_DATA_value();

    /**
     * Another operation is in progress that may prevent the desired function to complete
     * successfully.
     */
    public static int OPERATION_IN_PROGRESS = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_OPERATION_IN_PROGRESS_value();

    /**
     * Parse error.
     */
    public static int PARSE = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_PARSE_value();

    /**
     * Requested port is already use.
     */
    public static int PORT_IN_USE = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_PORT_IN_USE_value();

    /**
     * The requested entity has been stopped.
     */
    public static int STOPPED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_STOPPED_value();

    /**
     * A timeout occurred before the operation completed.
     */
    public static int TIMEOUT = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_TIMEOUT_value();

    /**
     * An unknown error occured.
     */
    public static int UNKNOWN = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_UNKNOWN_value();

    /**
     * Always depends on context.
     */
    public static int NONE = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NONE_value();

    /**
     * The target device is not attached to any server (it is offline).
     */
    public static int NOT_ATTACHED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NOT_ATTACHED_value();

    /**
     * The basestation rejected a connection due to invalid SCT or JWT
     */
    public static int TOKEN_REJECTED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_TOKEN_REJECTED_value();

    /**
     * An operation would have blocked a thread that should not be blocked
     */
    public static int COULD_BLOCK = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_COULD_BLOCK_value();

    /**
     * A request was did not have proper authorization
     */
    public static int UNAUTHORIZED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_UNAUTHORIZED_value();

    /**
     * A request was rejected due to rate limiting
     */
    public static int TOO_MANY_REQUESTS = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_TOO_MANY_REQUESTS_value();

    /**
     * The basestation rejected a connection due to unknown product ID
     */
    public static int UNKNOWN_PRODUCT_ID = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_UNKNOWN_PRODUCT_ID_value();

    /**
     * The basestation rejected a connection due to unknown device ID
     */
    public static int UNKNOWN_DEVICE_ID = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_UNKNOWN_DEVICE_ID_value();

    /**
     * The basestation rejected a connection due to unknown server key
     */
    public static int UNKNOWN_SERVER_KEY = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_UNKNOWN_SERVER_KEY_value();

    /**
     * A connection was refused by the network
     */
    public static int CONNECTION_REFUSED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_CONNECTION_REFUSED_value();

    /**
     * A TCP Tunnel failed to open as it was attempted on a privileged port
     */
    public static int PRIVILEGED_PORT = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_PRIVILEGED_PORT_value();

    public static string GetErrorMessage(int ec) {
        return Nabto.Edge.Client.Impl.NabtoClientNative.nabto_client_error_get_message(ec);
    }

    public static string GetErrorString(int ec) {
        return Nabto.Edge.Client.Impl.NabtoClientNative.nabto_client_error_get_string(ec);
    }
}
