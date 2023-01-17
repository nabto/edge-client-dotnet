namespace Nabto.Edge.Client;

public interface Stream {
    public Task OpenAsync(UInt32 port);
    public Task<byte[]> ReadSomeAsync(int max);
    public Task<byte[]> ReadAllAsync(int bytes);
    public Task WriteAsync(byte[] data);
    public Task CloseAsync();
}
