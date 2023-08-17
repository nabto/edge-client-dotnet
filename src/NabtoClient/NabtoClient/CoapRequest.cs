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
public interface ICoapRequest : IDisposable, IAsyncDisposable
{

    /**
     * <summary>
     * Set payload and content format for the payload.
     * </summary>
     * <param name="contentFormat"> See https://www.iana.org/assignments/core-parameters/core-parameters.xhtml, some often used values are defined in ContentFormat.</param>
     * <param name="data"> Data for the request encoded as specified in the `contentFormat` parameter.</param>
     * <exception cref="NabtoException">Thrown with error code `FAILED` if payload could not be set</exception>
     */
    public void SetRequestPayload(ushort contentFormat, byte[] data);

    /**
     * <summary>
     * <para>Execute a CoAP request asynchronously.</para>
     *
     * </summary>
     * <returns>
     *     Task which completes with a CoapResponse when the async operation completes
     * </returns>
     * <exception cref="NabtoException">Thrown with error code `TIMEOUT` if the request timed out.</exception>
     * <exception cref="NabtoException">Thrown with error code `STOPPED` if the coap request or a parent object is stopped.</exception>
     * <exception cref="NabtoException">Thrown with error code `NOT_CONNECTED` if the connection is not established yet.</exception>
     */
    public Task<CoapResponse> ExecuteAsync();

    /**
     * <summary>Stop this CoAP request.</summary>
     */
    public void Stop();
}
