namespace Nabto.Edge.Client;

public class ConnectionOptions {
    public string? PrivateKey { get; set; }
    public string? ProductId { get; set; }
    public string? DeviceId { get; set; }
    public string? ServerUrl { get; set; }
    public string? ServerKey { get; set; }
    public string? ServerJwtToken { get; set; }
    public string? ServerConnectToken { get; set; }
    public string? AppName { get; set; }
    public string? AppVersion { get; set; }

    public int? KeepAliveInterval { get; set; }
    public int? KeepAliveRetryInterval { get; set; }
    public int? KeepAliveMaxRetries { get; set; }

    public int? DtlsHelloTimeout { get; set; }

    public bool? Local { get; set; }

    public bool? Remote { get; set; }

    public bool? Rendezvous { get; set; }

    public bool? ScanLocalConnect { get; set; }
}

/**
 * This interface represents a connection to a specific Nabto Edge device. The Connection object must
 * be kept alive for the duration of all streams, tunnels, and CoAP sessions created from it.
 *
 * Instances are created using `NabtoClient.createConnection()`.
 */
public interface Connection
{
    /**
     * Connection events
     *
     * - `Connected`: a connection is established
     * - `Closed`: a connection is closed
     * - `ChannelChanged`: the underlying channel has changed, e.g. from relay to p2p
     */
    public enum ConnectionEvent {
        Connected,
        Closed,
        ChannelChanged
    }

    /**
     * ConnectionEvent delegate to receive connection events.
     * @param e resulting ConnectionEvent
     */
    public delegate void ConnectionEventHandler(ConnectionEvent e);

    /* TODO
     * Set a ConnectionEventHandler to receive connection events.
     * @param e resulting ConnectionEvent
     */
    public ConnectionEventHandler? ConnectionEventHandlers { get; set; }


    /**
     * Set connection options. Options must be set prior to invoking `connect()`.
     *
     * @param json The options to set as a JSON-string
     * @throws INVALID_ARGUMENT if input is invalid
     */
    public void SetOptions(string json);

    /**
     * Set connection options. Options must be set prior to invoking `connect()`.
     *
     * @param options The options to set as ConnectionOptions object
     * @throws INVALID_ARGUMENT if input is invalid
     */
    public void SetOptions(ConnectionOptions options);


    /**
     * Get the full fingerprint of the remote device public key. The fingerprint is used to validate
     * the identity of the remote device.
     *
     * @throws INVALID_STATE if the connection is not established.
     * @return The fingerprint encoded as hex.
     */
    public string GetDeviceFingerprint();

    /**
     * Get the fingerprint of the client public key used for this connection.
     * @throws INVALID_STATE if the connection is not established.
     * @return The fingerprint encoded as hex.
     */
    public string GetClientFingerprint();

    /**
     * Establish this connection asynchronously.
     *
     * The returned task is completed with an error if an error occurs.
     * - `UNAUTHORIZED` if the authentication options do not match the basestation configuration
     * for this
     * - `TOKEN_REJECTED` if the basestation could not validate the specified token
     * - `STOPPED` if the client instance was stopped
     * - `NO_CHANNELS` if all parameters input were accepted but a connection could not be
     * established. Details about what went wrong are available as the
     * associated localError and remoteError.
     * - `NO_CHANNELS.remoteError.NOT_ATTACHED` if the target remote device is not attached to the basestation
     * - `NO_CHANNELS.remoteError.FORBIDDEN` if the basestation request is rejected
     * - `NO_CHANNELS.remoteError.NONE` if remote relay was not enabled
     * - `NO_CHANNELS.localError.NONE` if mDNS discovery was not enabled
     * - `NO_CHANNELS.localError.NOT_FOUND` if no local device was found
     *
     * @return Task completed when the connect attempt succeeds or fails.
     */
    public Task ConnectAsync();

    /**
     * Close this connection asynchronously.
     *
     * The returned Task completes with an error if an error occurs.
     * - `NabtoClientError` if an error occurs during close.
     *
     * @return Task completed when the close succeeds or fails.
     */
    public Task CloseAsync();

    public Task PasswordAuthenticate(string username, string password);

    /**
     * Get underlying error code on local channel.
     *
     * Possible local channel error code are:
     *
     * - `NOT_FOUND` if the device was not found locally
     * - `NONE` if mDNS discovery was not enabled
     *
     * @return the local channel error code
     */
    public int GetLocalChannelErrorCode();

    /**
     * Get underlying error code on remote channel.
     *
     * Possible remote channel error code are:
     *
     * - `NOT_ATTACHED` if the target remote device is not attached to the basestation
     * - `TIMEOUT` if a timeout occured when connecting to the basestation.
     * - `FORBIDDEN` if the basestation request is rejected
     * - `TOKEN_REJECTED` if the basestation rejected based on an invalid SCT or JWT
     * - `DNS` if the server URL failed to resolve
     * - `UNKNOWN_SERVER_KEY` if the provided server key was not known by the basestation
     * - `UNKNOWN_PRODUCT_ID` if the provided product ID was not known by the basestation
     * - `UNKNOWN_DEVICE_ID` if the provided device ID was not known by the basestation
     * - `NONE` if remote relay was not enabled
     *
     * @return the remote channel error code
     */
    public int GetRemoteChannelErrorCode();

    /**
     * Get the direct channel error code.
     *
     * Possible direct channel error code are:
     *
     * - `NOT_FOUND` if no responses was received on any added direct channels
     * - `NONE` if direct channels was not enabled
     *
     * @return the direct channel error code
     */
    public int GetDirectCandidatesChannelErrorCode();

    /**
     * Create a coap request object. The returned CoapRequest object must be kept alive while in use.
     *
     * @param method CoAP request method e.g. GET, POST or PUT
     * @param path CoAP request path e.g. /hello-world
     * @return the created CoapRequest object.
     */
    public Nabto.Edge.Client.CoapRequest CreateCoapRequest(string method, string path);

    /**
     * Create stream. The returned Stream object must be kept alive while in use.
     *
     * @return The created stream.
     */
    public Nabto.Edge.Client.Stream CreateStream();

    /**
     * Create a TCP tunnel. The returned TcpTunnel object must be kept alive while in use.
     *
     * @return The created TCP tunnel.
     */
    public Nabto.Edge.Client.TcpTunnel CreateTcpTunnel();

};
