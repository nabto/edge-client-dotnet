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
public class StreamImpl : Nabto.Edge.Client.IStream
{

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClientImpl _client;
    private Nabto.Edge.Client.Impl.ConnectionImpl _connection;
    private bool _disposed;

    internal static Nabto.Edge.Client.IStream Create(Nabto.Edge.Client.Impl.NabtoClientImpl client, Nabto.Edge.Client.Impl.ConnectionImpl connection)
    {
        IntPtr ptr = NabtoClientNative.nabto_client_stream_new(connection.GetHandle());
        if (ptr == IntPtr.Zero)
        {
            throw new AllocationException("Could not allocate Stream");
        }
        return new StreamImpl(client, connection, ptr);
    }

    private IntPtr GetHandle()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException("Stream", "The Stream instance has been disposed.");
        }
        return _handle;
    }

    /// <inheritdoc/>
    public StreamImpl(Nabto.Edge.Client.Impl.NabtoClientImpl client, Nabto.Edge.Client.Impl.ConnectionImpl connection, IntPtr handle)
    {
        _client = client;
        _connection = connection;
        _handle = handle;
    }

    /// <inheritdoc/>
    public async Task OpenAsync(UInt32 port)
    {
        TaskCompletionSource openTask = new TaskCompletionSource();
        var task = openTask.Task;

        var future = FutureImpl.Create(_client);

        NabtoClientNative.nabto_client_stream_open(GetHandle(), future.GetHandle(), port);

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

        NabtoClientNative.nabto_client_stream_read_some(GetHandle(), future.GetHandle(), op.Buffer, op.BufferLength, out op.ReadLength);

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

        NabtoClientNative.nabto_client_stream_read_all(GetHandle(), future.GetHandle(), op.Buffer, op.BufferLength, out op.ReadLength);

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

        NabtoClientNative.nabto_client_stream_write(GetHandle(), future.GetHandle(), op.Buffer, op.BufferLength);

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

        NabtoClientNative.nabto_client_stream_close(GetHandle(), future.GetHandle());

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
    public void Stop()
    {
        NabtoClientNative.nabto_client_stream_stop(GetHandle());
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    ~StreamImpl()
    {
        Dispose(false);
    }

    /// <summary>Do the actual resource disposal here</summary>
    protected void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            Stop();
            NabtoClientNative.nabto_client_stream_free(_handle);
            _disposed = true;
        }
    }
}
