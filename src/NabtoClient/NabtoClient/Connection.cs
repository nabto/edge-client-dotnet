namespace Nabto.Edge.Client;

/**
 * <summary>Options that can be set on a connection. Alternative to passing a JSON string.</summary>
 */
public class ConnectionOptions
{

    /**
     * <summary>PEM encoded EC private key.</summary>
     */
    public string? PrivateKey { get; set; }

    /**
     * <summary>Product id of the device to connect to.</summary>
     */
    public string? ProductId { get; set; }

    /**
     * <summary>Device id of the device to connect to.</summary>
     */
    public string? DeviceId { get; set; }
    /**
     * <summary>Override the default relay dispatcher endpoint. This is the initial server the client connects to find and make a remote connection to the remote peer. The default endpoint is https://productid.clients.nabto.com. This is only needed if the solution is deployed as a standalone solution with selfmanaged dns.</summary>
     */
    public string? ServerUrl { get; set; }

    /**
     * <summary>Set server key for legacy solutions not using Server Connect Tokens or JWT.</summary>
     */
    public string? ServerKey { get; set; }

    /**
     * <summary>Set a JWT to use when connecting to the Nabto basestation.</summary>
     */
    public string? ServerJwtToken { get; set; }

    /**
     * <summary>Set a Server Connect Token to use when connecting to the Nabto basestation.</summary>
     */
    public string? ServerConnectToken { get; set; }

    /**
     * <summary>Set application name (to simplify diagnostics).</summary>
     */
    public string? AppName { get; set; }

    /**
     * <summary>Set application version (to simplify diagnostics).</summary>
     */
    public string? AppVersion { get; set; }


    /**
     * <summary>Keep alive interval between client and device in milliseconds (default 30000).</summary>
     */
    public int? KeepAliveInterval { get; set; }


    /**
     * <summary>Keep alive retry interval between client and device in milliseconds (default 2000).</summary>
     */
    public int? KeepAliveRetryInterval { get; set; }


    /**
     * <summary>Keep alive max number of retries (default 15).</summary>
     */
    public int? KeepAliveMaxRetries { get; set; }

    /**
     * <summary>Set the timeout for getting the first DTLS packet back from the device. This is used to make a connection attempt fail faster if a route to the device is believed to be open, when in fact it is not. Eg. if the device lost its connection to the Nabto Server, but the server has yet to detect the disconnect. Value in milliseconds, default 10000.</summary>
     */
    public int? DtlsHelloTimeout { get; set; }

    /**
     * <summary>Enable/disable local connections (default enabled).</summary>
     */
    public bool? Local { get; set; }

    /**
     * <summary>Enable/disable remote connections mediated by the basestation, resulting in relay or P2P (default enabled).</summary>
     */
    public bool? Remote { get; set; }

    /**
     * <summary>Enable/disable P2P connections (default enabled).</summary>
     */
    public bool? Rendezvous { get; set; }

    /**
     * <summary>Use the legacy (pre Nabto Edge 5.2) protocol for local discovery.</summary>
     */
    public bool? ScanLocalConnect { get; set; }
}

/**
 * <summary>
 *
 * <para>This interface represents a connection to a specific Nabto Edge device. The Connection object must
 * be kept alive for the duration of all streams, tunnels, and CoAP sessions created from it.</para>
 *
 * <para>Instances are created using <c>NabtoClient.createConnection()</c></para>.
 *
 * </summary>
 */
public interface Connection : IDisposable, IAsyncDisposable
{
    /**
     * <summary>Connection life cycle events.</summary>
     */
    public enum ConnectionEvent
    {
        /**
         * <summary>Connection is established.</summary>
         */
        Connected,

        /**
         * <summary>Connection is closed.</summary>
         */
        Closed,

        /**
         * <summary>The underlying channel has changed, e.g. from relay to P2P.</summary>
         */
        ChannelChanged
    }

    /**
     * <summary>
     * ConnectionEvent delegate to receive connection events.
     *
     * Possible events are:
     *
     * <list type="bullet">
     *   <item>
     *     <description>
     *       `Connected`: a connection is established
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *     `Closed`: a connection is closed
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *     `ChannelChanged`: the underlying channel has changed, e.g. from relay to P2P
     *     </description>
     *   </item>
     * </list>
     * </summary>
     * <param name="e">resulting ConnectionEvent</param>
     */
    public delegate void ConnectionEventHandler(ConnectionEvent e);

    /**
     * <summary>
     * Set a ConnectionEventHandler to receive connection events.
     * </summary>
     */
    public ConnectionEventHandler? ConnectionEventHandlers { get; set; }

