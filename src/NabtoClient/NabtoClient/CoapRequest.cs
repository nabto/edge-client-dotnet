namespace Nabto.Edge.Client;

/**
 * <summary>
 *
 * <para>This interface represents a CoAP request on an open connection, ready to be executed.</para>
 *
 * <para>Instances are created using createCoapRequest() function on the Connection class.
 * The CoapRequest object must be kept alive while in use.</para>
 *
 * <para>See https://docs.nabto.com/developer/guides/get-started/coap/intro.html for info about Nabto Edge
 * CoAP.</para>
 * </summary>
 */
 public interface CoapRequest : IDisposable, IAsyncDisposable {

    /**
     * <summary>
     * Set payload and content format for the payload.
     * </summary>
     * <param name="contentFormat"> See https://www.iana.org/assignments/core-parameters/core-parameters.xhtml, some often used values are defined in ContentFormat.</param>
     * <param name="data"> Data for the request encoded as specified in the `contentFormat` parameter.</param>
     * <exception cref="NabtoException">FAILED if payload could not be set</exception>
     */
    public void SetRequestPayload(ushort contentFormat, byte[] data);

    /**
     * <summary>
     * <para>Execute a CoAP request asynchronously.</para>
     *
     * <para>The specified closure is invoked when the response is ready or an early error occurs.</para>
     *
     * <para>If a response is available, the first parameter in the CoapResponseReceiver closure
     * invocation is OK and the second parameter is set to the created CoapResponse.</para>
     *
     * <para>If an early error occurs, the first parameter is set to an appropriate NabtoEdgeClientError
     * and the second parameter is nil.</para>
     *
     * </summary>
     * <returns>
     *     Task which completes with a CoapResponse when the async operation completes
     * </returns>
     */
    public Task<CoapResponse> ExecuteAsync();
}
