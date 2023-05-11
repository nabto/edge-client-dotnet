using System.Runtime.InteropServices;

namespace Nabto.Edge.Client.Impl;

internal class ReadOperation
{

    internal IntPtr Buffer { get; }
    internal UIntPtr ReadLength;
    internal UIntPtr BufferLength;


    internal ReadOperation(int bufferSize)
    {
        Buffer = Marshal.AllocHGlobal(bufferSize);
        BufferLength = (UIntPtr)bufferSize;
    }

    ~ReadOperation()
    {
        Marshal.FreeHGlobal(Buffer);
    }
}

internal class WriteOperation
{

    internal IntPtr Buffer { get; }
    internal UIntPtr BufferLength { get; }
    internal WriteOperation(byte[] data)
    {
        BufferLength = (UIntPtr)data.Length;
        Buffer = Marshal.AllocHGlobal(data.Length);
        for (int i = 0; i < data.Length; i++)
        {
            unsafe
            {
                byte* b = (byte*)Buffer;
                b[i] = data[i];
            }

        }
    }
}

/// <inheritdoc/>
public class Stream : Nabto.Edge.Client.Stream
{

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClientImpl _client;
    private Nabto.Edge.Client.Impl.ConnectionImpl _connection;

    internal static Nabto.Edge.Client.Stream Create(Nabto.Edge.Client.Impl.NabtoClientImpl client, Nabto.Edge.Client.Impl.ConnectionImpl connection)
    {
        IntPtr ptr = NabtoClientNative.nabto_client_stream_new(connection.GetHandle());
        if (ptr == IntPtr.Zero)
        {
            throw new NullReferenceException();
        }
        return new Stream(client, connection, ptr);
    }

    /// <inheritdoc/>
    public Stream(Nabto.Edge.Client.Impl.NabtoClientImpl client, Nabto.Edge.Client.Impl.ConnectionImpl connection, IntPtr handle)
    {
        _client = client;
        _connection = connection;
        _handle = handle;
    }

    /// <inheritdoc/>
    ~Stream()
    {
        NabtoClientNative.nabto_client_stream_free(_handle);
    }

    /// <inheritdoc/>
    public async Task OpenAsync(UInt32 port)
    {
        TaskCompletionSource openTask = new TaskCompletionSource();
        var task = openTask.Task;

        var future = FutureImpl.Create(_client);

        NabtoClientNative.nabto_client_stream_open(_handle, future.GetHandle(), port);

        var ec = await future.WaitAsync();
        if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value())
        {
            return;
        }
        else
        {
            throw NabtoExceptionFactory.Create(ec);
        }
    }

    /// <inheritdoc/>
    public async Task<byte[]> ReadSomeAsync(int max)
    {
        TaskCompletionSource<byte[]> readTask = new TaskCompletionSource<byte[]>();
        var task = readTask.Task;

        var future = FutureImpl.Create(_client);

        var op = new ReadOperation(max);
        var gcHandle = GCHandle.Alloc(op, GCHandleType.Pinned);

        NabtoClientNative.nabto_client_stream_read_some(_handle, future.GetHandle(), op.Buffer, op.BufferLength, out op.ReadLength);

        var ec = await future.WaitAsync();

        if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value())
        {
            var data = new byte[op.ReadLength];
            unsafe
            {
                byte* b = (byte*)op.Buffer;
                for (uint i = 0; i < (uint)op.ReadLength; i++)
                {
                    data[i] = b[i];
                }
                return data;
            }
        }
        else
        {
            throw NabtoExceptionFactory.Create(ec);
        }
    }

    /// <inheritdoc/>
    public async Task<byte[]> ReadAllAsync(int bytes)
    {
        TaskCompletionSource<byte[]> readTask = new TaskCompletionSource<byte[]>();
        var task = readTask.Task;

        var future = FutureImpl.Create(_client);

        var op = new ReadOperation(bytes);
        var gcHandle = GCHandle.Alloc(op, GCHandleType.Pinned);

        NabtoClientNative.nabto_client_stream_read_all(_handle, future.GetHandle(), op.Buffer, op.BufferLength, out op.ReadLength);

        var ec = await future.WaitAsync();
        if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value())
        {
            var data = new byte[op.ReadLength];
            unsafe
            {
                byte* b = (byte*)op.Buffer;
                for (uint i = 0; i < (uint)op.ReadLength; i++)
                {
                    data[i] = b[i];
                }
                return data;
            }
        }
        else
        {
            throw NabtoExceptionFactory.Create(ec);
        }
    }

    /// <inheritdoc/>
    public async Task WriteAsync(byte[] data)
    {
        TaskCompletionSource writeTask = new TaskCompletionSource();
        var task = writeTask.Task;

        var future = FutureImpl.Create(_client);

        var op = new WriteOperation(data);
        var gcHandle = GCHandle.Alloc(op, GCHandleType.Pinned);

        NabtoClientNative.nabto_client_stream_write(_handle, future.GetHandle(), op.Buffer, op.BufferLength);

        var ec = await future.WaitAsync();

        if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value())
        {
            return;
        }
        else
        {
            throw NabtoExceptionFactory.Create(ec);
        }
    }

    /// <inheritdoc/>
    public async Task CloseAsync()
    {
        TaskCompletionSource closeTask = new TaskCompletionSource();
        var task = closeTask.Task;

        var future = FutureImpl.Create(_client);

        NabtoClientNative.nabto_client_stream_close(_handle, future.GetHandle());

        var ec = await future.WaitAsync();

        if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value())
        {
            return;
        }
        else
        {
            throw NabtoExceptionFactory.Create(ec);
        }
    }
}
