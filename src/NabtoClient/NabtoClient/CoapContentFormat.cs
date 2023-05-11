namespace Nabto.Edge.Client;

/**
 * <summary>Often used CoAP content formats, see https://www.iana.org/assignments/core-parameters/core-parameters.xhtml for exhaustive list.</summary>
 */
public class CoapContentFormat
{
    /**
     * <summary>Plain text content.</summary>
     */
    public static ushort TEXT_PLAIN = 0;

    /**
     * <summary>XML content.</summary>
     */
    public static ushort APPLICATION_XML = 41;

    /**
     * <summary>Data stream content.</summary>
     */
    public static ushort APPLICATION_OCTET_STREAM = 42;

    /**
     * <summary>JSON encoded content.</summary>
     */
    public static ushort APPLICATION_JSON = 50;

    /**
     * <summary>CBOR encoded content.</summary>
     */
    public static ushort APPLICATION_CBOR = 60;
};