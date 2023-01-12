namespace Nabto.Edge.Client;

public interface CoapResponse {
    public ushort GetResponseStatusCode();
    public ushort GetResponseContentFormat();
    public byte[] GetResponsePayload();
}