    /**
     * <summary>The type of this connection - relay or direct (local or p2p).</summary>
     */
    public enum ConnectionType
    {
        /// <summary>local or p2p connection</summary>
        Direct,
        /// <summary>relay through basestation</summary>
        Relay
    }

    /**
     * <summary>Get the type of this connection.</summary>
     * <exception cref="NabtoException">Thrown with error code `NABTO_CLIENT_EC_NOT_CONNECTED` if the connection is not established.</exception>
     * <exception cref="NabtoException">Thrown with error code `NABTO_CLIENT_EC_NOT_STOPPED` if the connection or client is stopped or closed.</exception>
     * <returns>The type of this connection.</returns>
     */
    public ConnectionType GetConnectionType();

    /**
     * <summary>
     * Set connection options. Options must be set prior to invoking <c>connect()</c>.
     * </summary>
     *
     * <param name="json"> The options to set as a JSON-string</param>
     * <exception cref="ArgumentException">If input is invalid</exception>
     */
    public void SetOptions(string json);

    /**
     * <summary>
     * Set connection options. Options must be set prior to invoking <c>connect()</c>.
     * </summary>
     *
     * <param name="options"> The options to set as ConnectionOptions object</param>
     * <exception cref="ArgumentException">If input is invalid</exception>
     */
    public void SetOptions(ConnectionOptions options);

    /**
     * <summary>
     * Get options currently set on a connection, represented as a JSON string.
     * </summary>
     * <returns type="string">The connection options as a JSON string.</returns>
     */
    public string GetOptionsAsJson();

    /**
     * <summary>
     * Get options currently set on a connection.
     * </summary>
     * <returns type="ConnectionOptions">The connection options.</returns>
     */
    public ConnectionOptions GetOptions();

    /**
     * <summary>
     * Get the full fingerprint of the remote device public key. The fingerprint is used to validate
     * the identity of the remote device.
     * </summary>
     *
     * <exception cref="NabtoException">Thrown with error code `INVALID_STATE` if the connection is not established.</exception>     
     * <exception cref="NabtoException">Thrown with error code `STOPPED` if the connection or the parent client instance are stopped.</exception>     
     * <exception cref="NabtoException">Thrown with error code `NONE` if no fingerprint is available.</exception>     
     * <returns type="string">The fingerprint encoded as hex.</returns>     
     */
    public string GetDeviceFingerprint();

    /**
     * <summary>
     * Get the fingerprint of the client public key used for this connection.
     * </summary>
     * <returns>The fingerprint encoded as hex.</returns>
     * <exception cref="NabtoException">Thrown with error code `INVALID_STATE` if no private key set on connection.</exception>
     */
    public string GetClientFingerprint();

    /**
     * <summary>Establish this connection asynchronously.</summary>
     * <exception cref="NabtoException">Thrown with error code `INVALID_STATE` if the connection is missing required options, for instance a valid private key.</exception>
     * <exception cref="NabtoException">Thrown with error code `TIMEOUT` if the channel to the device was created but the dtls connection to the device timed out.</exception>
     * <exception cref="NabtoException">Thrown with error code `STOPPED` if the client instance was stopped.</exception>
     * <exception cref="NabtoException">Thrown with error code `DEVICE_INTERNAL_ERROR` the device encountered an internal error during the dtls connect attempt. This is most likely due to no more connection resources available in the device.</exception>
     * <exception cref="NabtoException">Thrown with error code `NO_CHANNELS` if all parameters input were accepted but a connection could not be established for some reason. The specific reason can be retrieved using `GetLocalChannelErrorCode()` and `GetRemoteChannelErrorCode()`.</exception>
     * <returns>Task completed when the connect attempt succeeds or fails.</returns>
     */
    public Task ConnectAsync();

    /**
     * <summary>Close this connection asynchronously.</summary>
     * <exception cref="NabtoException">Thrown with error code `OPERATION_IN_PROGRESS` if another close is in progreess.</exception>
     * <exception cref="NabtoException">Thrown with error code `STOPPED` if the client instance was stopped.</exception>
     * <exception cref="NabtoException">Thrown with error code `NOT_CONNECTED` if the connection is not established.</exception>
     * <returns>Task completed when the close succeeds or fails.</returns>
     */
    public Task CloseAsync();

    /**
     * <summary>Stop pending connect or close on a connection. After stop has been called the connection should not be used any more. Stop can be used if the user cancels a connect/close request.</summary>
     */
    public void Stop();

