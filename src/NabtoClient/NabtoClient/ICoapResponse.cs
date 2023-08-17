namespace Nabto.Edge.Client;

/**
 * <summary>
 * This interface encapsulates a CoAP response, resulting from executing a CoapRequest.
 *
 * See https://docs.nabto.com/developer/guides/get-started/coap/intro.html for info about Nabto Edge
 * CoAP.
 * </summary>
 */
public interface ICoapResponse
{

    /**
     * <summary>
     * The CoAP response status code.
     *
     * </summary>
     * <returns>
     *     the CoAP response status code.
     * </returns>
     * <exception cref="NabtoException">INVALID_STATE if there is no response yet.</exception>
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
     * <exception cref="NabtoException">Thrown with error code `NO_DATA` if the response does not have a content format.</exception>
     * <exception cref="NabtoException">Thrown with error code `INVALID_STATE` if no response is ready.</exception>
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
     * <exception cref="NabtoException">Thrown with error code `NO_DATA` if the response does not have a payload.</exception>
     * <exception cref="NabtoException">Thrown with error code `INVALID_STATE` if no response is ready.</exception>
     */
    public byte[] GetResponsePayload();
}
