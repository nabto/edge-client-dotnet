namespace Nabto.Edge.Client;

public interface CoapRequest {
    public void SetRequestPayload(ushort contentFormat, byte[] data);
    public Task<CoapResponse> ExecuteAsync();
}
