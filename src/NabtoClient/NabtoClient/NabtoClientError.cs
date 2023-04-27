namespace Nabto.Edge.Client;

// TODO: this doc was copied from the android SDK. The Android SDK has ABORTED not found on this list. This list has BAD_REQUEST and PRIVILEGED_PORT not found in the android list. Sync these lists, and also include swift in the sync.

/**
 * <summary>
 * This class represents error codes that the nabto client sdk exposes. The documentation for the
 * actual error codes varies slightly with context, so see the detailed documentation for the Nabto
 * Client SDK for details where the codes can be returned.
 * </summary>
 */
public class NabtoClientError
{
    /**
     * <summary>
     * Operation was successful.
     * </summary>
     */
    public static int OK = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_OK_value();

    /**
     * <summary>
     * Unexpected or wrongly formatted response from remote peer.
     * </summary>
     */
    public static int BAD_RESPONSE = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_BAD_RESPONSE_value();

    /**
     * <summary>
     * Unexpected or wrongly formatted resquest.
     * </summary>
     */
    public static int BAD_REQUEST = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_BAD_REQUEST_value();

    /**
     * <summary>
     * Connection is closed.
     * </summary>
     */
    public static int CLOSED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_CLOSED_value();

    /**
     * <summary>
     * A DNS related error occurred.
     * </summary>
     */
    public static int DNS = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_DNS_value();

    /**
     * <summary>
     * End-of-file was reached
     * </summary>
     */
    public static int EOF = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_EOF_value();

    /**
     * <summary>
     * The current user has been rejected access to the requested resource.
     * </summary>
     */
    public static int FORBIDDEN = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_FORBIDDEN_value();

    /**
     * <summary>
     * The queried future is not resolved yet.
     * </summary>
     */
    public static int FUTURE_NOT_RESOLVED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_FUTURE_NOT_RESOLVED_value();

    /**
     * <summary>
     * Invalid argument specified.
     * </summary>
     */
    public static int INVALID_ARGUMENT = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_INVALID_ARGUMENT_value();

    /**
     * <summary>
     * A function was invoked on the SDK when it was not in an appropriate state, for instance
     * setting configuration parameters on a connection after it is connected.
     * </summary>
     */
    public static int INVALID_STATE = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_INVALID_STATE_value();

    /**
     * <summary>
     * The connection failed for some unspecified reason.
     * </summary>
     */
    public static int NOT_CONNECTED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NOT_CONNECTED_value();

    /**
     * <summary>
     * An entity was not found (e.g., no hosts found during discovery).
     * </summary>
     */
    public static int NOT_FOUND = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NOT_FOUND_value();

    /**
     * <summary>
     * Functionality not implemented.
     * </summary>
     */
    public static int NOT_IMPLEMENTED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NOT_IMPLEMENTED_value();

    /**
     * <summary>
     * It was not possible to open any channel (p2p, relay, direct) to the target device, ie no
     * connection could be established.
     * </summary>
     */
    public static int NO_CHANNELS = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NO_CHANNELS_value();

    /**
     * <summary>
     * No data received.
     * </summary>
     */
    public static int NO_DATA = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NO_DATA_value();

    /**
     * <summary>
     * Another operation is in progress that may prevent the desired function to complete
     * successfully.
     * </summary>
     */
    public static int OPERATION_IN_PROGRESS = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_OPERATION_IN_PROGRESS_value();

    /**
     * <summary>
     * Parse error.
     * </summary>
     */
    public static int PARSE = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_PARSE_value();

    /**
     * <summary>
     * Requested port is already use.
     * </summary>
     */
    public static int PORT_IN_USE = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_PORT_IN_USE_value();

    /**
     * <summary>
     * The requested entity has been stopped.
     * </summary>
     */
    public static int STOPPED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_STOPPED_value();

    /**
     * <summary>
     * A timeout occurred before the operation completed.
     * </summary>
     */
    public static int TIMEOUT = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_TIMEOUT_value();

    /**
     * <summary>
     * An unknown error occured.
     * </summary>
     */
    public static int UNKNOWN = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_UNKNOWN_value();

    /**
     * <summary>
     * Always depends on context.
     * </summary>
     */
    public static int NONE = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NONE_value();

    /**
     * <summary>
     * The target device is not attached to any server (it is offline).
     * </summary>
     */
    public static int NOT_ATTACHED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_NOT_ATTACHED_value();

    /**
     * <summary>
     * The basestation rejected a connection due to invalid SCT or JWT
     * </summary>
     */
    public static int TOKEN_REJECTED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_TOKEN_REJECTED_value();

    /**
     * <summary>
     * An operation would have blocked a thread that should not be blocked
     * </summary>
     */
    public static int COULD_BLOCK = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_COULD_BLOCK_value();

    /**
     * <summary>
     * A request was did not have proper authorization
     * </summary>
     */
    public static int UNAUTHORIZED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_UNAUTHORIZED_value();

    /**
     * <summary>
     * A request was rejected due to rate limiting
     * </summary>
     */
    public static int TOO_MANY_REQUESTS = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_TOO_MANY_REQUESTS_value();

    /**
     * <summary>
     * The basestation rejected a connection due to unknown product ID
     * </summary>
     */
    public static int UNKNOWN_PRODUCT_ID = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_UNKNOWN_PRODUCT_ID_value();

    /**
     * <summary>
     * The basestation rejected a connection due to unknown device ID
     * </summary>
     */
    public static int UNKNOWN_DEVICE_ID = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_UNKNOWN_DEVICE_ID_value();

    /**
     * <summary>
     * The basestation rejected a connection due to unknown server key
     * </summary>
     */
    public static int UNKNOWN_SERVER_KEY = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_UNKNOWN_SERVER_KEY_value();

    /**
     * <summary>
     * A connection was refused by the network
     * </summary>
     */
    public static int CONNECTION_REFUSED = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_CONNECTION_REFUSED_value();

    /**
     * <summary>
     * A TCP Tunnel failed to open as it was attempted on a privileged port
     * </summary>
     */
    public static int PRIVILEGED_PORT = Nabto.Edge.Client.Impl.NabtoClientNative.NABTO_CLIENT_EC_PRIVILEGED_PORT_value();

    public static string GetErrorMessage(int ec) {
        return Nabto.Edge.Client.Impl.NabtoClientNative.nabto_client_error_get_message(ec);
    }

    public static string GetErrorString(int ec) {
        return Nabto.Edge.Client.Impl.NabtoClientNative.nabto_client_error_get_string(ec);
    }
}