    /**
     * <summary>
     * <para>Do a password authentication exchange with a device. Blocks until authentication attempt is complete.</para>
     *
     * <para>Password authenticate the client and the device. The password authentication is bidirectional and based on PAKE, such that both the client and the device learns that the other end knows the password, without revealing the password to the other end.</para>
     *
     * <para>A specific use case for the password authentication is to prove the identity of a device which identity is not already known, e.g. in a pairing scenario.</para>
     * </summary>
     * <exception cref="NabtoException">Thrown with error code `NABTO_CLIENT_EC_UNAUTHORIZED` if the username and/or password is invalid.</exception>
     * <exception cref="NabtoException">Thrown with error code `NABTO_CLIENT_EC_NOT_FOUND` if the password authentication feature is not available on the device.</exception>
     * <exception cref="NabtoException">Thrown with error code `NABTO_CLIENT_EC_NOT_CONNECTED` if the connection is not established.</exception>
     * <exception cref="NabtoException">Thrown with error code `NABTO_CLIENT_EC_OPERATION_IN_PROGRESS` if a password authentication request is already in progress on the connection.</exception>
     * <exception cref="NabtoException">Thrown with error code `NABTO_CLIENT_EC_TOO_MANY_REQUESTS` if too many password attempts has been made.</exception>
     * <exception cref="NabtoException">Thrown with error code `NABTO_CLIENT_EC_STOPPED` if the client or connection is stopped.</exception>
     * <returns>Task completed when the close succeeds or fails.</returns>
     */
    public Task PasswordAuthenticateAsync(string username, string password);

    /**
     * <summary>
     * Get underlying error code on local channel.     *
     *
     * Possible local channel error code are:
     *
     * <list type="bullet">
     *   <item>
     *     <description>
     *       `NONE`: if mDNS discovery was not enabled for the connection.
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *       `NOT_FOUND`: if mDNS was enabled but the device was not found.
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *       `OPERATION_IN_PROGRESS`: if scanning is still in progress.
     *     </description>
     *   </item>
     * </list>
     * </summary>
     * <returns>the local channel error code</returns>
     */
    public int GetLocalChannelErrorCode();

    /**
     * <summary>
     * Get underlying error code on remote channel.
     *
     * Possible remote channel error code are:
     *
     * <list type="bullet">
     *   <item>
     *     <description>
     *       `NOT_ATTACHED`: If the target remote device is not attached to the basestation (the device is offline).
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *       `TIMEOUT`: If a timeout occured when connecting to the basestation.
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *       `FORBIDDEN`: If the basestation request is rejected.
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *       `TOKEN_REJECTED`: If the basestation rejected based on an invalid SCT or JWT.
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *       `DNS`: If the server URL failed to resolve.
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *       `UNKNOWN_SERVER_KEY`: If the provided server key was not known by the basestation.
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *       `UNKNOWN_PRODUCT_ID`: If the provided product ID was not known by the basestation.
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *       `UNKNOWN_DEVICE_ID`: If the provided device ID was not known by the basestation.
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *       `NONE`: If remote relay was not enabled.
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *       `OPERATION_IN_PROGRESS`: if the opening of the channel is still in progress.
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *       `INVALID_STATE`:  if required options are missing for the remote connection
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *       `CONNECTION_REFUSED`: if the client could not connect to the basestation.
     *     </description>
     *   </item>
     * </list>
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
     * <list type="bullet">
     *   <item>
     *     <description>
     *       `OK`: if a direct candidate was found.
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *       `NONE`: if direct connection was not enabled (no candidates added).
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *       `NOT_FOUND`: if no direct candidates resulted in UDP ping responses.
     *     </description>
     *   </item>
     *   <item>
     *     <description>
     *       `OPERATION_IN_PROGRESS`: if opening of a direct candidate is in progress.
     *     </description>
     *   </item>
     * </list>
     * </summary>
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
     * <exception cref="AllocationException"> if the underlying SDK failed to create the request</exception>
     * <returns>the created CoapRequest object.</returns>
     */
    public Nabto.Edge.Client.CoapRequest CreateCoapRequest(string method, string path);

    /**
     * <summary>
     * Create stream. The returned Stream object must be kept alive while in use.
     * </summary>
     * <exception cref="AllocationException"> if the underlying SDK failed to create the stream</exception>     *
     * <returns>The created stream.</returns>
     */
    public Nabto.Edge.Client.Stream CreateStream();

    /**
     * <summary>
     * Create a TCP tunnel. The returned TcpTunnel object must be kept alive while in use.
     * </summary>
     * <exception cref="AllocationException"> if the underlying SDK failed to create the tunnel</exception>     *
     * <returns>The created TCP tunnel.</returns>
     */
    public Nabto.Edge.Client.TcpTunnel CreateTcpTunnel();

}