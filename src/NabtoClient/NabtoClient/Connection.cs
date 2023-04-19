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
 * <summary>
 * This interface represents a connection to a specific Nabto Edge device. The Connection object must
 * be kept alive for the duration of all streams, tunnels, and CoAP sessions created from it.
 *
 * Instances are created using <c>NabtoClient.createConnection()</c>.
 * </summary>
 */
public interface Connection
{
    /**
     * <summary>
     * Connection events
     *
     * - <c>Connected</c>: a connection is established
     * - <c>Closed</c>: a connection is closed
     * - <c>ChannelChanged</c>: the underlying channel has changed, e.g. from relay to p2p
     * </summary>
     */
    public enum ConnectionEvent {
        Connected,
        Closed,
        ChannelChanged
    }

    /**
     * <summary>
     * ConnectionEvent delegate to receive connection events.
     * </summary>
     * <param name="e">resulting ConnectionEvent</param>
     */
    public delegate void ConnectionEventHandler(ConnectionEvent e);

    /**
     * <summary>
     * Set a ConnectionEventHandler to receive connection events.
     * </summary>
     * <value>Property <c>ConnectionEventHandlers</c> used to subscribe for connection events</value>
     */
    public ConnectionEventHandler? ConnectionEventHandlers { get; set; }


    /**
     * <summary>
     * Set connection options. Options must be set prior to invoking <c>connect()</c>.
     * </summary>
     *
     * <param name="json"> The options to set as a JSON-string</param>
     * <exception cref="NabtoException">Thrown with error code: <c>INVALID_ARGUMENT</c> if input is invalid</exception>
     */
    public void SetOptions(string json);

    /**
     * <summary>
     * Set connection options. Options must be set prior to invoking <c>connect()</c>.
     * </summary>
     *
     * <param name="options"> The options to set as ConnectionOptions object</param>
     * <exception cref="NabtoException">Thrown with error code: <c>INVALID_ARGUMENT</c> if input is invalid</exception>
     */
    public void SetOptions(ConnectionOptions options);


    /**
     * <summary>
     * Get the full fingerprint of the remote device public key. The fingerprint is used to validate
     * the identity of the remote device.
     * </summary>
     *
     * <exception cref="NabtoException">Thrown with error code: <c>INVALID_STATE</c> if the connection is not established.</exception>
     * <returns type="string">The fingerprint encoded as hex.</returns>
     */
    public string GetDeviceFingerprint();

    /**
     * <summary>
     * Get the fingerprint of the client public key used for this connection.
     * </summary>
     * <returns>The fingerprint encoded as hex.</returns>
     * <exception cref="NabtoException">Thrown with error code: <c>INVALID_STATE</c> if the connection is not established.</exception>
     */
    public string GetClientFingerprint();

    /**
     * <summary>
     * Establish this connection asynchronously.
     *
     * The returned task is completed with an error if an error occurs.
     * - <c>UNAUTHORIZED</c> if the authentication options do not match the basestation configuration
     * for this
     * - <c>TOKEN_REJECTED</c> if the basestation could not validate the specified token
     * - <c>STOPPED</c> if the client instance was stopped
     * - <c>NO_CHANNELS</c> if all parameters input were accepted but a connection could not be
     * established. Details about what went wrong are available as the
     * associated localError and remoteError.
     * - <c>NO_CHANNELS.remoteError.NOT_ATTACHED</c> if the target remote device is not attached to the basestation
     * - <c>NO_CHANNELS.remoteError.FORBIDDEN</c> if the basestation request is rejected
     * - <c>NO_CHANNELS.remoteError.NONE</c> if remote relay was not enabled
     * - <c>NO_CHANNELS.localError.NONE</c> if mDNS discovery was not enabled
     * - <c>NO_CHANNELS.localError.NOT_FOUND</c> if no local device was found
     * </summary>
     *
     * <returns>Task completed when the connect attempt succeeds or fails.</returns>
     */
    public Task ConnectAsync();

    /**
     * <summary>
     * Close this connection asynchronously.
     *
     * The returned Task completes with an error if an error occurs.
     * - <c>NabtoClientError</c> if an error occurs during close.
     * </summary>
     *
     * <returns>Task completed when the close succeeds or fails.</returns>
     */
    public Task CloseAsync();

    public Task PasswordAuthenticate(string username, string password);

    /**
     * <summary>
     * Get underlying error code on local channel.
     *
     * Possible local channel error code are:
     *
     * - <c>NOT_FOUND</c> if the device was not found locally
     * - <c>NONE</c> if mDNS discovery was not enabled
     * </summary>
     *
     * <returns>the local channel error code</returns>
     */
    public int GetLocalChannelErrorCode();

    /**
     * <summary>
     * Get underlying error code on remote channel.
     *
     * Possible remote channel error code are:
     *
     * - <c>NOT_ATTACHED</c> if the target remote device is not attached to the basestation
     * - <c>TIMEOUT</c> if a timeout occured when connecting to the basestation.
     * - <c>FORBIDDEN</c> if the basestation request is rejected
     * - <c>TOKEN_REJECTED</c> if the basestation rejected based on an invalid SCT or JWT
     * - <c>DNS</c> if the server URL failed to resolve
     * - <c>UNKNOWN_SERVER_KEY</c> if the provided server key was not known by the basestation
     * - <c>UNKNOWN_PRODUCT_ID</c> if the provided product ID was not known by the basestation
     * - <c>UNKNOWN_DEVICE_ID</c> if the provided device ID was not known by the basestation
     * - <c>NONE</c> if remote relay was not enabled
     * </summary>
     *
     * <returns>the remote channel error code</returns>
     */
    public int GetRemoteChannelErrorCode();

    /**
     * <summary>
     * Get the direct channel error code.
     *
     * Possible direct channel error code are:
     *
     * - <c>NOT_FOUND</c> if no responses was received on any added direct channels
     * - <c>NONE</c> if direct channels was not enabled
     * </summary>
     *
     * <returns>the direct channel error code</returns>
     */
    public int GetDirectCandidatesChannelErrorCode();

    /**
     * <summary>
     * Create a coap request object. The returned CoapRequest object must be kept alive while in use.
     * </summary>
     *
     * <param name="method"> CoAP request method e.g. GET, POST or PUT</param>
     * <param name="path"> CoAP request path e.g. /hello-world</param>
     * <returns>the created CoapRequest object.</returns>
     */
    public Nabto.Edge.Client.CoapRequest CreateCoapRequest(string method, string path);

    /**
     * <summary>
     * Create stream. The returned Stream object must be kept alive while in use.
     * </summary>
     *
     * <returns>The created stream.</returns>
     */
    public Nabto.Edge.Client.Stream CreateStream();

    /**
     * <summary>
     * Create a TCP tunnel. The returned TcpTunnel object must be kept alive while in use.
     * </summary>
     *
     * <returns>The created TCP tunnel.</returns>
     */
    public Nabto.Edge.Client.TcpTunnel CreateTcpTunnel();

};
