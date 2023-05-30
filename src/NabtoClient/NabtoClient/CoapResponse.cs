namespace Nabto.Edge.Client;

/**
 * <summary>
 * This interface encapsulates a CoAP response, resulting from executing a CoapRequest.
 *
 * See https://docs.nabto.com/developer/guides/get-started/coap/intro.html for info about Nabto Edge
 * CoAP.
 * </summary>
 */
public interface CoapResponse
{

    /**
     * <summary>
     * The CoAP response status code.
     *
     * </summary>
     * <returns>
     *     the CoAP response status code.
     * </returns>
     */
    public ushort GetResponseStatusCode();


    /**
     * <summary>
     * The CoAP response content format.
     *
     * </summary>
     * <returns>
     *     the CoAP response content format.
     * </returns>
     */
    public ushort GetResponseContentFormat();


    /**
     * <summary>
     * The CoAP payload, if the status indicates such.
     *
     * </summary>
     * <returns>
     *     the CoAP response payload.
     * </returns>
     */
    public byte[] GetResponsePayload();
}
