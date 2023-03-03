namespace Nabto.Edge.Client;

/**
 * This interface encapsulates a CoAP response, resulting from executing a CoapRequest.
 *
 * See https://docs.nabto.com/developer/guides/get-started/coap/intro.html for info about Nabto Edge
 * CoAP.
 */
 public interface CoapResponse {

    /**
     * The CoAP response status code.
     *
     * @return the CoAP response status code.
     */
    public ushort GetResponseStatusCode();


    /**
     * The CoAP response content format.
     *
     * @return the CoAP response content format.
     */
    public ushort GetResponseContentFormat();


    /**
     * The CoAP payload, if the status indicates such.
     *
     * @return the CoAP response payload.
     */
    public byte[] GetResponsePayload();
}
