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
     * </summary>
     * <param name="service"> The service to connect to on the remote device (as defined in the device's
     * configuration), e.g. "http", "http-admin", "ssh", "rtsp".</param>
     * <param name="localPort"> The local port to listen on. If 0 is specified, an ephemeral port is used,
     * it can be retrieved with `getLocalPort()`.</param>
     * <returns>
     *     The Task that will complete once the tunnel is opened.
     * </returns>
     * <exception cref="NabtoException">Thrown with error code `NOT_FOUND` if requesting an unknown service. </exception>
     * <exception cref="NabtoException">Thrown with error code `FORBIDDEN` if target device did not allow opening a tunnel to specified service for the current client.</exception>
     * <exception cref="NabtoException">Thrown with error code `STOPPED` if the tunnel is stopped.</exception>
     * <exception cref="NabtoException">Thrown with error code `NOT_CONNECTED` if the connection is not established yet..</exception>     
     * <exception cref="NabtoException">Thrown with error code `PRIVILEGED_PORT` if the connection is not established because the port is privileged and the user does not have access to start a listening socket on that port number.</exception>     
     */
    public Task OpenAsync(string service, ushort localPort);


    /**
     * <summary>
     * Close a tunnel without blocking.
     * </summary>
     * <returns>
     *     The Task that will complete once the tunnel is closed.
     * </returns>
     * <exception cref="NabtoException">Thrown with error code `STOPPED` if the tunnel is stopped.</exception>     
     * <exception cref="NabtoException">Thrown with error code `INVALID_STATE`  if the tunnel has not been opened yet.</exception>     
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
     * <exception cref="NabtoException">Thrown with error code `INVALID_STATE`  if the tunnel has not been opened ye..</exception>     
     */
    public ushort GetLocalPort();

    /**
     * <summary>Stop a tunnel. Stop can be used to cancel async functions like open and close. The tunnel cannot be used after it has been stopped.</summary>
     */
    public void Stop();
}
