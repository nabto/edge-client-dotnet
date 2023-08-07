namespace Nabto.Edge.Client;

/**
 * <summary>
 *
 * <para>The Stream API.</para>
 *
 * <para>The Streaming API enables socket-like communication between client and device. The stream is
 * reliable and ensures data is received ordered and complete. If either of these conditions cannot be
 * met, the stream will be closed in such a way that it is detectable.</para>
 *
 * <para>Stream instances are created using the Connection.createStream() factory method.
 * The Stream object must be kept alive while in use.</para>
 *
 * </summary>
 */
public interface Stream : IDisposable, IAsyncDisposable
{

    /**
     * <summary>
     * Open a stream asynchronously. 
     * </summary>
     * <param name="port">  The port to use on the remote server, a
     * streamPort is a demultiplexing id.</param>
     * <returns>
     *     The Task that will complete once the stream is opened.
     * </returns>
     * <exception cref="NabtoException">Thrown with error code `STOPPED` if the stream could not be created, e.g. the handshake is stopped/aborted or the connection or client context is stopped.</exception>
     * <exception cref="NabtoException">Thrown with error code `NOT_CONNECTED` if the connection is not established yet.</exception>
     */
    public Task OpenAsync(UInt32 port);

    /**
     * <summary>
     * Read some bytes from a stream without blocking.
     * The returned Task can fail with:
     * <list type="bullet">
     * <item>
     *   <description>
     *     `STOPPED` if the stream was stopped.
     *   </description>
     * </item>
     * <item>
     *   <description>
     *     `OPERATION_IN_PROGRESS` if another read is in progress.
     *   </description>
     * </item>
     * <item>
     *   <description>
     *     `EOF` if eof is reached
     *   </description>
     * </item>
     * </list>
     *
     * </summary>
     * <param name="max"> The max number of bytes to read</param>
     * <returns>
     *     The Task that will complete when the bytes are ready.
     * </returns>
     * <exception cref="NabtoException">Thrown with error code `STOPPED` if the stream was stopped.</exception>
     * <exception cref="NabtoException">Thrown with error code `OPERATION_IN_PROGRESS` if another read is in progress.</exception>
     * <exception cref="NabtoException">Thrown with error code `EOF` if eof is reached.</exception>
     */
    public Task<byte[]> ReadSomeAsync(int max);

    /**
     * <summary>
     * Read an exact amount of bytes from a stream.
     * </summary>
     * <param name="bytes"> The number of bytes to read</param>
     * <returns>
     *     The Task that will complete when the bytes are ready.
     * </returns>
     * <exception cref="NabtoException">Thrown with error code `STOPPED` if the stream was stopped.</exception>
     * <exception cref="NabtoException">Thrown with error code `OPERATION_IN_PROGRESS` if another read is in progress.</exception>
     * <exception cref="NabtoException">Thrown with error code `EOF` if eof is reached.</exception>
     */
    public Task<byte[]> ReadAllAsync(int bytes);

    /**
     * <summary>
     * Write bytes to a stream.
     * </summary>
     * <param name="data">  The data to write to the stream.</param>
     * <returns>
     *     The Task that will complete once the operation is done.
     * </returns>
     * <exception cref="NabtoException">Thrown with error code `CLOSED` if the stream is closed for writing.</exception>
     * <exception cref="NabtoException">Thrown with error code `STOPPED` if the stream was stopped.</exception>
     * <exception cref="NabtoException">Thrown with error code `OPERATION_IN_PROGRESS` if another write is in progress.</exception>
     * <exception cref="NabtoException">Thrown with error code `EOF` if eof is reached.</exception>
     */
    public Task WriteAsync(byte[] data);

    /**
     * <summary>
     * <para>Close the write direction of the stream. This will make the
     * other end reach end of file when reading from a stream when all
     * sent data has been received and acknowledged.</para>
     *
     * <para>A call to close does not affect the read direction of the
     * stream.</para>
     * </summary>
     * <returns>
     *     The Task that will complete once the operation is done.
     * </returns>
     * <exception cref="NabtoException">Thrown with error code `STOPPED` if the stream was stopped.</exception>
     * <exception cref="NabtoException">Thrown with error code `OPERATION_IN_PROGRESS` if a stream close or stream write is in progress.</exception>
     * <exception cref="NabtoException">Thrown with error code `EOF` if eof is reached.</exception>
     */
    public Task CloseAsync();

    /**
     * <summary>All pending read operations are stopped. The write direction is also closed.</summary>
     */
    public void Stop();
}
