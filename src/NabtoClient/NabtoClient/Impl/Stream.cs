using System.Runtime.InteropServices;

namespace Nabto.Edge.Client.Impl;

public class ReadOperation {

    public IntPtr Buffer { get; }
    public UIntPtr ReadLength;
    public UIntPtr BufferLength;


    public ReadOperation(int bufferSize) {
        Buffer = Marshal.AllocHGlobal(bufferSize);
        BufferLength = (UIntPtr)bufferSize;
    }

    ~ReadOperation() {
        Marshal.FreeHGlobal(Buffer);
    }
}

public class WriteOperation {

    public IntPtr Buffer { get; }
    public UIntPtr BufferLength { get; }
    public WriteOperation(byte[] data) {
        BufferLength = (UIntPtr)data.Length;
        Buffer = Marshal.AllocHGlobal(data.Length);
        for (int i = 0; i < data.Length; i++) {
            unsafe {
                byte* b = (byte*)Buffer;
                b[i] = data[i];
            }

        }
    }
}

public class Stream : Nabto.Edge.Client.Stream {

    private IntPtr _handle;
    private Nabto.Edge.Client.Impl.NabtoClient _client;
    private Nabto.Edge.Client.Impl.Connection _connection;

    public static Nabto.Edge.Client.Stream Create(Nabto.Edge.Client.Impl.NabtoClient client, Nabto.Edge.Client.Impl.Connection connection)
    {
        IntPtr ptr = NabtoClientNative.nabto_client_stream_new(connection.GetHandle());
        if (ptr == IntPtr.Zero) {
            throw new NullReferenceException();
        }
        return new Stream(client, connection, ptr);
    }

    public Stream(Nabto.Edge.Client.Impl.NabtoClient client, Nabto.Edge.Client.Impl.Connection connection, IntPtr handle)
    {
        _client = client;
        _connection = connection;
        _handle = handle;
    }

    ~Stream() {
        NabtoClientNative.nabto_client_stream_free(_handle);
    }

    public Task OpenAsync(UInt32 port)
    {
        TaskCompletionSource connectTask = new TaskCompletionSource();
        var task = connectTask.Task;

        var future = Future.Create(_client);

        NabtoClientNative.nabto_client_stream_open(_handle, future.GetHandle(), port);

        future.Wait((ec) => {
            if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value()) {
                connectTask.SetResult();
            } else {
                connectTask.SetException(NabtoException.Create(ec));
            }
        });

        return task;
    }

    public Task<byte[]> ReadSomeAsync(int max)
    {
        TaskCompletionSource<byte[]> connectTask = new TaskCompletionSource<byte[]>();
        var task = connectTask.Task;

        var future = Future.Create(_client);

        var op = new ReadOperation(max);

        NabtoClientNative.nabto_client_stream_read_some(_handle, future.GetHandle(), op.Buffer, op.BufferLength, out op.ReadLength);

        future.Wait((ec) => {
            if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value()) {
                var data = new byte[op.ReadLength];
                unsafe {
                byte* b = (byte*)op.Buffer;
                for (uint i = 0; i < (uint)op.ReadLength; i++) {
                    data[i] = b[i];
                }
                connectTask.SetResult(data);
                }
            } else {
                connectTask.SetException(NabtoException.Create(ec));
            }
        });

        return task;
    }
    public Task<byte[]> ReadAllAsync(int bytes) {
        TaskCompletionSource<byte[]> connectTask = new TaskCompletionSource<byte[]>();
        var task = connectTask.Task;

        var future = Future.Create(_client);

        var op = new ReadOperation(bytes);

        NabtoClientNative.nabto_client_stream_read_all(_handle, future.GetHandle(), op.Buffer, op.BufferLength, out op.ReadLength);

        future.Wait((ec) => {
            if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value()) {
                var data = new byte[op.ReadLength];
                unsafe {
                byte* b = (byte*)op.Buffer;
                for (uint i = 0; i < (uint)op.ReadLength; i++) {
                    data[i] = b[i];
                }
                connectTask.SetResult(data);
                }
            } else {
                connectTask.SetException(NabtoException.Create(ec));
            }
        });

        return task;
    }
    public Task WriteAsync(byte[] data)
    {
        TaskCompletionSource connectTask = new TaskCompletionSource();
        var task = connectTask.Task;

        var future = Future.Create(_client);

        var op = new WriteOperation(data);

        NabtoClientNative.nabto_client_stream_write(_handle, future.GetHandle(), op.Buffer, op.BufferLength);

        future.Wait((ec) => {
            if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value()) {

                connectTask.SetResult();
            } else {
                connectTask.SetException(NabtoException.Create(ec));
            }
        });

        return task;
    }
    public Task CloseAsync()
    {
        TaskCompletionSource connectTask = new TaskCompletionSource();
        var task = connectTask.Task;

        var future = Future.Create(_client);

        NabtoClientNative.nabto_client_stream_close(_handle, future.GetHandle());

        future.Wait((ec) => {
            if (ec == NabtoClientNative.NABTO_CLIENT_EC_OK_value()) {

                connectTask.SetResult();
            } else {
                connectTask.SetException(NabtoException.Create(ec));
            }
        });

        return task;
    }
}
