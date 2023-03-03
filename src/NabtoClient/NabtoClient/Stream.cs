namespace Nabto.Edge.Client;

/**
 * The Stream API.
 *
 * The Streaming API enables socket-like communication between client and device. The stream is
 * reliable and ensures data is received ordered and complete. If either of these conditions cannot be
 * met, the stream will be closed in such a way that it is detectable.
 *
 * Stream instances are created using the Connection.createStream() factory method.
 * The Stream object must be kept alive while in use.
 *
 */
public interface Stream {

    /**
     * Open a stream async.
     * The returned task can fail with
     * - `STOPPED` if the stream or a parent object was stopped.
     * - `NOT_CONNECTED` if the connection is not established yet.
     *
     * @param streamPort  The streamPort to use on the remote server, a
     * streamPort is a demultiplexing id.
     * @return The Task that will complete once the stream is opened.
     */
    public Task OpenAsync(UInt32 port);

    /**
     * Read some bytes from a stream without blocking.
     * The returned Task can fail with:
     * - `STOPPED` if the stream was stopped.
     * - `OPERATION_IN_PROGRESS` if another read is in progress.
     * - `EOF` if eof is reached
     *
     * @param max The max number of bytes to read
     * @return The Task that will complete when the bytes are ready.
     */
    public Task<byte[]> ReadSomeAsync(int max);

    /**
     * Read an exact amount of bytes from a stream.
     * The returned Task can fail with:
     * - `STOPPED` if the stream was stopped.
     * - `OPERATION_IN_PROGRESS` if another read is in progress.
     * - `EOF` if eof is reached
     *
     * @param bytes The number of bytes to read
     * @return The Task that will complete when the bytes are ready.
     */
    public Task<byte[]> ReadAllAsync(int bytes);

    /**
     * Write bytes to a stream.
     *
     * @param bytes  The bytes to write to the stream.
     * @return The Task that will complete once the operation is done.
     */
    public Task WriteAsync(byte[] data);

    /**
     * Close the write direction of the stream. This will make the
     * other end reach end of file when reading from a stream when all
     * sent data has been received and acknowledged.
     *
     * A call to close does not affect the read direction of the
     * stream.
     *
     * Close can fail with the with:
     * - `STOPPED` if the stream is stopped.
     * - `OPERATION_IN_PROGRESS` if another stop is in progress.
     * - `INVALID_STATE` if the stream is not opened yet.
     *
     * @return The Task that will complete once the operation is done.
     */
    public Task CloseAsync();
}
