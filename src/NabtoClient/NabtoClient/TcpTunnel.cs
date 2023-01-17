namespace Nabto.Edge.Client;

public interface TcpTunnel {
    public Task OpenAsync(string service, ushort localPort);
    public Task CloseAsync();
    public ushort GetLocalPort();
}
