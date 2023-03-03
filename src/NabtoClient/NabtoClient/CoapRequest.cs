namespace Nabto.Edge.Client;

/**
 * This interface represents a CoAP request on an open connection, ready to be executed.
 *
 * Instances are created using createCoapRequest() function on the Connection class.
 * The CoapRequest object must be kept alive while in use.
 *
 * See https://docs.nabto.com/developer/guides/get-started/coap/intro.html for info about Nabto Edge
 * CoAP.
 */
 public interface CoapRequest {

    /**
     * Set payload and content format for the payload.
     * @param contentFormat See https://www.iana.org/assignments/core-parameters/core-parameters.xhtml, some often used values are defined in ContentFormat.
     * @param data Data for the request encoded as specified in the `contentFormat` parameter.
     * @throws NabtoEdgeClientError.FAILED if payload could not be set
     */
    public void SetRequestPayload(ushort contentFormat, byte[] data);

    /**
     * Execute a CoAP request asynchronously.
     *
     * The specified closure is invoked when the response is ready or an early error occurs.
     *
     * If a response is available, the first parameter in the CoapResponseReceiver closure
     * invocation is OK and the second parameter is set to the created CoapResponse.
     *
     * If an early error occurs, the first parameter is set to an appropriate NabtoEdgeClientError
     * and the second parameter is nil.
     *
     * @return Task which completes with a CoapResponse when the async operation completes
     */
    public Task<CoapResponse> ExecuteAsync();
}
