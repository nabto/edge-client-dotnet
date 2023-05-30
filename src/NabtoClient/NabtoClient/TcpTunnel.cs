namespace Nabto.Edge.Client;

/**
 * <summary>
 *
 * <para>The TCP Tunnel API.</para>
 *
 * <para>The TCP Tunnel API is a high level wrapper for streaming, allowing applications to tunnel traffic
 *  through Nabto by integrating through a simple TCP socket, just like e.g. SSH tunnels. TCP Tunnels
 * can hence be used to quickly add remote access capabilities to existing applications that already
 * support TCP communication.</para>
 *
 * <para>The client opens a TCP listener which listens for incoming TCP connections on the local
 * port. When a connection is accepted by the TCP listener, a new stream is created to the
 * device. When the stream is created on the device, the device opens a TCP connection to the
 * specified service. Once this connection is opened, TCP data flows from the TCP client on the
 * client side to the TCP server on the device side.</para>
 *
 * <para>TCP Tunnel instances are created using the Connection.createTcpTunnel() factory method.
 * The TcpTunnel object must be kept alive while in use.</para>
 * </summary>
 */
public interface TcpTunnel : IDisposable, IAsyncDisposable
{

    /**
     * <summary>
     * Open this tunnel without blocking. The returned task can fail with:
     * <list type="bullet">
     * <item>
     *   <description>
     *     `FORBIDDEN` if the device did not allow opening the tunnel.
     *   </description>
     * </item>
     * <item>
     *   <description>
     *     `STOPPED` if the tunnel or a parent object is stopped.
     *   </description>
     * </item>
     * <item>
     *   <description>
     *     `NOT_CONNECTED` if the connection is not established yet.
     *   </description>
     * </item>
     * </list>
     *
     * </summary>
     * <param name="service"> The service to connect to on the remote device (as defined in the device's
     * configuration), e.g. "http", "http-admin", "ssh", "rtsp".</param>
     * <param name="localPort"> The local port to listen on. If 0 is specified, an ephemeral port is used,
     * it can be retrieved with `getLocalPort()`.</param>
     * <returns>
     *     The Task that will complete once the tunnel is opened.
     * </returns>
     */
    public Task OpenAsync(string service, ushort localPort);


    /**
     * <summary>
     * Close a tunnel without blocking.
     * </summary>
     * <returns>
     *     The Task that will complete once the tunnel is closed.
     * </returns>
     */
    public Task CloseAsync();

    /**
     * <summary>
     * Get the local port which the tunnel is bound to.
     * </summary>
     * <exception cref="NabtoException">NabtoException with error code `INVALID_STATE` if the tunnel is not open.</exception>
     * <returns>
     *     the local port number used.
     * </returns>
     */
    public ushort GetLocalPort();
}
