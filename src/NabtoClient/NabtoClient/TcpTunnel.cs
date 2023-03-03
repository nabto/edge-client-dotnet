namespace Nabto.Edge.Client;

/**
 * The TCP Tunnel API.
 *
 * The TCP Tunnel API is a high level wrapper for streaming, allowing applications to tunnel traffic
 * through Nabto by integrating through a simple TCP socket, just like e.g. SSH tunnels. TCP Tunnels
 * can hence be used to quickly add remote access capabilities to existing applications that already
 * support TCP communication.
 *
 * The client opens a TCP listener which listens for incoming TCP connections on the local
 * port. When a connection is accepted by the TCP listener, a new stream is created to the
 * device. When the stream is created on the device, the device opens a TCP connection to the
 * specified service. Once this connection is opened, TCP data flows from the TCP client on the
 * client side to the TCP server on the device side.
 *
 * TCP Tunnel instances are created using the Connection.createTcpTunnel() factory method.
 * The TcpTunnel object must be kept alive while in use.
 */
public interface TcpTunnel {

    /**
     * Open this tunnel without blocking. The returned task can fail with:
     * - `FORBIDDEN` if the device did not allow opening the tunnel.
     * - `STOPPED` if the tunnel or a parent object is stopped.
     * - `NOT_CONNECTED` if the connection is not established yet.
     *
     * @param service The service to connect to on the remote device (as defined in the device's
     * configuration), e.g. "http", "http-admin", "ssh", "rtsp".
     * @param localPort The local port to listen on. If 0 is specified, an ephemeral port is used,
     * it can be retrieved with `getLocalPort()`.
     * @return The Task that will complete once the tunnel is opened.
     */
    public Task OpenAsync(string service, ushort localPort);


    /**
     * Close a tunnel without blocking.
     *
     * @return The Task that will complete once the tunnel is closed.
     */
    public Task CloseAsync();

    /**
     * Get the local port which the tunnel is bound to.
     *
     * @throws NabtoException with error code `INVALID_STATE` if the tunnel is not open.
     * @return the local port number used.
     */
    public ushort GetLocalPort();
}
